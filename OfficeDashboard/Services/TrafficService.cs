using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDashboard
{
    public class TrafficService
    {
        public string ApiKey { get; set; }

        public string Url { get; set; }

        public TrafficService(string url, string apiKey)
        {
            Url = url;
            ApiKey = apiKey;
        }

        //https://www.nzta.govt.nz/traffic-and-travel-information/infoconnect-section-page/about-the-apis/auckland-traffic-api/

    }
}
