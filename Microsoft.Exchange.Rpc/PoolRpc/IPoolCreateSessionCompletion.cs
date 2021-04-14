using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	public interface IPoolCreateSessionCompletion : IRpcAsyncCompletion
	{
		void CompleteAsyncCall(uint sessionHandle, string displayName, uint maximumPolls, uint retryCount, uint retryDelay, ushort sessionId, ArraySegment<byte> auxiliaryOut);
	}
}
