using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IComponentsWrapper
	{
		bool IsPaused { get; }

		bool IsActive { get; }

		bool IsShuttingDown { get; }

		bool IsBridgeHead { get; }

		object SyncRoot { get; }
	}
}
