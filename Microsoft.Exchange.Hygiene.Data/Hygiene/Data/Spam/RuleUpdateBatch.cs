using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class RuleUpdateBatch : ConfigurableBatch
	{
		public RuleUpdateBatch()
		{
		}

		public RuleUpdateBatch(IEnumerable<RuleUpdate> items)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (RuleUpdate ruleUpdate in items)
			{
				Guid identity = CombGuidGenerator.NewGuid();
				if (ruleUpdate.ID != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleUpdate.IDProperty, ruleUpdate.ID);
				}
				if (ruleUpdate.RuleID != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleUpdate.RuleIDProperty, ruleUpdate.RuleID);
				}
				if (ruleUpdate.RuleType != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleUpdate.RuleTypeProperty, ruleUpdate.RuleType);
				}
				if (ruleUpdate.IsPersistent != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleUpdate.IsPersistentProperty, ruleUpdate.IsPersistent);
				}
				if (ruleUpdate.IsActive != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleUpdate.IsActiveProperty, ruleUpdate.IsActive);
				}
				if (ruleUpdate.State != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleUpdate.StateProperty, ruleUpdate.State);
				}
				if (ruleUpdate.AddedVersion != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleUpdate.AddedVersionProperty, ruleUpdate.AddedVersion);
				}
				if (ruleUpdate.RemovedVersion != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RuleUpdate.RemovedVersionProperty, ruleUpdate.RemovedVersion);
				}
			}
			base.Batch = batchPropertyTable;
		}
	}
}
