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
                            if(!CrossOpenTok.Current.TryStartSession())
                            {
                                return;
                            }
                            Navigation.PushAsync(new ChatRoomPage());
                        })
                    }
                }
            };
        }
    }
}
