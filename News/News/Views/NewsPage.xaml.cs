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
                    groupedList.ItemsSource = t1.Result.Articles.Skip(1);
                    firstPageUrl.Source = t1.Result.Articles[0].UrlToImage;
                    //firstPageDescription.Text = t1.Result.Articles[0].Description;
                    firstPageDateTime.Text = t1.Result.Articles[0].DateTime.ToString("hh:mm - d");
                    firstPageAuthor.Text = t1.Result.Articles[0].Author;
                    firstPageTitle.Text = t1.Result.Articles[0].Title;
                });
            });
        }

        private async void groupedList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            NewsItem item = (NewsItem)e.Item;

            await Navigation.PushAsync(new ArticleView(item.Url));
        }


        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Which Option would you like to select?", "Cancel", null, "Slow Internet", "Throw an error");

            if (optionsButton.IsPressed)
                ImageButton_Clicked(sender, e);

            if (action != null)
                await LoadNews();

            if (action.Contains("Throw an error"))
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception)
                {
                    await DisplayAlert("Error!", "You're not allowed to do that!", "Cancel");
                }
            }
            if (action.Contains("Slow Internet"))
                SlowInternetPressed();
        }

        private async void SlowInternetPressed()
        {
            slowedInternet.IsVisible = true;
            slowedInternet.IsRunning = true;
            await Task.Delay(4000);
            await LoadNews();
            slowedInternet.IsRunning = false;
        }

        private async void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            refreshProgress.Progress = 0;
            refreshProgress.IsVisible = true;
            await refreshProgress.ProgressTo(1, 2000, Easing.Linear);
            refreshProgress.IsVisible = false;  
            await LoadNews();
        }

    }
}