using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OpenTok
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingCallPage : PopupPage
    {
        public IncomingCallPage()
        {
            InitializeComponent();
            BackgroundColor = Color.DarkGreen;
        }

        private async void OnClose(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}