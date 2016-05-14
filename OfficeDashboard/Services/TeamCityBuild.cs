namespace OfficeDashboard
{
    public class TeamCityBuild
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the build type identifier.
        /// </summary>
        /// <value>
        /// The build type identifier.
        /// </value>
        public string BuildTypeId { get; set; }

        /// <summary>
        /// Gets the build project.
        /// </summary>
        /// <value>
        /// The build project.
        /// </value>
        public string BuildProject => BuildTypeId?.Split('_')[0];

        /// <summary>
        /// Gets the name of the build.
        /// </summary>
        /// <value>
        /// The name of the build.
        /// </value>
        public string BuildName => BuildTypeId?.Split('_')[1];

        /// <summary>
        /// Gets or sets the build number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State { get; set; }
    }
}
