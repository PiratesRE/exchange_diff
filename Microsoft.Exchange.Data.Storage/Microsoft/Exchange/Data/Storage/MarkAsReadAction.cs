using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MarkAsReadAction : Action
	{
		private MarkAsReadAction(Rule rule) : base(ActionType.MarkAsReadAction, rule)
		{
		}

		public static MarkAsReadAction Create(Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule
			});
			return new MarkAsReadAction(rule);
		}

		public override Rule.ProviderIdEnum ProviderId
		{
			get
			{
				return Rule.ProviderIdEnum.Exchange14;
			}
		}

		internal override RuleAction BuildRuleAction()
		{
			return new RuleAction.MarkAsRead();
		}
	}
}
