using System;
using System.Security;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Datacenter;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal sealed class EhfTargetServerConfig : DatacenterTargetServerConfig
	{
		public EhfTargetServerConfig(EdgeSyncEhfConnector connector, Uri internetWebProxy) : base(connector.Name, EhfTargetServerConfig.GetProvisioningUrl(connector), connector.PrimaryLeaseLocation, connector.BackupLeaseLocation)
		{
			this.internetWebProxy = internetWebProxy;
			this.userName = null;
			this.password = null;
			if (connector.AuthenticationCredential != null)
			{
				this.userName = connector.AuthenticationCredential.UserName;
				this.password = connector.AuthenticationCredential.Password;
			}
			this.version = connector.Version;
			this.adminSyncEnabled = connector.AdminSyncEnabled;
			this.resellerId = EhfSynchronizationProvider.GetResellerId(connector);
			this.ehfSyncAppConfig = new EhfSyncAppConfig();
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		public SecureString Password
		{
			get
			{
				return this.password;
			}
		}

		public int ResellerId
		{
			get
			{
				return this.resellerId;
			}
		}

		public Uri InternetWebProxy
		{
			get
			{
				return this.internetWebProxy;
			}
		}

		public EhfWebServiceVersion EhfWebServiceVersion
		{
			get
			{
				return (EhfWebServiceVersion)this.version;
			}
		}

		public bool AdminSyncEnabled
		{
			get
			{
				return this.adminSyncEnabled;
			}
		}

		public EhfSyncAppConfig EhfSyncAppConfig
		{
			get
			{
				return this.ehfSyncAppConfig;
			}
		}

		private static string GetProvisioningUrl(EdgeSyncEhfConnector connector)
		{
			Uri provisioningUrl = connector.ProvisioningUrl;
			EhfSynchronizationProvider.ValidateProvisioningUrl(provisioningUrl, connector.AuthenticationCredential, connector.DistinguishedName);
			return provisioningUrl.AbsoluteUri;
		}

		private readonly string userName;

		private readonly SecureString password;

		private readonly int resellerId;

		private readonly Uri internetWebProxy;

		private readonly int version;

		private readonly bool adminSyncEnabled;

		private readonly EhfSyncAppConfig ehfSyncAppConfig;
	}
}
