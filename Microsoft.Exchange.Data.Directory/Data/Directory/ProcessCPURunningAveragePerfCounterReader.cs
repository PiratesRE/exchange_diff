using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ProcessCPURunningAveragePerfCounterReader : BaseRunningAveragePerfCounterReader
	{
		public ProcessCPURunningAveragePerfCounterReader() : base(10, 500U)
		{
			this.currentProcess = NativeMethods.GetCurrentProcess();
			if (CPUUsage.GetCurrentCPU(this.currentProcess, ref this.lastCPU))
			{
				this.lastTime = DateTime.UtcNow;
				return;
			}
			this.lastCPU = 0L;
			this.lastTime = DateTime.MinValue;
		}

		protected override bool AcquireCounter()
		{
			return true;
		}

		protected override float ReadCounter()
		{
			float result;
			CPUUsage.CalculateCPUUsagePercentage(this.currentProcess, ref this.lastTime, ref this.lastCPU, out result);
			return result;
		}

		private DateTime lastTime;

		private long lastCPU;

		private IntPtr currentProcess;
	}
}
