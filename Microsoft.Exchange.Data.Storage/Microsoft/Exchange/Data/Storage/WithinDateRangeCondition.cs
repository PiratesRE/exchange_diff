using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WithinDateRangeCondition : Condition
	{
		private WithinDateRangeCondition(Rule rule, ExDateTime? rangeLow, ExDateTime? rangeHigh) : base(ConditionType.WithinDateRangeCondition, rule)
		{
			this.rangeLow = rangeLow;
			this.rangeHigh = rangeHigh;
		}

		public static WithinDateRangeCondition Create(Rule rule, ExDateTime? rangeLow, ExDateTime? rangeHigh)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			if (rangeLow == null && rangeHigh == null)
			{
				rule.ThrowValidateException(delegate
				{
					throw new ArgumentException("Both cannot be null");
				}, "rangeLow, rangeHigh");
			}
			if (rangeLow != null && rangeHigh != null && rangeLow > rangeHigh)
			{
				rule.ThrowValidateException(delegate
				{
					throw new ArgumentOutOfRangeException("rangeLow cannot be > rangeHigh");
				}, "rangeLow, rangeHigh");
			}
			return new WithinDateRangeCondition(rule, rangeLow, rangeHigh);
		}

		public ExDateTime? RangeLow
		{
			get
			{
				return this.rangeLow;
			}
		}

		public ExDateTime? RangeHigh
		{
			get
			{
				return this.rangeHigh;
			}
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateOneOrTwoTimesRestrictions((this.rangeHigh != null) ? new ExDateTime?(ExTimeZone.UtcTimeZone.ConvertDateTime(this.rangeHigh.Value)) : null, (this.rangeLow != null) ? new ExDateTime?(ExTimeZone.UtcTimeZone.ConvertDateTime(this.rangeLow.Value)) : null);
		}

		private readonly ExDateTime? rangeLow = null;

		private readonly ExDateTime? rangeHigh = null;
	}
}
