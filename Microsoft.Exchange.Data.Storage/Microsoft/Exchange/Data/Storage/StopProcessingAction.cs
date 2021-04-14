using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StopProcessingAction : Action
	{
		private StopProcessingAction(Rule rule) : base(ActionType.StopProcessingAction, rule)
		{
		}

		public static StopProcessingAction Create(Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule
			});
			return new StopProcessingAction(rule);
		}

		internal override RuleAction BuildRuleAction()
		{
			return null;
		}
	}
}
