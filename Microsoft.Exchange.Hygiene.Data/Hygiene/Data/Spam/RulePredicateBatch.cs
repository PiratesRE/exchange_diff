using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class RulePredicateBatch : ConfigurableBatch
	{
		public RulePredicateBatch()
		{
		}

		public RulePredicateBatch(IEnumerable<RulePredicate> items)
		{
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (RulePredicate rulePredicate in items)
			{
				Guid identity = CombGuidGenerator.NewGuid();
				batchPropertyTable.AddPropertyValue(identity, RulePredicate.IDProperty, rulePredicate.ID);
				batchPropertyTable.AddPropertyValue(identity, RulePredicate.PredicateIDProperty, rulePredicate.PredicateID);
				batchPropertyTable.AddPropertyValue(identity, RulePredicate.PredicateTypeProperty, rulePredicate.PredicateType);
				if (rulePredicate.Sequence != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RulePredicate.SequenceProperty, rulePredicate.Sequence);
				}
				if (rulePredicate.ParentID != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RulePredicate.ParentIDProperty, rulePredicate.ParentID);
				}
				if (rulePredicate.ProcessorID != null)
				{
					batchPropertyTable.AddPropertyValue(identity, RulePredicate.ProcessorIdProperty, rulePredicate.ProcessorID);
				}
			}
			base.Batch = batchPropertyTable;
		}
	}
}
