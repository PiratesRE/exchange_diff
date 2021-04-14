using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReadReceiptCondition : Condition
	{
		private ReadReceiptCondition(Rule rule) : base(ConditionType.ReadReceiptCondition, rule)
		{
		}

		public static ReadReceiptCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new ReadReceiptCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			PropTag tag = base.Rule.PropertyDefinitionToPropTagFromCache(InternalSchema.IsReadReceipt);
			return new Restriction.PropertyRestriction(Restriction.RelOp.Equal, tag, true);
		}
	}
}
