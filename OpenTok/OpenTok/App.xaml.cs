using System;
using Xamarin.Forms;
using Xamarin.Forms.OpenTok.Service;
using Xamarin.Forms.Xaml;

namespace OpenTok
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            CrossOpenTok.Current.ApiKey = "40000000"; // OpenTok project api key, not your account
            CrossOpenTok.Current.SessionId = "2_MX40NjQ1OTA3Mn5-MTU3Mzc0OTUxNzgxMX5aRll5SkpibUhCci91eUpSd0lGMkdhZWF-UH4"; // Id of session for connecting

            if (Device.RuntimePlatform == Device.Android)
                CrossOpenTok.Current.UserToken = "T1==cGFydG5lcl9pZD00NjQ1OTA3MiZzaWc9YmQ5MGZlZThlMDYwYTJiZTZiNzcyYzI2ZDg5YjQ2ODVmY2ZiMTlkZjpzZXNzaW9uX2lkPTJfTVg0ME5qUTFPVEEzTW41LU1UVTNNemMwT1RVeE56Z3hNWDVhUmxsNVNrcGliVWhDY2k5MWVVcFNkMGxHTWtkaFpXRi1VSDQmY3JlYXRlX3RpbWU9MTU3NDA5MjIyMiZub25jZT0wLjc0OTY4MzE3NTc2NDM1NSZyb2xlPXB1Ymxpc2hlciZleHBpcmVfdGltZT0xNTc2Njg0MjIxJmluaXRpYWxfbGF5b3V0X2NsYXNzX2xpc3Q9"; // User's token
            else if (Device.RuntimePlatform == Device.iOS)
                CrossOpenTok.Current.UserToken = "T1==cGFydG5lcl9pZD00NjQ1OTA3MiZzaWc9OTEwNjljMzFmNTViYzllZTY2ZDlkZDE3YjBjZWJhNWY5MjU0YjQ0OTpzZXNzaW9uX2lkPTJfTVg0ME5qUTFPVEEzTW41LU1UVTNNemMwT1RVeE56Z3hNWDVhUmxsNVNrcGliVWhDY2k5MWVVcFNkMGxHTWtkaFpXRi1VSDQmY3JlYXRlX3RpbWU9MTU3NDA5MTk2NiZub25jZT0wLjQzMzU3OTU0OTE3NTYwMDkzJnJvbGU9cHVibGlzaGVyJmV4cGlyZV90aW1lPTE1NzY2ODM5NjQmaW5pdGlhbF9sYXlvdXRfY2xhc3NfbGlzdD0=";

            CrossOpenTok.Current.Error += (m) => MainPage.DisplayAlert("ERROR", m, "OK");
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
