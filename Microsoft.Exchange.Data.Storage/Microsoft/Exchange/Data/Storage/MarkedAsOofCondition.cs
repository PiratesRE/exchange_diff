using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MarkedAsOofCondition : FormsCondition
	{
		private MarkedAsOofCondition(Rule rule, string[] text) : base(ConditionType.MarkedAsOofCondition, rule, text)
		{
		}

		public static MarkedAsOofCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new MarkedAsOofCondition(rule, new string[]
			{
				"IPM.Note.Rules.OofTemplate.Microsoft"
			});
		}
	}
}
