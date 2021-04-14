using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SentOnlyToMeCondition : Condition
	{
		private SentOnlyToMeCondition(Rule rule) : base(ConditionType.SentOnlyToMeCondition, rule)
		{
		}

		public static SentOnlyToMeCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new SentOnlyToMeCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateOnlyToMeRestriction();
		}
	}
}
