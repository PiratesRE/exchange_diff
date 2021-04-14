using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SentCcMeCondition : Condition
	{
		private SentCcMeCondition(Rule rule) : base(ConditionType.SentCcMeCondition, rule)
		{
		}

		public static SentCcMeCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new SentCcMeCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateCcToMeRestriction();
		}
	}
}
