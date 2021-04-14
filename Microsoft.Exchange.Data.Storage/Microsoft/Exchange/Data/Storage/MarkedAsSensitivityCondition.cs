using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MarkedAsSensitivityCondition : Condition
	{
		private MarkedAsSensitivityCondition(Rule rule, Sensitivity sensitivity) : base(ConditionType.MarkedAsSensitivityCondition, rule)
		{
			this.sensitivity = sensitivity;
		}

		public static MarkedAsSensitivityCondition Create(Rule rule, Sensitivity sensitivity)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			EnumValidator.ThrowIfInvalid<Sensitivity>(sensitivity, "sensitivity");
			return new MarkedAsSensitivityCondition(rule, sensitivity);
		}

		public Sensitivity Sensitivity
		{
			get
			{
				return this.sensitivity;
			}
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateIntPropertyRestriction(PropTag.Sensitivity, (int)this.Sensitivity, Restriction.RelOp.Equal);
		}

		private readonly Sensitivity sensitivity;
	}
}
