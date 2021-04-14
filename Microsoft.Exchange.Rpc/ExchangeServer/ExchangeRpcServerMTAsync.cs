using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal abstract class ExchangeRpcServerMTAsync : ExchangeRpcServerAsyncBase
	{
		public ExchangeRpcServerMTAsync()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ExchangeRpcServerMTAsync()
		{
			IntPtr rpcIntfHandle = new IntPtr(<Module>.asyncemsmdbMT_v0_1_s_ifspec);
			ExchangeRpcServerMTAsync.RpcIntfHandle = rpcIntfHandle;
		}

		public static IntPtr RpcIntfHandle;
	}
}
