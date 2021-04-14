using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal class CpuTracker : DisposeTrackableBase
	{
		private CpuTracker(string instance)
		{
			this.instance = instance;
			this.currentThread = ProcessThreadHelper.Current;
			this.startCpuTime = this.currentThread.TotalProcessorTime;
		}

		public TimeSpan TotalProcessorTime
		{
			get
			{
				if (this.totalProcessorTime == null)
				{
					throw new InvalidOperationException(DiagnosticsResources.ExcInvalidOpPropertyBeforeEnd);
				}
				return this.totalProcessorTime.Value;
			}
		}

		public static CpuTracker StartCpuTracking()
		{
			return new CpuTracker(string.Empty);
		}

		public static CpuTracker StartCpuTracking(string instance)
		{
			return new CpuTracker(instance);
		}

		public void End()
		{
			this.Dispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CpuTracker>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (!base.IsDisposed)
			{
				this.totalProcessorTime = new TimeSpan?(this.currentThread.TotalProcessorTime - this.startCpuTime);
				ActivityContext.AddOperation(ActivityOperationType.CustomCpu, this.instance, (float)this.TotalProcessorTime.TotalMilliseconds, 1);
			}
		}

		private readonly string instance;

		private readonly TimeSpan startCpuTime;

		private readonly ProcessThread currentThread;

		private TimeSpan? totalProcessorTime = null;
	}
}
