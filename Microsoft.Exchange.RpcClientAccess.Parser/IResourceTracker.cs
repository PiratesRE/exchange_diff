using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal interface IResourceTracker
	{
		bool TryReserveMemory(int size);

		int StreamSizeLimit { get; }
	}
}
