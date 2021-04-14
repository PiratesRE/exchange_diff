using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContainsSubjectOrBodyStringCondition : StringCondition
	{
		private ContainsSubjectOrBodyStringCondition(Rule rule, string[] text) : base(ConditionType.ContainsSubjectOrBodyStringCondition, rule, text)
		{
		}

		public static ContainsSubjectOrBodyStringCondition Create(Rule rule, string[] text)
		{
			Condition.CheckParams(new object[]
			{
				rule,
				text
			});
			return new ContainsSubjectOrBodyStringCondition(rule, text);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateSubjectOrBodyRestriction(base.Text);
		}
	}
}
