using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using InfoBot.MessageGenerators.HackerNewsMessageGenerator.Models;

namespace InfoBot.MessageGenerators.HackerNewsMessageGenerator
{
    public class HackerNewsMessageGenerator : IMessageGenerator
    {
        private const string TopStoriesUrl = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";
        private const string GetItemUrl = "https://hacker-news.firebaseio.com/v0/item/{0}.json?print=pretty";

        public async Task<string> GetMessageAsync(string command)
        {
            var ids = await GetTopPostsIdsAsync();

            var result = new StringBuilder();

            var counter = 0;
            foreach(var id in ids)
            {
                var item = await GetItem(id);
                if(item.type == ItemType.story)
                {
                    counter++;
                    result.Append($"{counter}. {item.title} {item.url}");

                    if(counter == 10)
                    {
                        break;
                    }
                }
            }

            return result.ToString();
        }

        private async Task<List<int>> GetTopPostsIdsAsync()
        {
            var request = WebRequest.Create(TopStoriesUrl);
            request.Method = "GET";
            var content = await Helpers.GetResponseAsync(request);

            return JsonConvert.DeserializeObject<List<int>>(content);
        }

        private async Task<Item> GetItem(int id)
        {
            var url = string.Format(GetItemUrl, id);
            var request = WebRequest.Create(url);
            request.Method = "GET";

            var response = await Helpers.GetResponseAsync(request);

            return JsonConvert.DeserializeObject<Item>(response);
        }
    }
}
