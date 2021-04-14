using System;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface ISchedulerCommand
	{
		void Execute();
	}
}
