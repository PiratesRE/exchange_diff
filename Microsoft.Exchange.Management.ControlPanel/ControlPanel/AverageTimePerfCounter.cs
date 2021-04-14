using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class AverageTimePerfCounter : AveragePerfCounter, IDisposable
	{
		public AverageTimePerfCounter(ExPerformanceCounter averageCount, ExPerformanceCounter averageBase) : base(averageCount, averageBase)
		{
		}

		public AverageTimePerfCounter(ExPerformanceCounter averageCount, ExPerformanceCounter averageBase, bool autoStart) : base(averageCount, averageBase)
		{
			if (autoStart)
			{
				this.Start();
			}
		}

		public void Start()
		{
			this.stopwatch = Stopwatch.StartNew();
		}

		public void Stop()
		{
			base.AddSample(this.stopwatch.ElapsedTicks);
			this.stopwatch = null;
		}

		void IDisposable.Dispose()
		{
			if (this.stopwatch != null)
			{
				this.Stop();
			}
		}

		private Stopwatch stopwatch;
	}
}
