using System;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public abstract class ExchangeServerAccessLicenseBase
	{
		protected ExchangeServerAccessLicenseBase() : this(string.Empty)
		{
		}

		protected ExchangeServerAccessLicenseBase(string licenseName)
		{
			this.LicenseName = licenseName;
		}

		public string LicenseName { get; protected set; }
	}
}
