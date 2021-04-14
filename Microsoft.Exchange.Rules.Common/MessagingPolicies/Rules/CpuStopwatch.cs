using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class CpuStopwatch
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetCurrentThread();

		internal long ElapsedMilliseconds
		{
			get
			{
				return this.elapsedMs;
			}
		}

		internal void Restart()
		{
			this.elapsedMs = 0L;
			this.Start();
		}

		internal void Reset()
		{
			this.elapsedMs = 0L;
		}

		internal void Start()
		{
			this.monitoredThread = CpuStopwatch.GetCurrentThread();
			if (this.monitoredThread == IntPtr.Zero)
			{
				throw new InvalidOperationException("CpuStopWatch: GetCurrentThread API failed to get current thread handle.");
			}
			long num;
			long num2;
			CpuStopwatch.GetThreadTimes(this.monitoredThread, out num, out num2, out this.kernelStartMark, out this.userStartMark);
		}

		internal void Stop()
		{
			if (this.monitoredThread == IntPtr.Zero)
			{
				throw new InvalidOperationException("CpuStopWatch: Start was not called before Stop - current thread is not set.");
			}
			long num;
			long num2;
			long num3;
			long num4;
			CpuStopwatch.GetThreadTimes(this.monitoredThread, out num, out num2, out num3, out num4);
			this.elapsedMs = (num3 - this.kernelStartMark + (num4 - this.userStartMark)) / 10000L;
			this.kernelStartMark = num3;
			this.userStartMark = num4;
		}

		private long kernelStartMark;

		private long userStartMark;

		private IntPtr monitoredThread = IntPtr.Zero;

		private long elapsedMs;
	}
}
