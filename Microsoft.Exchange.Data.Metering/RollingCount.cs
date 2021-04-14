using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class RollingCount<TEntityType, TCountType> : Count<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		public RollingCount(ICountedEntity<TEntityType> entity, IRollingCountConfig config, TCountType measure) : this(entity, config, measure, () => DateTime.UtcNow)
		{
		}

		public RollingCount(ICountedEntity<TEntityType> entity, IRollingCountConfig config, TCountType measure, Func<DateTime> timeProvider) : base(entity, config, measure, timeProvider)
		{
			this.rollingCountConfig = config;
		}

		public override long Total
		{
			get
			{
				if (this.rollingValue == null)
				{
					return 0L;
				}
				if (this.rollingValue.IsEmpty)
				{
					return 0L;
				}
				long sum = this.rollingValue.Sum;
				if (base.Trendline != null)
				{
					base.Trendline.AddDataPoint(sum);
				}
				return sum;
			}
		}

		public override long Average
		{
			get
			{
				if (base.Trendline != null)
				{
					return base.Trendline.GetAverage();
				}
				return this.Total;
			}
		}

		public override string ToString()
		{
			return string.Format("RollingCount for Entity {0}, Measure:{1}", base.Entity, base.Measure);
		}

		protected override Count<TEntityType, TCountType> InternalMerge(Count<TEntityType, TCountType> count)
		{
			RollingCount<TEntityType, TCountType> rollingCount = count as RollingCount<TEntityType, TCountType>;
			if (rollingCount == null)
			{
				throw new InvalidOperationException("count with a rollingConfig should be a RollingCount");
			}
			if (this.rollingValue == null)
			{
				base.CopyPropertiesTo(rollingCount);
				return rollingCount;
			}
			if (rollingCount.rollingValue == null)
			{
				return this;
			}
			if (base.NeedsUpdate)
			{
				base.TimedUpdate();
			}
			if (rollingCount.NeedsUpdate)
			{
				rollingCount.TimedUpdate();
			}
			this.rollingValue.Merge(rollingCount.rollingValue);
			base.Trendline = new Trendline(this.rollingValue.PastTotalValues, this.rollingCountConfig.WindowBucketSize, this.rollingCountConfig.IdleCleanupInterval, base.TimeProvider);
			rollingCount.CopyPropertiesTo(this);
			base.UpdateAccessTime();
			return this;
		}

		protected override void InternalAddValue(long increment)
		{
			base.UpdateAccessTime();
			if (this.rollingValue == null)
			{
				this.rollingValue = new SlidingTotalCounter(this.rollingCountConfig.WindowInterval, this.rollingCountConfig.WindowBucketSize, base.TimeProvider);
			}
			this.rollingValue.AddValue(increment);
			if (base.Trendline == null)
			{
				base.Trendline = new Trendline(this.rollingCountConfig.WindowInterval, this.rollingCountConfig.IdleCleanupInterval, base.TimeProvider);
			}
			base.Trendline.AddDataPoint(this.rollingValue.Sum);
		}

		protected override bool InternalSetValue(long value)
		{
			return false;
		}

		private readonly IRollingCountConfig rollingCountConfig;

		private SlidingTotalCounter rollingValue;
	}
}
