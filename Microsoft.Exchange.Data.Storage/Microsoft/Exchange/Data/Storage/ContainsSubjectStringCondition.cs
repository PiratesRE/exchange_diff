using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContainsSubjectStringCondition : StringCondition
	{
		private ContainsSubjectStringCondition(Rule rule, string[] text) : base(ConditionType.ContainsSubjectStringCondition, rule, text)
		{
		}

		public static ContainsSubjectStringCondition Create(Rule rule, string[] text)
		{
			Condition.CheckParams(new object[]
			{
				rule,
				text
			});
			return new ContainsSubjectStringCondition(rule, text);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateORStringContentRestriction(base.Text, PropTag.Subject, ContentFlags.SubString | ContentFlags.IgnoreCase);
		}
	}
}
