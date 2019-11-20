using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
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
        }

        private void OnEndCall(object sender, EventArgs e)
        {
            CrossOpenTok.Current.EndSession();
            CrossOpenTok.Current.MessageReceived -= OnMessageReceived;
            Navigation.PopAsync();
        }

        private void OnMessage(object sender, EventArgs e)
        {
            CrossOpenTok.Current.SendMessageAsync($"Path.GetRandomFileName: {Path.GetRandomFileName()}");
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