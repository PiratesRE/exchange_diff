using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class ADJournalRuleStorageManager : ADRuleStorageManager
	{
		public ADJournalRuleStorageManager(string ruleCollectionName, IConfigDataProvider session) : base(ruleCollectionName, session)
		{
		}

		public ADJournalRuleStorageManager(string ruleCollectionName, List<JournalRuleData> rules)
		{
			this.ruleCollectionName = ruleCollectionName;
			if (rules != null)
			{
				foreach (JournalRuleData journalRuleData in rules)
				{
					TransportRule transportRule = new TransportRule();
					transportRule.ImmutableId = journalRuleData.ImmutableId;
					transportRule.Xml = journalRuleData.Xml;
					transportRule.Priority = journalRuleData.Priority;
					transportRule.Name = journalRuleData.Name;
					this.adRules.Add(transportRule);
				}
				this.adRules.Sort(new Comparison<TransportRule>(ADRuleStorageManager.CompareTransportRule));
			}
		}

		public static Guid GetLawfulInterceptTenantGuid(string lawfulInterceptTenantName)
		{
			Guid empty = Guid.Empty;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromTenantAcceptedDomain(lawfulInterceptTenantName), 69, "GetLawfulInterceptTenantGuid", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\rules\\Journaling\\ADJournalRuleStorageManager.cs");
			ExchangeConfigurationUnit[] array = tenantOrTopologyConfigurationSession.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, null, null, 1);
			if (array != null && array.Length > 0 && !string.IsNullOrEmpty(array[0].ExternalDirectoryOrganizationId))
			{
				empty = new Guid(array[0].ExternalDirectoryOrganizationId);
			}
			return empty;
		}

		protected override TransportRuleSerializer Serializer
		{
			get
			{
				return JournalingRuleSerializer.Instance;
			}
		}

		protected override TransportRuleParser Parser
		{
			get
			{
				return JournalingRuleParser.Instance;
			}
		}
	}
}
