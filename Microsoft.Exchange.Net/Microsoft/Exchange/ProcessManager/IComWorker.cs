using System;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.ProcessManager
{
	internal interface IComWorker<IComInterface>
	{
		IComInterface Worker { get; }

		SafeProcessHandle SafeProcessHandle { get; }

		int ProcessId { get; }
	}
}
