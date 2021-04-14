using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskPerformanceData
	{
		public PerformanceData Start
		{
			get
			{
				return this.start;
			}
			set
			{
				if (this.haveStartValue)
				{
					throw new InvalidOperationException("Start is already set.");
				}
				this.start = value;
				this.haveStartValue = true;
			}
		}

		public PerformanceData End
		{
			get
			{
				return this.end;
			}
			set
			{
				if (!this.haveStartValue)
				{
					throw new InvalidOperationException("May not set End before Start.");
				}
				if (this.haveEndValue)
				{
					throw new InvalidOperationException("End is already set.");
				}
				this.end = value;
				bool flag = false;
				if (this.start.Latency <= this.end.Latency)
				{
					flag = (this.start.Count <= this.end.Count);
				}
				this.difference = (flag ? (value - this.start) : PerformanceData.Unknown);
				this.haveEndValue = true;
			}
		}

		public PerformanceData Difference
		{
			get
			{
				return this.difference;
			}
		}

		public string Operations { get; internal set; }

		internal void InvalidateIfAsynchronous()
		{
			if (this.Start.ThreadId != this.End.ThreadId)
			{
				this.difference = PerformanceData.Unknown;
			}
		}

		private PerformanceData start;

		private PerformanceData end;

		private PerformanceData difference;

		private bool haveStartValue;

		private bool haveEndValue;
	}
}
