using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamRuleBlobBatch : ConfigurableBatch
	{
		public SpamRuleBlobBatch()
		{
		}

		public SpamRuleBlobBatch(IEnumerable<SpamRuleBlob> items)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (SpamRuleBlob spamRuleBlob in items)
			{
				Guid identity = CombGuidGenerator.NewGuid();
				batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.IdProperty, spamRuleBlob.Id);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.RuleIdProperty, spamRuleBlob.RuleId);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.GroupIdProperty, spamRuleBlob.GroupId);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.ScopeIdProperty, spamRuleBlob.ScopeId);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.RuleDataProperty, spamRuleBlob.RuleData);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.RuleMetaDataProperty, spamRuleBlob.RuleMetaData);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.PriorityProperty, spamRuleBlob.Priority);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.PublishingStateProperty, spamRuleBlob.PublishingState);
				batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.ProcessorDataProperty, spamRuleBlob.ProcessorData);
				if (spamRuleBlob.DeletedDatetime != null)
				{
					batchPropertyTable.AddPropertyValue(identity, SpamRuleBlobSchema.DeletedDatetimeProperty, spamRuleBlob.DeletedDatetime);
				}
			}
			base.Batch = batchPropertyTable;
		}
	}
}
