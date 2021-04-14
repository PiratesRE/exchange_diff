using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	public class AverageTimeCounterBase : AverageCounter
	{
		public AverageTimeCounterBase(ExPerformanceCounter averageCount, ExPerformanceCounter averageBase) : base(averageCount, averageBase)
		{
		}

		public AverageTimeCounterBase(ExPerformanceCounter averageCount, ExPerformanceCounter averageBase, bool autoStart) : base(averageCount, averageBase)
		{
			if (autoStart)
			{
				this.Start();
			}
		}

		private Stopwatch Stopwatch { get; set; }

		protected bool IsStarted
		{
			get
			{
				return this.Stopwatch != null;
			}
		}

		public void Start()
		{
			this.Stopwatch = Stopwatch.StartNew();
		}

		public long Stop()
		{
			long elapsedTicks = this.Stopwatch.ElapsedTicks;
			base.AddSample(elapsedTicks);
			this.Stopwatch = null;
			return elapsedTicks;
		}
	}
}
