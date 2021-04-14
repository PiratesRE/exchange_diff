using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class ExecutionTimeInfo : IExecutionInfo
	{
		public TimeSpan CallDuration
		{
			get
			{
				return this.callDuration;
			}
		}

		public void OnStart()
		{
			this.stopwatch.Restart();
		}

		public void OnException(Exception ex)
		{
		}

		public void OnFinish()
		{
			this.stopwatch.Stop();
			this.callDuration = this.stopwatch.Elapsed;
		}

		private Stopwatch stopwatch = new Stopwatch();

		private TimeSpan callDuration = TimeSpan.Zero;
	}
}
