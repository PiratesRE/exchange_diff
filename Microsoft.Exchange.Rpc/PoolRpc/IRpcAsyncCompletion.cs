using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	public interface IRpcAsyncCompletion
	{
		void AbortAsyncCall(uint exceptionCode);

		void FailAsyncCall(int errorCode, ArraySegment<byte> auxiliaryOut);
	}
}
