using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FlagMessageAction : ActionBase
	{
		private FlagMessageAction(ActionType actionType, FlagStatus flagStatus, Rule rule) : base(actionType, rule)
		{
			this.flagStatus = flagStatus;
		}

		public static FlagMessageAction Create(FlagStatus flagStatus, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule
			});
			EnumValidator.ThrowIfInvalid<FlagStatus>(flagStatus, "flagStatus");
			return new FlagMessageAction(ActionType.FlagMessageAction, flagStatus, rule);
		}

		public FlagStatus FlagStatus
		{
			get
			{
				return this.flagStatus;
			}
		}

		internal override RuleAction BuildRuleAction()
		{
			PropTag propTag = (PropTag)277872643U;
			return new RuleAction.Tag(new PropValue(propTag, this.flagStatus));
		}

		private readonly FlagStatus flagStatus;
	}
}
