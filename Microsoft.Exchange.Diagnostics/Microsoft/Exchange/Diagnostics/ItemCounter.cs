using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class ItemCounter
	{
		public ItemCounter(ExPerformanceCounter current, ExPerformanceCounter peak, ExPerformanceCounter total)
		{
			if (current == null)
			{
				throw new ArgumentNullException("current");
			}
			if (peak == null)
			{
				throw new ArgumentNullException("peak");
			}
			if (total == null)
			{
				throw new ArgumentNullException("total");
			}
			this.current = current;
			this.peak = peak;
			this.total = total;
		}

		public void Increment()
		{
			this.total.Increment();
			lock (this.current)
			{
				long val = this.current.Increment();
				this.peak.RawValue = Math.Max(this.peak.RawValue, val);
			}
		}

		public void Decrement()
		{
			lock (this.current)
			{
				this.current.Decrement();
			}
		}

		private ExPerformanceCounter current;

		private ExPerformanceCounter peak;

		private ExPerformanceCounter total;
	}
}
