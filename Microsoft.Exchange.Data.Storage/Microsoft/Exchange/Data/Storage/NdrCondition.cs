using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NdrCondition : Condition
	{
		private NdrCondition(Rule rule) : base(ConditionType.NdrCondition, rule)
		{
		}

		public static NdrCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new NdrCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateIsNdrRestrictions();
		}
	}
}
