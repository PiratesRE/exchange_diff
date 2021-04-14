using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	public interface IPoolSessionDoRpcCompletion : IRpcAsyncCompletion
	{
		void CompleteAsyncCall(uint flags, ArraySegment<byte> response, ArraySegment<byte> auxiliaryOut);
	}
}
