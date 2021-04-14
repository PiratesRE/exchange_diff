using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class RusDataProviderBase<T>
	{
		protected RusDataProviderBase(IConfigurationSession session, IConfigurationSession rootOrgSession, ProvisioningCache provisioningCache)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (rootOrgSession == null)
			{
				throw new ArgumentNullException("rootOrgSession");
			}
			if (provisioningCache == null)
			{
				throw new ArgumentNullException("provisioningCache");
			}
			this.configurationSession = session;
			this.rootOrgConfigurationSession = rootOrgSession;
			this.provisioningCache = provisioningCache;
		}

		protected IConfigurationSession ConfigurationSession
		{
			get
			{
				return this.configurationSession;
			}
		}

		protected IConfigurationSession RootOrgConfigurationSession
		{
			get
			{
				return this.rootOrgConfigurationSession;
			}
		}

		protected ProvisioningCache ProvisioningCache
		{
			get
			{
				return this.provisioningCache;
			}
		}

		public string DomainController
		{
			get
			{
				string text = this.ConfigurationSession.DomainController;
				if (string.IsNullOrEmpty(text))
				{
					string accountOrResourceForestFqdn = this.ConfigurationSession.SessionSettings.GetAccountOrResourceForestFqdn();
					text = this.ConfigurationSession.SessionSettings.ServerSettings.ConfigurationDomainController(accountOrResourceForestFqdn);
					if (string.IsNullOrEmpty(text))
					{
						text = ADSession.GetCurrentConfigDC(accountOrResourceForestFqdn);
					}
				}
				return text;
			}
		}

		public ADObjectId OrgContainerId
		{
			get
			{
				if (this.orgContainerId == null)
				{
					this.orgContainerId = this.rootOrgConfigurationSession.GetOrgContainerId();
				}
				return this.orgContainerId;
			}
		}

		public ReadOnlyCollection<T> GetPolicies(OrganizationId organizationId, LogMessageDelegate logger)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			PolicyContainer<T> policyContainer = new PolicyContainer<T>();
			policyContainer.OrganizationId = organizationId;
			this.LoadPolicies(policyContainer, logger);
			return policyContainer.Policies.AsReadOnly();
		}

		protected virtual void LoadPolicies(PolicyContainer<T> container, LogMessageDelegate logger)
		{
		}

		private IConfigurationSession configurationSession;

		private IConfigurationSession rootOrgConfigurationSession;

		private ADObjectId orgContainerId;

		private ProvisioningCache provisioningCache;
	}
}
