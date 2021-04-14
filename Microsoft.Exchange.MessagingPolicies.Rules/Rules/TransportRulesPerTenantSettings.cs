using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TransportRulesPerTenantSettings : TenantConfigurationCacheableItem<TransportRule>
	{
		protected virtual string RuleCollectionName
		{
			get
			{
				return "TransportVersioned";
			}
		}

		protected virtual RuleParser Parser
		{
			get
			{
				return TransportRuleParser.Instance;
			}
		}

		public RuleCollection RuleCollection
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.ruleCollection;
			}
		}

		public override long ItemSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.estimatedSize;
			}
		}

		public override void ReadData(IConfigurationSession session, object state = null)
		{
			ADObjectId containerId = ADRuleStorageManager.GetContainerId(session, this.RuleCollectionName);
			IEnumerable<TransportRule> adTransportRules = session.FindPaged<TransportRule>(null, containerId, false, null, 0);
			RuleHealthMonitor ruleHealthMonitor = state as RuleHealthMonitor;
			this.ruleCollection = this.ParseRules(adTransportRules, ruleHealthMonitor);
			this.UpdateEstimatedSize();
		}

		public override void ReadData(IConfigurationSession session)
		{
			this.ReadData(session, null);
		}

		private void UpdateEstimatedSize()
		{
			if (this.ruleCollection == null)
			{
				this.estimatedSize = 0L;
				return;
			}
			this.estimatedSize = (long)(18 + this.ruleCollection.GetEstimatedSize());
		}

		protected virtual RuleCollection ParseRules(IEnumerable<TransportRule> adTransportRules, RuleHealthMonitor ruleHealthMonitor)
		{
			RuleCollection result;
			try
			{
				List<TransportRule> list = adTransportRules as List<TransportRule>;
				if (list == null)
				{
					list = new List<TransportRule>();
					foreach (TransportRule item in adTransportRules)
					{
						list.Add(item);
					}
				}
				list.Sort(new Comparison<TransportRule>(ADRuleStorageManager.CompareTransportRule));
				result = ADRuleStorageManager.ParseRuleCollection(this.RuleCollectionName, list, ruleHealthMonitor, this.Parser);
			}
			catch (ParserException ex)
			{
				throw new DataValidationException(new PropertyValidationError(ex.LocalizedString, TransportRuleSchema.Xml, null), ex);
			}
			return result;
		}

		internal const int FixedObjectOverhead = 18;

		protected RuleCollection ruleCollection;

		private long estimatedSize;
	}
}
