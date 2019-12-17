using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.OpenTok.Service;

namespace OpenTok
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Content = new StackLayout
            {
                Children =
                {
                    new Button
                    {
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Text = "CLICK TO CHAT",
                        Command = new Command(() => {
                            if(!CrossOpenTok.Current.TryStartSession(Enums.CameraCaptureResolution.High, Enums.CameraCaptureFrameRate.Fps30))
                            {
                                return;
                            }
                            Navigation.PushAsync(new ChatRoomPage());
                        })
                    },
                    new Button
                    {
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Text = "TEST POPUP",
                        Command = new Command(async () => {
                            if(!CrossOpenTok.Current.TryStartSession(Enums.CameraCaptureResolution.High, Enums.CameraCaptureFrameRate.Fps30))
                            {
                                return;
                            }
                            var page = new IncomingCallPage();
                            await PopupNavigation.Instance.PushAsync(page);
                        })
                    }
                }
            };
        }
    }
}
