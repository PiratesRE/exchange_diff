using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	public interface IPoolCloseSessionCompletion : IRpcAsyncCompletion
	{
		void CompleteAsyncCall();
	}
}
