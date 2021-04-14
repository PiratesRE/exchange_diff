using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal class AbsoluteCount<TEntityType, TCountType> : Count<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		public AbsoluteCount(ICountedEntity<TEntityType> entity, IAbsoluteCountConfig config, TCountType measure) : this(entity, config, measure, () => DateTime.UtcNow)
		{
		}

		public AbsoluteCount(ICountedEntity<TEntityType> entity, IAbsoluteCountConfig config, TCountType measure, Func<DateTime> timeProvider) : base(entity, config, measure, timeProvider)
		{
			this.absoluteConfig = config;
		}

		public override long Total
		{
			get
			{
				return this.absoluteValue;
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
				return this.absoluteValue;
			}
		}

		public override string ToString()
		{
			return string.Format("AbsoluteCount for Entity {0}, Measure:{1}", base.Entity, base.Measure);
		}

		protected override Count<TEntityType, TCountType> InternalMerge(Count<TEntityType, TCountType> count)
		{
			AbsoluteCount<TEntityType, TCountType> absoluteCount = count as AbsoluteCount<TEntityType, TCountType>;
			if (absoluteCount == null)
			{
				throw new InvalidOperationException("count with an absoluteConfig should be an AbsoluteCount");
			}
			if (base.NeedsUpdate)
			{
				base.TimedUpdate();
			}
			if (absoluteCount.NeedsUpdate)
			{
				absoluteCount.TimedUpdate();
			}
			this.absoluteValue += absoluteCount.absoluteValue;
			if (base.Trendline == null)
			{
				base.Trendline = absoluteCount.Trendline;
			}
			else if (absoluteCount.Trendline != null && base.Trendline.OldestPointTime > absoluteCount.Trendline.OldestPointTime)
			{
				base.Trendline = absoluteCount.Trendline;
			}
			base.UpdateAccessTime();
			absoluteCount.CopyPropertiesTo(this);
			return this;
		}

		protected override void InternalAddValue(long increment)
		{
			base.UpdateAccessTime();
			this.absoluteValue += increment;
			this.AddToTrendline();
		}

		protected override bool InternalSetValue(long value)
		{
			base.UpdateAccessTime();
			this.absoluteValue = value;
			this.AddToTrendline();
			return true;
		}

		private void AddToTrendline()
		{
			if (this.absoluteConfig.HistoryLookbackWindow <= TimeSpan.Zero)
			{
				return;
			}
			if (base.Trendline == null)
			{
				base.Trendline = new Trendline(this.absoluteConfig.HistoryLookbackWindow, this.absoluteConfig.IdleCleanupInterval, base.TimeProvider);
			}
			if (base.Trendline != null)
			{
				base.Trendline.AddDataPoint(this.absoluteValue);
			}
		}

		private readonly IAbsoluteCountConfig absoluteConfig;

		private long absoluteValue;
	}
}
