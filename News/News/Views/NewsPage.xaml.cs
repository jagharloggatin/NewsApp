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
        /// <summary>
        /// When overridden, allows application developers to customize behavior immediately prior to the <see cref="T:Xamarin.Forms.Page" /> becoming visible.
        /// </summary>
        /// <remarks>
        /// To be added.
        /// </remarks>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            MainThread.BeginInvokeOnMainThread(async () => { await LoadNews(); });
        }

        /// <summary>
        /// Loads the news.
        /// </summary>
        private async Task LoadNews()
        {
            NewsCategory selectedCategory = (NewsCategory)Enum.Parse(typeof(NewsCategory), Title.ToLower());

            await Task.Run(() =>
            {
                Task<NewsGroup> t1 = service.GetNewsAsync(selectedCategory);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        groupedList.ItemsSource = t1.Result.Articles.Skip(1);
                        firstPageUrl.Source = t1.Result.Articles[0].UrlToImage;
                        //firstPageDescription.Text = t1.Result.Articles[0].Description;
                        firstPageDateTime.Text = t1.Result.Articles[0].DateTime.ToString("hh:mm - d");
                        firstPageAuthor.Text = t1.Result.Articles[0].Author;
                        firstPageTitle.Text = t1.Result.Articles[0].Title;
                    }
                    catch (Exception e)
                    {
                        await DisplayAlert("Error!", e.Message, "Cancel");
                        await Navigation.PopToRootAsync();
                    }
                });
            });
        }
        /// <summary>
        /// Handles the ItemTapped event of the grouped list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ItemTappedEventArgs"/> instance containing the event data.</param>
        private async void groupedList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            NewsItem item = (NewsItem)e.Item;

            await Navigation.PushAsync(new ArticleView(item.Url));
        }
        /// <summary>
        /// Handles the event of the options menu
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.Exception"></exception>
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Which Option would you like to select?", "Cancel", null, "Slow Internet", "Throw an error");

            if (optionsButton.IsPressed)
                ImageButton_Clicked(sender, e);

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
        /// <summary>
        /// Slows the internet.
        /// </summary>
        private async void SlowInternetPressed()
        {
            slowedInternet.IsVisible = true;
            slowedInternet.IsRunning = true;
            await Task.Delay(4000);
            await LoadNews();
            slowedInternet.IsRunning = false;
        }

        /// <summary>
        /// Handles the 1 event of the ImageButton_Clicked control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            refreshProgress.Progress = 0;
            refreshProgress.IsVisible = true;
            await refreshProgress.ProgressTo(1, 2000, Easing.Linear);
            refreshProgress.IsVisible = false;  
            await LoadNews();
        }

        /// <summary>
        /// Handles the Clicked event of the firstPageUrl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void firstPageUrl_Clicked(object sender, EventArgs e)
        {

        }
    }
}