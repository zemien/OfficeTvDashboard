namespace OfficeDashboard
{
    /// <summary>
    /// Settings class
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets the weather API key.
        /// </summary>
        /// <value>
        /// The weather API key.
        /// </value>
        public string WeatherApiKey { get; set; }
        /// <summary>
        /// Gets or sets the garage cam image URL.
        /// </summary>
        /// <value>
        /// The garage cam image URL.
        /// </value>
        public string GarageCamImageUrl { get; set; }
        /// <summary>
        /// Gets or sets the motorway traffic API key.
        /// </summary>
        /// <value>
        /// The motorway traffic API key.
        /// </value>
        public string MotorwayTrafficApiKey { get; set; }
        /// <summary>
        /// Gets or sets the team city server URL.
        /// </summary>
        /// <value>
        /// The team city server URL.
        /// </value>
        public string TeamCityServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the daily quotes URL.
        /// </summary>
        /// <value>
        /// The daily quotes URL.
        /// </value>
        public string DailyQuotesUrl { get; set; }
    }
}
