using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AutomaticForwardCondition : Condition
	{
		private AutomaticForwardCondition(Rule rule) : base(ConditionType.AutomaticForwardCondition, rule)
		{
		}

		public static AutomaticForwardCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new AutomaticForwardCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreatePropertyRestriction<bool>(PropTag.AutoForwarded, true);
		}
	}
}
