using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.OpenTok;
using Xamarin.Forms.OpenTok.Service;
using Xamarin.Forms.Xaml;

namespace OpenTok
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatRoomPage : ContentPage
    {
        public ChatRoomPage()
        {
            InitializeComponent();
            CrossOpenTok.Current.MessageReceived += OnMessageReceived;

            Wrapper.Children.Insert(0, new OpenTokPublisherView()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = 100,
                HeightRequest = 100
            });

            DataTemplate dataTemplate = new DataTemplate(() =>
            {
                OpenTokSubscriberView reply = new OpenTokSubscriberView();
                reply.SetBinding(OpenTokSubscriberView.StreamIdProperty, ".");
                reply.WidthRequest = 400;
                reply.HeightRequest = 400;
                return reply;
            });

            StackLayout slSubscribers = new StackLayout();
            BindableLayout.SetItemsSource(slSubscribers, CrossOpenTok.Current.StreamIdCollection);
            BindableLayout.SetItemTemplate(slSubscribers, dataTemplate);
            slSubscribers.BackgroundColor = Color.Blue;

            Wrapper.Children.Insert(1,slSubscribers);
        }

        private void OnEndCall(object sender, EventArgs e)
        {
            CrossOpenTok.Current.EndSession();
            CrossOpenTok.Current.MessageReceived -= OnMessageReceived;
            Navigation.PopAsync();
        }

        private void OnMute(object sender, EventArgs e)
        {
            //CrossOpenTok.Current.SendMessageAsync($"Path.GetRandomFileName: {Path.GetRandomFileName()}");
            CrossOpenTok.Current.IsAudioPublishingEnabled = !CrossOpenTok.Current.IsAudioPublishingEnabled;
        }

        private void OnSwapCamera(object sender, EventArgs e)
        {
            CrossOpenTok.Current.CycleCamera();
        }

        private void OnMessageReceived(string message)
        {
            DisplayAlert("Random message received", message, "OK");
        }
    }
}