using System;
using System.Threading;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IUMAsyncComponent
	{
		AutoResetEvent StoppedEvent { get; }

		bool IsInitialized { get; }

		string Name { get; }

		void StartNow(StartupStage stage);

		void StopAsync();

		void CleanupAfterStopped();
	}
}
