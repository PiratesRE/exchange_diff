using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MarkSensitivityAction : ActionBase
	{
		private MarkSensitivityAction(ActionType actionType, Sensitivity sensitivity, Rule rule) : base(actionType, rule)
		{
			this.sensitivity = sensitivity;
		}

		public static MarkSensitivityAction Create(Sensitivity sensitivity, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule
			});
			EnumValidator.ThrowIfInvalid<Sensitivity>(sensitivity, "sensitivity");
			return new MarkSensitivityAction(ActionType.MarkSensitivityAction, sensitivity, rule);
		}

		public Sensitivity Sensitivity
		{
			get
			{
				return this.sensitivity;
			}
		}

		internal override RuleAction BuildRuleAction()
		{
			return new RuleAction.Tag(new PropValue(PropTag.Sensitivity, this.Sensitivity));
		}

		private Sensitivity sensitivity;
	}
}
