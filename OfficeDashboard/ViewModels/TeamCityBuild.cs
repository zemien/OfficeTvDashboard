﻿using System;

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
        /// Gets the name of the build if there is one.
        /// </summary>
        /// <value>
        /// The name of the build.
        /// </value>
        public string ProjectName { get; set; }

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
        /// Gets or sets the status text.
        /// </summary>
        /// <value>
        /// The status text.
        /// </value>
        public string StatusText { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the link to more information about this build.
        /// </summary>
        /// <value>
        /// The href.
        /// </value>
        public string Href { get; set; }

        /// <summary>
        /// Gets or sets the last changed by.
        /// </summary>
        /// <value>
        /// The last changed by.
        /// </value>
        public string LastChangedBy { get; set; }
    }
}
