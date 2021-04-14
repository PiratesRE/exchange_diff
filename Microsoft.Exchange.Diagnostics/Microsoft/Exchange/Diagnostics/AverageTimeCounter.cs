using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class AverageTimeCounter : AverageTimeCounterBase, ITimerCounter, IDisposable
	{
		public AverageTimeCounter(ExPerformanceCounter averageCount, ExPerformanceCounter averageBase) : base(averageCount, averageBase)
		{
		}

		public AverageTimeCounter(ExPerformanceCounter averageCount, ExPerformanceCounter averageBase, bool autoStart) : base(averageCount, averageBase, autoStart)
		{
		}

		void IDisposable.Dispose()
		{
			if (base.IsStarted)
			{
				base.Stop();
			}
		}
	}
}
