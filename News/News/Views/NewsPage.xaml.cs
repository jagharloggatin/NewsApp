using News.Models;
using News.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace News.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        NewsService service;
        public NewsPage()
        {
            InitializeComponent();
            service = new NewsService();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MainThread.BeginInvokeOnMainThread(async () => { await LoadNews(); });

        }

        private async Task LoadNews()
        {
            NewsCategory selectedCategory = (NewsCategory)Enum.Parse(typeof(NewsCategory), Title.ToLower());

            await Task.Run(() =>
            {
                Task<NewsGroup> t1 = service.GetNewsAsync(selectedCategory);
                Device.BeginInvokeOnMainThread(() =>
                {
                    groupedList.ItemsSource = t1.Result.Articles;
                    pageInfo.Text = $"{Title}";
                });
            });
        }

        private async void groupedList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            NewsItem item = (NewsItem)e.Item;

            await Navigation.PushAsync(new ArticleView(item.Url));
        }
    }
}