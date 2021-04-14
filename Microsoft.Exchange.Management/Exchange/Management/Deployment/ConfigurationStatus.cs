using System;

namespace Microsoft.Exchange.Management.Deployment
{
	internal struct ConfigurationStatus
	{
		public ConfigurationStatus(string role)
		{
			this.Role = role;
			this.Action = InstallationModes.Unknown;
			this.Watermark = null;
		}

		public ConfigurationStatus(string role, InstallationModes action)
		{
			this.Role = role;
			this.Action = action;
			this.Watermark = null;
		}

		public string Role;

		public InstallationModes Action;

		public string Watermark;
	}
}
