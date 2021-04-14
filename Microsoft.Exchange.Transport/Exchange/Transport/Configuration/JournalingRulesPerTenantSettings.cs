using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal sealed class JournalingRulesPerTenantSettings : TenantConfigurationCacheableItem<TransportRule>
	{
		public JournalingRulesPerTenantSettings()
		{
		}

		public JournalingRulesPerTenantSettings(IEnumerable<TransportRule> transportRules) : base(true)
		{
			this.SetInternalData(transportRules);
		}

		public List<JournalRuleData> JournalRuleDataList
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.journalRuleDataList;
			}
		}

		public override long ItemSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				if (this.journalRuleDataList == null)
				{
					return 0L;
				}
				long num = 0L;
				foreach (JournalRuleData journalRuleData in this.journalRuleDataList)
				{
					num += journalRuleData.ItemSize;
				}
				num += 18L;
				return num;
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			QueryFilter filter = new TextFilter(ADObjectSchema.Name, "JournalingVersioned", MatchOptions.FullString, MatchFlags.Default);
			TransportRuleCollection[] array = (TransportRuleCollection[])session.Find<TransportRuleCollection>(filter, null, true, null);
			if (array.Length != 1)
			{
				ExTraceGlobals.JournalingTracer.TraceError<int>(0L, "JournalingRulesPerTenantSettings - query for JournalRuleCollection returned '{0}' results", array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					ExTraceGlobals.JournalingTracer.TraceError<int, string>(0L, "JournalingRulesPerTenantSettings Result #{0} DN - '{1}'", i, array[i].DistinguishedName.ToString());
				}
				this.SetInternalData(null);
				return;
			}
			IEnumerable<TransportRule> internalData = session.FindPaged<TransportRule>(null, array[0].Id, false, null, 0);
			this.SetInternalData(internalData);
		}

		private void SetInternalData(IEnumerable<TransportRule> transportRules)
		{
			if (transportRules == null)
			{
				this.journalRuleDataList = null;
				return;
			}
			List<JournalRuleData> list = new List<JournalRuleData>();
			foreach (TransportRule rule in transportRules)
			{
				list.Add(new JournalRuleData(rule));
			}
			this.journalRuleDataList = list;
		}

		public const string RuleCollectionName = "JournalingVersioned";

		private List<JournalRuleData> journalRuleDataList;
	}
}
