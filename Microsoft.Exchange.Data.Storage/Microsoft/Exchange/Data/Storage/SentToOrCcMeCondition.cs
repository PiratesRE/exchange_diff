using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SentToOrCcMeCondition : Condition
	{
		private SentToOrCcMeCondition(Rule rule) : base(ConditionType.SentToOrCcMeCondition, rule)
		{
		}

		public static SentToOrCcMeCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new SentToOrCcMeCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateBooleanPropertyRestriction(PropTag.MessageRecipMe, true, Restriction.RelOp.Equal);
		}
	}
}
