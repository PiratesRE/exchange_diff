using System;

namespace Microsoft.Exchange.EdgeSync.Datacenter
{
	internal class DatacenterTargetServerConfig : TargetServerConfig
	{
		protected DatacenterTargetServerConfig(string name, string provisioningUrl, string primaryLeaseLocation, string backupLeaseLocation) : base(name, provisioningUrl, 0)
		{
			this.primaryLeaseLocation = primaryLeaseLocation;
			this.backupLeaseLocation = backupLeaseLocation;
		}

		public string ProvisioningUrl
		{
			get
			{
				return base.Host;
			}
		}

		public string PrimaryLeaseLocation
		{
			get
			{
				return this.primaryLeaseLocation;
			}
		}

		public string BackupLeaseLocation
		{
			get
			{
				return this.backupLeaseLocation;
			}
		}

		private readonly string primaryLeaseLocation;

		private readonly string backupLeaseLocation;
	}
}
