using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	public interface IPoolConnectCompletion : IRpcAsyncCompletion
	{
		void CompleteAsyncCall(IntPtr contextHandle, uint flags, uint maxPoolSize, Guid poolGuid, ArraySegment<byte> auxiliaryOut);
	}
}
