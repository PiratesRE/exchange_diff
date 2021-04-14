using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContainsBodyStringCondition : StringCondition
	{
		private ContainsBodyStringCondition(Rule rule, string[] text) : base(ConditionType.ContainsBodyStringCondition, rule, text)
		{
		}

		public static ContainsBodyStringCondition Create(Rule rule, string[] text)
		{
			Condition.CheckParams(new object[]
			{
				rule,
				text
			});
			return new ContainsBodyStringCondition(rule, text);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateORStringContentRestriction(base.Text, PropTag.Body, ContentFlags.SubString | ContentFlags.IgnoreCase);
		}
	}
}
