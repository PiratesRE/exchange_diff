using System;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal abstract class RusDataHandler
	{
		public RusDataHandler(string configurationDomainController, string recipientDomainController, string globalCatalog, NetworkCredential credential, PartitionId partitionId, ProvisioningCache provisioningCache, LogMessageDelegate logger)
		{
			this.configurationDomainController = configurationDomainController;
			this.recipientDomainController = recipientDomainController;
			this.globalCatalog = globalCatalog;
			this.credential = credential;
			this.partitionId = partitionId;
			this.provisioningCache = provisioningCache;
			this.logger = logger;
			this.CurrentOrganizationId = OrganizationId.ForestWideOrgId;
		}

		protected OrganizationId CurrentOrganizationId { get; set; }

		public string ConfigurationDomainController
		{
			get
			{
				return this.configurationDomainController;
			}
		}

		public string RecipientDomainController
		{
			get
			{
				return this.recipientDomainController;
			}
		}

		public string GlobalCatalog
		{
			get
			{
				return this.globalCatalog;
			}
		}

		public NetworkCredential Credential
		{
			get
			{
				return this.credential;
			}
		}

		public ADObjectId RootOrgContainerId
		{
			get
			{
				if (this.rootOrgContainerId == null)
				{
					if (this.credential != null)
					{
						this.rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(this.configurationDomainController, this.credential);
					}
					else if (this.partitionId != null)
					{
						this.rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(this.partitionId.ForestFQDN, this.configurationDomainController, null);
					}
					else
					{
						this.rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
					}
				}
				return this.rootOrgContainerId;
			}
		}

		public ADSessionSettings SessionSettings
		{
			get
			{
				if (this.sessionSettings == null || !this.sessionSettings.CurrentOrganizationId.Equals(this.CurrentOrganizationId))
				{
					this.sessionSettings = this.GetLocalSessionSettings(this.CurrentOrganizationId);
				}
				return this.sessionSettings;
			}
		}

		public ADSessionSettings GetLocalSessionSettings(OrganizationId organizationId)
		{
			if (this.partitionId != null)
			{
				return ADSessionSettings.FromAccountPartitionRootOrgScopeSet(this.partitionId);
			}
			return ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.RootOrgContainerId, organizationId, null, false);
		}

		public IConfigurationSession ConfigurationSession
		{
			get
			{
				if (this.configurationSession == null || this.ShouldChangeScope(this.configurationSession))
				{
					this.configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.configurationDomainController, true, ConsistencyMode.PartiallyConsistent, this.credential, this.SessionSettings, 194, "ConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\Rus\\RusDataHandler.cs");
				}
				return this.configurationSession;
			}
		}

		public ITopologyConfigurationSession RootOrgConfigurationSession
		{
			get
			{
				if (this.rootOrgConfigurationSession == null)
				{
					ADSessionSettings adsessionSettings = (this.partitionId == null) ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromAccountPartitionRootOrgScopeSet(this.partitionId);
					bool flag = TaskHelper.ShouldPassDomainControllerToSession(this.configurationDomainController, adsessionSettings);
					this.rootOrgConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(flag ? this.configurationDomainController : null, true, ConsistencyMode.PartiallyConsistent, flag ? this.credential : null, adsessionSettings, 216, "RootOrgConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\Rus\\RusDataHandler.cs");
				}
				return this.rootOrgConfigurationSession;
			}
		}

		public IRecipientSession RecipientSession
		{
			get
			{
				string domainController = this.recipientDomainController;
				if (this.recipientSession != null)
				{
					domainController = this.recipientSession.DomainController;
				}
				if (this.recipientSession == null || this.ShouldChangeScope(this.recipientSession))
				{
					this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, false, ConsistencyMode.PartiallyConsistent, this.credential, this.SessionSettings, 243, "RecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\Rus\\RusDataHandler.cs");
				}
				return this.recipientSession;
			}
		}

		public IRecipientSession GlobalCatalogSession
		{
			get
			{
				if (this.globalCatalogSession == null)
				{
					this.globalCatalogSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.globalCatalog, true, ConsistencyMode.PartiallyConsistent, this.credential, (this.partitionId == null) ? ADSessionSettings.FromRootOrgScopeSet() : ADSessionSettings.FromAccountPartitionRootOrgScopeSet(this.partitionId), 263, "GlobalCatalogSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\Rus\\RusDataHandler.cs");
					this.globalCatalogSession.UseGlobalCatalog = true;
				}
				return this.globalCatalogSession;
			}
		}

		public IRecipientSession GetTenantLocalRecipientSession(OrganizationId organizationId)
		{
			ADSessionSettings localSessionSettings = this.GetLocalSessionSettings(organizationId);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.recipientDomainController, false, ConsistencyMode.PartiallyConsistent, this.credential, localSessionSettings, 282, "GetTenantLocalRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\Rus\\RusDataHandler.cs");
		}

		public IRecipientSession GetTenantLocalGlobalCatalogSession(OrganizationId organizationId)
		{
			ADSessionSettings localSessionSettings = this.GetLocalSessionSettings(organizationId);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.globalCatalog, true, ConsistencyMode.PartiallyConsistent, this.credential, localSessionSettings, 299, "GetTenantLocalGlobalCatalogSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\Rus\\RusDataHandler.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			return tenantOrRootOrgRecipientSession;
		}

		public ProvisioningCache ProvisioningCache
		{
			get
			{
				return this.provisioningCache;
			}
		}

		public LogMessageDelegate Logger
		{
			get
			{
				return this.logger;
			}
		}

		private bool ShouldChangeScope(IDirectorySession session)
		{
			return this.CurrentOrganizationId != null && session.SessionSettings.CurrentOrganizationId != null && !this.CurrentOrganizationId.Equals(session.SessionSettings.CurrentOrganizationId);
		}

		private string configurationDomainController;

		private IConfigurationSession configurationSession;

		private ITopologyConfigurationSession rootOrgConfigurationSession;

		private string recipientDomainController;

		private IRecipientSession recipientSession;

		private string globalCatalog;

		private IRecipientSession globalCatalogSession;

		private NetworkCredential credential;

		private PartitionId partitionId;

		private ProvisioningCache provisioningCache;

		private ADSessionSettings sessionSettings;

		private ADObjectId rootOrgContainerId;

		private LogMessageDelegate logger;
	}
}
