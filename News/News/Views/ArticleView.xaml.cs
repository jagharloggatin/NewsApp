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
 
    }
}
