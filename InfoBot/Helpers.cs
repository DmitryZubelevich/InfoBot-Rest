using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InfoBot
{
    public class Helpers
    {
        public static async Task<string> GetResponseAsync(WebRequest request)
        {
            var wr = await request.GetResponseAsync();
            var receiveStream = wr.GetResponseStream();
            var reader = new StreamReader(receiveStream, Encoding.UTF8);
            var content = reader.ReadToEnd();
            return content;
        }
    }
}
