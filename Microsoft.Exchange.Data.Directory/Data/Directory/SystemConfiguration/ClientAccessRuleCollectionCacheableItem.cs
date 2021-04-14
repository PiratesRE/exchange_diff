using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ClientAccessRules;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ClientAccessRuleCollectionCacheableItem : TenantConfigurationCacheableItem<Organization>
	{
		public override long ItemSize
		{
			get
			{
				return (long)this.estimatedSize;
			}
		}

		public ClientAccessRuleCollection ClientAccessRuleCollection { get; private set; }

		public override void ReadData(IConfigurationSession configurationSession)
		{
			IEnumerable<ADClientAccessRule> enumerable = this.ReadRawData(configurationSession);
			this.ClientAccessRuleCollection = new ClientAccessRuleCollection(configurationSession.GetOrgContainerId().ToString());
			this.estimatedSize = 0;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.ClientAccessRulesCommon.ImplicitAllowLocalClientAccessRulesEnabled.Enabled && (null == configurationSession.SessionSettings.CurrentOrganizationId || OrganizationId.ForestWideOrgId.Equals(configurationSession.SessionSettings.CurrentOrganizationId)))
			{
				ClientAccessRule allowLocalClientAccessRule = ClientAccessRulesUtils.GetAllowLocalClientAccessRule();
				if (allowLocalClientAccessRule != null)
				{
					this.ClientAccessRuleCollection.AddWithoutNameCheck(allowLocalClientAccessRule);
					this.estimatedSize += allowLocalClientAccessRule.GetEstimatedSize();
				}
			}
			foreach (ADClientAccessRule adclientAccessRule in enumerable)
			{
				ClientAccessRule clientAccessRule = adclientAccessRule.GetClientAccessRule();
				this.ClientAccessRuleCollection.AddWithoutNameCheck(clientAccessRule);
				this.estimatedSize += clientAccessRule.GetEstimatedSize();
			}
		}

		private IEnumerable<ADClientAccessRule> ReadRawData(IConfigurationSession configurationSession)
		{
			IEnumerable<ADClientAccessRule> adClientAccessRules = configurationSession.FindAllPaged<ADClientAccessRule>().ReadAllPages();
			ClientAccessRulesPriorityManager clientAccessRulesPriorityManager = new ClientAccessRulesPriorityManager(adClientAccessRules);
			return clientAccessRulesPriorityManager.ADClientAccessRules;
		}

		private int estimatedSize;
	}
}
