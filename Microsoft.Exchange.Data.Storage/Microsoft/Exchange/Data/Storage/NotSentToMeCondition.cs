using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NotSentToMeCondition : Condition
	{
		private NotSentToMeCondition(Rule rule) : base(ConditionType.NotSentToMeCondition, rule)
		{
		}

		public static NotSentToMeCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new NotSentToMeCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateBooleanPropertyRestriction(PropTag.MessageToMe, false, Restriction.RelOp.Equal);
		}
	}
}
