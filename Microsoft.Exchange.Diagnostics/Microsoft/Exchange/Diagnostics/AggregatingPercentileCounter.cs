using System;

namespace Microsoft.Exchange.Diagnostics
{
	[Serializable]
	public sealed class AggregatingPercentileCounter : PercentileCounter, IAggregatingPercentileCounter, IPercentileCounter
	{
		public AggregatingPercentileCounter(long valueGranularity, long valueMaximum) : base(TimeSpan.MaxValue, TimeSpan.MaxValue, valueGranularity, valueMaximum, null)
		{
		}

		public void IncrementValue(ref long value, long increment)
		{
			lock (this.syncObject)
			{
				if (value + increment < 0L)
				{
					throw new ArgumentOutOfRangeException("increment", "Increment would make value negative");
				}
				if (value > 0L)
				{
					base.RemoveValue(value);
				}
				value += increment;
				base.AddValue(value);
			}
		}
	}
}
