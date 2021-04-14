using System;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class ShutdownCommand : ISchedulerCommand
	{
		public ShutdownCommand(ProcessingScheduler scheduler)
		{
			this.scheduler = scheduler;
		}

		public void Execute()
		{
			this.scheduler.StartShutdown();
		}

		public bool WaitForDone(int timeoutMilliseconds = -1)
		{
			return this.scheduler.WaitForShutdown(timeoutMilliseconds);
		}

		private readonly ProcessingScheduler scheduler;
	}
}
