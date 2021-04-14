using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class RuleBaseBatch : ConfigurableBatch
	{
		public RuleBaseBatch()
		{
		}

		public RuleBaseBatch(IEnumerable<RuleBase> items)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (RuleBase ruleBase in items)
			{
				Guid identity = CombGuidGenerator.NewGuid();
				batchPropertyTable.AddPropertyValue(identity, RuleBase.IDProperty, ruleBase.ID);
				batchPropertyTable.AddPropertyValue(identity, RuleBase.RuleIDProperty, ruleBase.RuleID);
				batchPropertyTable.AddPropertyValue(identity, RuleBase.RuleTypeProperty, ruleBase.RuleType);
				batchPropertyTable.AddPropertyValue(identity, RuleBase.ScopeIDProperty, ruleBase.ScopeID);
				batchPropertyTable.AddPropertyValue(identity, RuleBase.GroupIDProperty, ruleBase.GroupID);
				batchPropertyTable.AddPropertyValue(identity, RuleBase.SequenceProperty, ruleBase.Sequence);
				batchPropertyTable.AddPropertyValue(identity, RuleBase.IsPersistentProperty, ruleBase.IsPersistent);
				batchPropertyTable.AddPropertyValue(identity, RuleBase.IsActiveProperty, ruleBase.IsActive);
				if (ruleBase.State != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleBase.StateProperty, ruleBase.State);
				}
				if (ruleBase.AddedVersion != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleBase.AddedVersionProperty, ruleBase.AddedVersion);
				}
				if (ruleBase.RemovedVersion != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleBase.RemovedVersionProperty, ruleBase.RemovedVersion);
				}
			}
			base.Batch = batchPropertyTable;
		}
	}
}
