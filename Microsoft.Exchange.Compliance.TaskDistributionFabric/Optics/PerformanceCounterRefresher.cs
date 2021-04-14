using System;
using System.Threading;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Optics
{
	internal class PerformanceCounterRefresher : IDisposable
	{
		public PerformanceCounterRefresher()
		{
			this.performanceCounterUpdateTimer = new Timer(new TimerCallback(this.UpdateCounters), null, TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(30.0));
		}

		public void Dispose()
		{
			if (this.performanceCounterUpdateTimer != null)
			{
				this.performanceCounterUpdateTimer.Dispose();
				this.performanceCounterUpdateTimer = null;
			}
		}

		private void UpdateCounters(object state)
		{
			foreach (PerformanceCounterAccessor performanceCounterAccessor in PerformanceCounterAccessorRegistry.Instance.GetAllRegisteredAccessors())
			{
				performanceCounterAccessor.UpdateCounters();
			}
		}

		private Timer performanceCounterUpdateTimer;
	}
}
