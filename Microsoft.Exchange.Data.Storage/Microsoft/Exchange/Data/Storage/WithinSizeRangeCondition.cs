using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WithinSizeRangeCondition : Condition
	{
		private WithinSizeRangeCondition(Rule rule, int? rangeLow, int? rangeHigh) : base(ConditionType.WithinSizeRangeCondition, rule)
		{
			this.rangeLow = rangeLow;
			this.rangeHigh = rangeHigh;
		}

		public static WithinSizeRangeCondition Create(Rule rule, int? rangeLow, int? rangeHigh)
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
			if ((rangeLow != null && rangeLow < 0) || (rangeHigh != null && rangeHigh < 0))
			{
				rule.ThrowValidateException(delegate
				{
					throw new ArgumentOutOfRangeException("Cannot be negative");
				}, "rangeLow, rangeHigh");
			}
			if (rangeLow != null && rangeHigh != null && rangeLow > rangeHigh)
			{
				rule.ThrowValidateException(delegate
				{
					throw new ArgumentOutOfRangeException("rangeLow cannot be > rangeHigh");
				}, "rangeLow, rangeHigh");
			}
			return new WithinSizeRangeCondition(rule, rangeLow, rangeHigh);
		}

		public int? RangeLow
		{
			get
			{
				return this.rangeLow;
			}
		}

		public int? RangeHigh
		{
			get
			{
				return this.rangeHigh;
			}
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateSizeRestriction(this.rangeLow, this.rangeHigh);
		}

		private readonly int? rangeLow = null;

		private readonly int? rangeHigh = null;
	}
}
