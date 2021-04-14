using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ManagementEndpoint
{
	public abstract class ManagementEndpointBase : Task
	{
		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public GlobalDirectoryServiceType GlobalDirectoryService { get; set; }

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception) || exception is RedirectionEntryManagerException || exception is MonadDataAdapterInvocationException;
		}

		private IGlobalDirectorySession GlobalDirectorySession(string redirectFormatForMServ = null)
		{
			switch (this.GlobalDirectoryService)
			{
			case GlobalDirectoryServiceType.Default:
				return DirectorySessionFactory.GetGlobalSession(redirectFormatForMServ);
			case GlobalDirectoryServiceType.MServ:
				return new MServDirectorySession(redirectFormatForMServ);
			case GlobalDirectoryServiceType.Gls:
				return new GlsDirectorySession();
			default:
				throw new ArgumentException("GlobalDirectoryService");
			}
		}

		protected virtual string GetRedirectionTemplate()
		{
			return null;
		}

		internal abstract void ProcessRedirectionEntry(IGlobalDirectorySession session);

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (!ManagementEndpointBase.IsGlobalDirectoryConfigured())
			{
				base.WriteWarning("Management endpoint code skipped in test environment.");
				return;
			}
			IGlobalDirectorySession session = this.GlobalDirectorySession(this.GetRedirectionTemplate());
			this.ProcessRedirectionEntry(session);
		}

		public static bool IsGlobalDirectoryConfigured()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 111, "IsGlobalDirectoryConfigured", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ManagementEndpoint\\ManagementEndpointBase.cs");
			ADSite localSite = topologyConfigurationSession.GetLocalSite();
			return localSite.PartnerId != -1 || !localSite.DistinguishedName.EndsWith("DC=extest,DC=microsoft,DC=com");
		}

		internal static string GetSmtpNextHopDomain()
		{
			if (string.IsNullOrEmpty(ManagementEndpointBase.smtpNextHopFormat))
			{
				ManagementEndpointBase.smtpNextHopFormat = RegistrySettings.ExchangeServerCurrentVersion.SmtpNextHopDomainFormat;
			}
			return string.Format(ManagementEndpointBase.smtpNextHopFormat, ManagementEndpointBase.GetLocalSite().PartnerId);
		}

		private static ADSite GetLocalSite()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 142, "GetLocalSite", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ManagementEndpoint\\ManagementEndpointBase.cs");
			return topologyConfigurationSession.GetLocalSite();
		}

		private static string smtpNextHopFormat;
	}
}
