using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PermanentDeleteAction : Action
	{
		private PermanentDeleteAction(Rule rule) : base(ActionType.PermanentDeleteAction, rule)
		{
		}

		public static PermanentDeleteAction Create(Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule
			});
			return new PermanentDeleteAction(rule);
		}

		internal override RuleAction BuildRuleAction()
		{
			return new RuleAction.Delete();
		}
	}
}
