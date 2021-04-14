using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MarkImportanceAction : ActionBase
	{
		private MarkImportanceAction(ActionType actionType, Importance importance, Rule rule) : base(actionType, rule)
		{
			this.importance = importance;
		}

		public static MarkImportanceAction Create(Importance importance, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule
			});
			EnumValidator.ThrowIfInvalid<Importance>(importance, "importance");
			return new MarkImportanceAction(ActionType.MarkImportanceAction, importance, rule);
		}

		public Importance Importance
		{
			get
			{
				return this.importance;
			}
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
			return new RuleAction.Tag(new PropValue(PropTag.Importance, this.Importance));
		}

		private readonly Importance importance;
	}
}
