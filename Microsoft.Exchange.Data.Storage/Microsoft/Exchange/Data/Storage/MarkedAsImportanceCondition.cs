using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MarkedAsImportanceCondition : Condition
	{
		private MarkedAsImportanceCondition(Rule rule, Importance importance) : base(ConditionType.MarkedAsImportanceCondition, rule)
		{
			this.importance = importance;
		}

		public static MarkedAsImportanceCondition Create(Rule rule, Importance importance)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			EnumValidator.ThrowIfInvalid<Importance>(importance, "importance");
			return new MarkedAsImportanceCondition(rule, importance);
		}

		public Importance Importance
		{
			get
			{
				return this.importance;
			}
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateIntPropertyRestriction(PropTag.Importance, (int)this.Importance, Restriction.RelOp.Equal);
		}

		private readonly Importance importance;
	}
}
