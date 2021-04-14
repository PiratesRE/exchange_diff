using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContainsHeaderStringCondition : StringCondition
	{
		private ContainsHeaderStringCondition(Rule rule, string[] text) : base(ConditionType.ContainsHeaderStringCondition, rule, text)
		{
		}

		public static ContainsHeaderStringCondition Create(Rule rule, string[] text)
		{
			Condition.CheckParams(new object[]
			{
				rule,
				text
			});
			return new ContainsHeaderStringCondition(rule, text);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateORStringContentRestriction(base.Text, PropTag.TransportMessageHeaders, ContentFlags.SubString | ContentFlags.IgnoreCase);
		}
	}
}
