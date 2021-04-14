using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class PerfCounterGroup
	{
		public PerfCounterGroup(ExPerformanceCounter current, ExPerformanceCounter peak)
		{
			if (current == null)
			{
				throw new ArgumentNullException("current");
			}
			if (peak == null)
			{
				throw new ArgumentNullException("peak");
			}
			this.current = current;
			this.peak = peak;
		}

		public PerfCounterGroup(ExPerformanceCounter current, ExPerformanceCounter peak, ExPerformanceCounter total) : this(current, peak)
		{
			if (total == null)
			{
				throw new ArgumentNullException("total");
			}
			this.total = total;
		}

		public void Increment()
		{
			lock (this)
			{
				if (this.total != null)
				{
					this.total.Increment();
				}
				long val = this.current.Increment();
				this.peak.RawValue = Math.Max(this.peak.RawValue, val);
			}
		}

		public void Decrement()
		{
			lock (this)
			{
				this.current.Decrement();
			}
		}

		private ExPerformanceCounter current;

		private ExPerformanceCounter peak;

		private ExPerformanceCounter total;
	}
}
