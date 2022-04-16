#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using News.Models;
using News.ModelsSampleData;
using System.Net.Http.Json;

namespace News.Services
{
    public class NewsService
    {
        ConcurrentDictionary<(string, NewsCategory), NewsGroup> _cacheCategory = new ConcurrentDictionary<(string, NewsCategory), NewsGroup>();
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "d318329c40734776a014f9d9513e14ae";
        public async Task<NewsGroup> GetNewsAsync(NewsCategory category)
        {
            string keyDate = DateTime.Now.ToString("yyyy-MM-dd:HH:mm");
            NewsCategory keyCategory = category;
            var key = (keyDate, keyCategory);

            if (!_cacheCategory.TryGetValue(key, out var news))
            {
                news = new NewsGroup();
                news = await ReadNewsAsync(category);
                _cacheCategory[key] = news;
            }
            return news;
        }

        public async Task<NewsGroup> ReadNewsAsync(NewsCategory category)
        {

            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";

            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();

            //NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);
            NewsGroup news = new NewsGroup()
            {
                Category = category
            };

            news.Articles = nd.Articles.Select(x => new NewsItem
            {
                Title = x.Title,
                Description = x.Description,
                Url = x.Url,
                DateTime = x.PublishedAt,
                UrlToImage = x.UrlToImage,
                Author = x.Author

            }).ToList();

            return news;

            //Here is where you lift in your Service code from Part A
            /*
                    public async Task<NewsGroup> GetNewsAsync(NewsCategory category)
                    {

            #if UseNewsApiSample      
                        NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category);

            #else
                        //https://newsapi.org/docs/endpoints/top-headlines
                        var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}&apiKey={apiKey}";


                        //Recommend to use Newtonsoft Json Deserializer as it works best with Android
                        var webclient = new WebClient();
                        var json = await webclient.DownloadStringTaskAsync(uri);
                        NewsApiData nd = Newtonsoft.Json.JsonConvert.DeserializeObject<NewsApiData>(json);

            #endif
            */
        }
    }
}
