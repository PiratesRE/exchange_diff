using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SentToMeCondition : Condition
	{
		private SentToMeCondition(Rule rule) : base(ConditionType.SentToMeCondition, rule)
		{
		}

		public static SentToMeCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new SentToMeCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateBooleanPropertyRestriction(PropTag.MessageToMe, true, Restriction.RelOp.Equal);
		}
	}
}
