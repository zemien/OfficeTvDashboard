using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace OfficeDashboard
{
    /// <summary>
    /// Service for TeamCity
    /// </summary>
    public class TeamCityService
    {
        private HttpClient mWebServiceCaller;

        public string Url { get; set; }

        public TeamCityService(string url)
        {
            Url = url;
            HttpClientHandler myHandler = new HttpClientHandler();
            myHandler.AllowAutoRedirect = false;
            mWebServiceCaller = new HttpClient(myHandler);
            mWebServiceCaller.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            mWebServiceCaller.BaseAddress = new Uri($"{Url}/guestAuth/app/rest/");
        }

        /// <summary>
        /// Gets the 100 most recent builds.
        /// </summary>
        public async Task<List<TeamCityBuild>> GetMostRecentBuilds()
        {
            var buildString = await mWebServiceCaller.GetStringAsync("builds/");
            var buildJson = JObject.Parse(buildString);
            
            var builds = ((JArray)buildJson["build"]).ToObject<List<TeamCityBuild>>();
            return builds;
        }

        /// <summary>
        /// Gets the most recent build of each unique build type, excluding specified build type IDs 
        /// </summary>
        /// <returns></returns>
        public async Task<List<TeamCityBuild>> GetUniqueProjectsMostRecentBuilds(List<string> excludeBuildProjects)
        {
            var exclusions = excludeBuildProjects ?? new List<string>();
            var recentBuilds = await GetMostRecentBuilds();

            return (recentBuilds.Where(element => !exclusions.Contains(element.BuildProject))
                .OrderByDescending(element => element.Id)
                .GroupBy(element => element.BuildTypeId)
                .Select(groups => groups.First()).OrderByDescending(element => element.Id)).ToList();
                   
        }
    }
}
