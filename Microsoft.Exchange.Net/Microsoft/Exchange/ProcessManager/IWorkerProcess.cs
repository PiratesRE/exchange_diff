using System;

namespace Microsoft.Exchange.ProcessManager
{
	internal interface IWorkerProcess
	{
		void Retire();

		void Stop();

		void Pause();

		void Continue();
	}
}
