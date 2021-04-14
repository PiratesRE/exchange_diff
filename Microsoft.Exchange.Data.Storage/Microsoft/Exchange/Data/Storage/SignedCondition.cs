using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SignedCondition : Condition
	{
		private SignedCondition(Rule rule) : base(ConditionType.SignedCondition, rule)
		{
		}

		public static SignedCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new SignedCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			PropTag tag = base.Rule.PropertyDefinitionToPropTagFromCache(InternalSchema.IsSigned);
			return new Restriction.PropertyRestriction(Restriction.RelOp.Equal, tag, true);
		}
	}
}
