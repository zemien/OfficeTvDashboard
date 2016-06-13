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
            mWebServiceCaller.BaseAddress = new Uri(Url);
        }

        /// <summary>
        /// Gets the 100 most recent builds.
        /// </summary>
        public async Task<List<TeamCityBuild>> GetMostRecentBuilds()
        {
            var buildString = await mWebServiceCaller.GetStringAsync("/guestAuth/app/rest/builds/");
            var buildJson = JObject.Parse(buildString);
            
            var builds = ((JArray)buildJson["build"]).ToObject<List<TeamCityBuild>>();
            return builds;
        }

        /// <summary>
        /// Gets the most recent build of each unique build type, excluding specified build type IDs
        /// </summary>
        /// <param name="excludeBuildTypeIds">The exclude build type ids.</param>
        /// <returns></returns>
        public async Task<List<TeamCityBuild>> GetUniqueProjectsMostRecentBuilds(string excludeBuildTypeIds)
        {
            var exclusion = excludeBuildTypeIds ?? string.Empty;
            var recentBuilds = await GetMostRecentBuilds();

            var mostRecentBuilds = (recentBuilds.Where(element => !element.BuildTypeId.Contains(exclusion))
                .OrderByDescending(element => element.Id)
                .GroupBy(element => element.BuildTypeId)
                .Select(groups => groups.First()).OrderByDescending(element => element.Id)).ToList();

            await FillBuildDetails(mostRecentBuilds);

            return mostRecentBuilds;

        }

        /// <summary>
        /// Fills the build details (Project name and status text).
        /// </summary>
        /// <param name="builds">The builds.</param>
        public async Task FillBuildDetails(List<TeamCityBuild> builds)
        {
            foreach (var build in builds)
            {
                if (string.IsNullOrWhiteSpace(build.Href))
                    continue;

                var buildDetails = await mWebServiceCaller.GetStringAsync(build.Href);
                var buildDetailsJson = JObject.Parse(buildDetails);

                if (buildDetailsJson == null)
                {
                    continue;
                }

                build.ProjectName = (string)buildDetailsJson["buildType"]?["name"];
                build.StatusText = (string) buildDetailsJson["statusText"];
                build.LastChangedBy = (string) buildDetailsJson["lastChanges"]?["change"]?[0]?["username"];
            }
        }
    }
}
