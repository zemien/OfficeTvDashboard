using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OfficeDashboard
{
    public class DailyQuotesService
    {
        private readonly string mDailyQuotesUrl;

        public DailyQuotesService(string dailyQuotesUrl)
        {
            mDailyQuotesUrl = dailyQuotesUrl;
        }

        public async Task<Tuple<string, string>> GetDailyQuote()
        {
            var url = $"{mDailyQuotesUrl}/qod.json";

            HttpClientHandler myHandler = new HttpClientHandler();
            myHandler.AllowAutoRedirect = false;
            HttpClient myClient = new HttpClient(myHandler);
            var quote = JObject.Parse(await myClient.GetStringAsync(url));

            if (quote != null)
            {
                return new Tuple<string, string>(quote["contents"]["quotes"][0]["quote"].ToString(),
                    quote["contents"]["quotes"][0]["author"].ToString());
            }

            return null;
        }
    }
}
