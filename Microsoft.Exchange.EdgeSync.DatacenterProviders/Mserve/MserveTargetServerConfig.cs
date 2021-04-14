using System;
using Microsoft.Exchange.EdgeSync.Datacenter;

namespace Microsoft.Exchange.EdgeSync.Mserve
{
	internal sealed class MserveTargetServerConfig : DatacenterTargetServerConfig
	{
		public MserveTargetServerConfig(string name, string provisioningUrl, string settingsUrl, string remoteCertSubject, string primaryLeaseLocation, string backupLeaseLocation) : base(name, provisioningUrl, primaryLeaseLocation, backupLeaseLocation)
		{
			this.settingsUrl = settingsUrl;
			this.remoteCertSubject = remoteCertSubject;
		}

		public string SettingsUrl
		{
			get
			{
				return this.settingsUrl;
			}
		}

		public string RemoteCertSubject
		{
			get
			{
				return this.remoteCertSubject;
			}
		}

		public override string ShortHostName
		{
			get
			{
				return "MSERV";
			}
		}

		private const string ShortHostNameValue = "MSERV";

		private readonly string settingsUrl;

		private readonly string remoteCertSubject;
	}
}
