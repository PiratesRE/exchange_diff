using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ReportingWebService
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
			if (this.stopwatch != null)
			{
				base.AddSample(this.stopwatch.ElapsedTicks);
			}
			this.stopwatch = null;
		}

		public void Dispose()
		{
			if (this.stopwatch != null)
			{
				this.Stop();
			}
		}

		private Stopwatch stopwatch;
	}
}
