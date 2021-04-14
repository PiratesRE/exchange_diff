using System;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	public interface IPoolWaitForNotificationsCompletion : IRpcAsyncCompletion
	{
		void CompleteAsyncCall(uint[] sessionHandles);
	}
}
