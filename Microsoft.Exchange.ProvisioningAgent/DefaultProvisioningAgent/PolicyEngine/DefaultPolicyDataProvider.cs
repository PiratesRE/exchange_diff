using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.DefaultProvisioningAgent.PolicyEngine
{
	[ImmutableObject(true)]
	internal class DefaultPolicyDataProvider : IPolicyDataProvider
	{
		public DefaultPolicyDataProvider(IConfigurationSession scSession)
		{
			if (scSession == null)
			{
				throw new ArgumentNullException("scSession");
			}
			this.scSession = scSession;
			this.scSession.SessionSettings.IsSharedConfigChecked = true;
		}

		public IEnumerable<ADProvisioningPolicy> GetEffectiveProvisioningPolicy(OrganizationId organizationId, Type poType, ProvisioningPolicyType policyType, int maxResults, ProvisioningCache provisioningCache)
		{
			if (null == poType)
			{
				throw new ArgumentNullException("poType");
			}
			if ((policyType & ~(ProvisioningPolicyType.Template | ProvisioningPolicyType.Enforcement)) != (ProvisioningPolicyType)0)
			{
				throw new ArgumentOutOfRangeException("policyType");
			}
			if (!PolicyConfiguration.ObjectType2PolicyEntryDictionary.ContainsKey(poType))
			{
				return null;
			}
			if (policyType == ProvisioningPolicyType.Template)
			{
				PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy> policyConfigurationEntry = PolicyConfiguration.ObjectType2PolicyEntryDictionary[poType];
				if (policyConfigurationEntry == null)
				{
					return null;
				}
				return policyConfigurationEntry.GetTemplateProvisioningPolicy(this.ConfigurationSession, organizationId, poType, policyType, maxResults, provisioningCache);
			}
			else
			{
				if (policyType != ProvisioningPolicyType.Enforcement)
				{
					throw new ArgumentOutOfRangeException("policyType");
				}
				PolicyConfigurationEntry<RecipientTemplateProvisioningPolicy, RecipientEnforcementProvisioningPolicy> policyConfigEntry = PolicyConfiguration.ObjectType2PolicyEntryDictionary[poType];
				if (policyConfigEntry == null)
				{
					return null;
				}
				Guid enforcementProvisioningPolicies = CannedProvisioningCacheKeys.EnforcementProvisioningPolicies;
				return provisioningCache.TryAddAndGetOrganizationDictionaryValue<IEnumerable<ADProvisioningPolicy>, string>(enforcementProvisioningPolicies, organizationId, poType.Name, () => policyConfigEntry.GetEnforcementProvisioningPolicy(this.ConfigurationSession, organizationId, poType, policyType, maxResults, provisioningCache));
			}
		}

		public IConfigurationSession ConfigurationSession
		{
			get
			{
				return this.scSession;
			}
		}

		public string Source
		{
			get
			{
				return this.scSession.DomainController;
			}
		}

		private readonly IConfigurationSession scSession;
	}
}
