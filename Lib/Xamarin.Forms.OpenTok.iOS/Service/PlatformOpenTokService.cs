﻿using System;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms.OpenTok.Service;
using AVFoundation;
using OpenTok;
using Foundation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using static Xamarin.Forms.OpenTok.Service.Enums;

namespace Xamarin.Forms.OpenTok.iOS.Service
{
    [Preserve(AllMembers = true)]
    public sealed class PlatformOpenTokService : BaseOpenTokService
    {
        public event Action PublisherUpdated;
        public event Action SubscriberUpdated;

        private readonly object _sessionLocker = new object();
        private readonly ObservableCollection<string> _subscriberStreamIds = new ObservableCollection<string>();
        private readonly Collection<OTSubscriber> _subscribers = new Collection<OTSubscriber>();

        private OTCameraCaptureResolution _resolution = OTCameraCaptureResolution.High;
        private OTCameraCaptureFrameRate _frameRate = OTCameraCaptureFrameRate.OTCameraCaptureFrameRate15FPS;

        private PlatformOpenTokService()
        {
            PropertyChanged += OnPropertyChanged;
            StreamIdCollection = new ReadOnlyObservableCollection<string>(_subscriberStreamIds);
            Subscribers = new ReadOnlyCollection<OTSubscriber>(_subscribers);
        }

        public static PlatformOpenTokService Instance => CrossOpenTok.Current as PlatformOpenTokService;

        public override ReadOnlyObservableCollection<string> StreamIdCollection { get; }
        public ReadOnlyCollection<OTSubscriber> Subscribers { get; }
        public OTSession Session { get; private set; }
        public OTPublisher PublisherKit { get; private set; }

        public static void Init()
        {
            OpenTokPublisherViewRenderer.Preserve();
            OpenTokSubscriberViewRenderer.Preserve();
            CrossOpenTok.Init(() => new PlatformOpenTokService());
        }

        public override bool TryStartSession(CameraCaptureResolution? resolution = CameraCaptureResolution.High, CameraCaptureFrameRate? frameRate = CameraCaptureFrameRate.Fps15)
        {
            lock (_sessionLocker)
            {
                if (!CheckPermissions() ||
                    string.IsNullOrWhiteSpace(ApiKey) ||
                    string.IsNullOrWhiteSpace(SessionId) ||
                    string.IsNullOrWhiteSpace(UserToken))
                {
                    return false;
                }

                EndSession();
                IsSessionStarted = true;

                _resolution = getParseResolution(resolution);
                _frameRate = getParseFrameRate(frameRate);

                Session = new OTSession(ApiKey, SessionId, null);
                Session.ConnectionDestroyed += OnConnectionDestroyed;
                Session.DidConnect += OnDidConnect;
                Session.StreamCreated += OnStreamCreated;
                Session.StreamDestroyed += OnStreamDestroyed;
                Session.DidFailWithError += OnError;
                Session.ReceivedSignalType += OnSignalReceived;
                Session.Init();

                Session.ConnectWithToken(UserToken, out OTError error);
                using (error)
                {
                    return error == null;
                }
            }
        }

        private OTCameraCaptureResolution getParseResolution(CameraCaptureResolution? resolution)
        {
            OTCameraCaptureResolution reply = OTCameraCaptureResolution.High;

            if (resolution != null)
            {
                switch (resolution)
                {
                    case CameraCaptureResolution.Low:
                        reply = OTCameraCaptureResolution.Low;
                        break;
                    case CameraCaptureResolution.Medium:
                        reply = OTCameraCaptureResolution.Medium;
                        break;
                    case CameraCaptureResolution.High:
                        reply = OTCameraCaptureResolution.High;
                        break;
                }
            }

            return reply;
        }

        private OTCameraCaptureFrameRate getParseFrameRate(CameraCaptureFrameRate? frameRate)
        {
            OTCameraCaptureFrameRate reply = OTCameraCaptureFrameRate.OTCameraCaptureFrameRate15FPS;

            if (frameRate != null)
            {
                switch (frameRate)
                {
                    case CameraCaptureFrameRate.Fps1:
                        reply = OTCameraCaptureFrameRate.OTCameraCaptureFrameRate1FPS;
                        break;
                    case CameraCaptureFrameRate.Fps7:
                        reply = OTCameraCaptureFrameRate.OTCameraCaptureFrameRate7FPS;
                        break;
                    case CameraCaptureFrameRate.Fps15:
                        reply = OTCameraCaptureFrameRate.OTCameraCaptureFrameRate15FPS;
                        break;
                    case CameraCaptureFrameRate.Fps30:
                        reply = OTCameraCaptureFrameRate.OTCameraCaptureFrameRate30FPS;
                        break;
                }
            }

            return reply;
        }

        public override void EndSession()
        {
            lock (_sessionLocker)
            {
                try
                {
                    if (Session == null)
                    {
                        return;
                    }

                    foreach (var subscriberKit in _subscribers)
                    {
                        ClearSubscriber(subscriberKit);
                    }
                    _subscribers.Clear();
                    _subscriberStreamIds.Clear();

                    if (PublisherKit != null)
                    {
                        using (PublisherKit)
                        {
                            PublisherKit.PublishAudio = false;
                            PublisherKit.PublishVideo = false;
                            PublisherKit.StreamCreated -= OnPublisherStreamCreated;
                            Session.Unpublish(PublisherKit);
                        }
                        PublisherKit = null;
                    }

                    RaisePublisherUpdated()
                        .RaiseSubscriberUpdated();

                    if (Session != null)
                    {
                        using (Session)
                        {
                            Session.ConnectionDestroyed -= OnConnectionDestroyed;
                            Session.DidConnect -= OnDidConnect;
                            Session.StreamCreated -= OnStreamCreated;
                            Session.StreamDestroyed -= OnStreamDestroyed;
                            Session.DidFailWithError -= OnError;
                            Session.ReceivedSignalType -= OnSignalReceived;
                            Session.Disconnect();
                        }
                        Session = null;
                    }
                }
                finally
                {
                    IsSessionStarted = false;
                    IsPublishingStarted = false;
                }
            }
        }

        public override bool CheckPermissions() => true;

        public override Task<bool> SendMessageAsync(string message)
        {
            Session.SignalWithType(string.Empty, message, null, out OTError error);
            using (error)
            {
                return Task.FromResult(error == null);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsVideoPublishingEnabled):
                    UpdatePublisherProperty(p => p.PublishVideo = IsVideoPublishingEnabled);
                    return;
                case nameof(IsAudioPublishingEnabled):
                    UpdatePublisherProperty(p => p.PublishAudio = IsAudioPublishingEnabled);
                    return;
                case nameof(IsVideoSubscriptionEnabled):
                    UpdateSubscriberProperty(s => s.SubscribeToVideo = IsVideoSubscriptionEnabled);
                    return;
                case nameof(IsAudioSubscriptionEnabled):
                    UpdateSubscriberProperty(s => s.SubscribeToAudio = IsAudioSubscriptionEnabled);
                    return;
            }
        }

        private void UpdatePublisherProperty(Action<OTPublisher> updateAction)
        {
            if (PublisherKit == null)
            {
                return;
            }
            updateAction?.Invoke(PublisherKit);
        }

        private void UpdateSubscriberProperty(Action<OTSubscriber> updateAction)
        {
            foreach (var subscriberKit in _subscribers)
            {
                updateAction?.Invoke(subscriberKit);
            }
        }

        public override void CycleCamera()
        {
            if (PublisherKit == null)
            {
                return;
            }

            PublisherKit.CameraPosition = PublisherKit.CameraPosition == AVCaptureDevicePosition.Front
                ? AVCaptureDevicePosition.Back
                : AVCaptureDevicePosition.Front;
        }

        private void OnConnectionDestroyed(object sender, OTSessionDelegateConnectionEventArgs e)
            => RaiseSubscriberUpdated();

        private void OnDidConnect(object sender, EventArgs e)
        {
            if (Session == null || PublisherKit != null)
            {
                return;
            }

            PublisherKit = new OTPublisher(null, new OTPublisherSettings
            {
                Name = "XamarinOpenTok",
                CameraFrameRate = _frameRate,
                CameraResolution = _resolution
            })
            {
                PublishVideo = IsVideoPublishingEnabled,
                PublishAudio = IsAudioPublishingEnabled
            };
            PublisherKit.StreamCreated += OnPublisherStreamCreated;

            Session.Publish(PublisherKit);
            RaisePublisherUpdated();
        }

        private void OnStreamCreated(object sender, OTSessionDelegateStreamEventArgs e)
        {
            if (Session == null || _subscribers.Any(x => x.Stream?.StreamId == e.Stream?.StreamId))
            {
                return;
            }

            var subscriberKit = new OTSubscriber(e.Stream, null)
            {
                SubscribeToVideo = IsVideoSubscriptionEnabled,
                SubscribeToAudio = IsAudioSubscriptionEnabled
            };
            subscriberKit.DidConnectToStream += OnSubscriberConnected;
            subscriberKit.DidDisconnectFromStream += OnSubscriberDisconnected;
            subscriberKit.VideoDataReceived += OnSubscriberVideoDataReceived;
            subscriberKit.VideoEnabled += OnSubscriberVideoEnabled;
            subscriberKit.VideoDisabled += OnSubscriberVideoDisabled;

            Session.Subscribe(subscriberKit);
            var streamId = e.Stream.StreamId;
            _subscribers.Add(subscriberKit);
            _subscriberStreamIds.Add(streamId);
        }

        private void OnStreamDestroyed(object sender, OTSessionDelegateStreamEventArgs e)
        {
            var streamId = e.Stream.StreamId;
            var subscriberKit = _subscribers.FirstOrDefault(x => x.Stream?.StreamId == streamId);
            if (subscriberKit != null)
            {
                ClearSubscriber(subscriberKit);
                _subscribers.Remove(subscriberKit);
            }
            _subscriberStreamIds.Remove(streamId);
            RaiseSubscriberUpdated();
        }

        private void OnError(object sender, OTSessionDelegateErrorEventArgs e)
        {
            RaiseError(e.Error?.Code.ToString(CultureInfo.CurrentUICulture));
            EndSession();
        }

        private void OnSubscriberVideoDisabled(object sender, OTSubscriberKitDelegateVideoEventReasonEventArgs e)
            => RaiseSubscriberUpdated();

        private void OnSubscriberVideoDataReceived(object sender, EventArgs e)
            => RaiseSubscriberUpdated();

        private void OnSubscriberVideoEnabled(object sender, OTSubscriberKitDelegateVideoEventReasonEventArgs e)
            => RaiseSubscriberUpdated();

        private void OnSubscriberConnected(object sender, EventArgs e)
            => RaisePublisherUpdated().RaiseSubscriberUpdated();

        private void OnSubscriberDisconnected(object sender, EventArgs e)
            => RaisePublisherUpdated().RaiseSubscriberUpdated();

        private PlatformOpenTokService RaiseSubscriberUpdated()
        {
            SubscriberUpdated?.Invoke();
            return this;
        }

        private PlatformOpenTokService RaisePublisherUpdated()
        {
            PublisherUpdated?.Invoke();
            return this;
        }

        private void OnPublisherStreamCreated(object sender, OTPublisherDelegateStreamEventArgs e)
            => IsPublishingStarted = true;

        private void OnSignalReceived(object sender, OTSessionDelegateSignalEventArgs e)
            => RaiseMessageReceived(e.StringData);

        private void ClearSubscriber(OTSubscriber subscriberKit)
        {
            using (subscriberKit)
            {
                subscriberKit.SubscribeToAudio = false;
                subscriberKit.SubscribeToVideo = false;
                subscriberKit.DidConnectToStream -= OnSubscriberConnected;
                subscriberKit.DidDisconnectFromStream -= OnSubscriberDisconnected;
                subscriberKit.VideoDataReceived -= OnSubscriberVideoDataReceived;
                subscriberKit.VideoEnabled -= OnSubscriberVideoEnabled;
                subscriberKit.VideoDisabled -= OnSubscriberVideoDisabled;
            }
        }
    }
}
