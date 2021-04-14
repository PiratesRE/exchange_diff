using System;

namespace Microsoft.Exchange.Transport.Scheduler.Contracts
{
	internal interface IProcessingSchedulerAdmin
	{
		bool IsRunning { get; }

		void Pause();

		void Resume();

		bool Shutdown(int timeoutMilliseconds = -1);

		SchedulerDiagnosticsInfo GetDiagnosticsInfo();
	}
}
