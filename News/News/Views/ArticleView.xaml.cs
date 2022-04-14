using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;
using News.Services;
using News.Models;
using Xamarin.Essentials;
using System;

namespace News.Views
{
    public partial class ArticleView : ContentPage
    {
        //Here is where you show the news in Full page

        public ArticleView()
        {
            InitializeComponent();
        }
        public ArticleView(string Url)
        {
            InitializeComponent();
            BindingContext = new UrlWebViewSource
            {
                Url = HttpUtility.UrlDecode(Url)
            };
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await progressBar.ProgressTo(0.9, 900, Easing.SpringIn);
        }

        private void webView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            progressBar.IsVisible = true;
        }

        private void webView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            progressBar.IsVisible = false;
        }

        private async void optionsButton2_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PopToRootAsync();
        }
    }
}
