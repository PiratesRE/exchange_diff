using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal abstract class ExchangeRpcServerAsyncBase : RpcServerBase
	{
		public ExchangeRpcServerAsyncBase()
		{
		}

		public virtual IProxyServer GetProxyServer()
		{
			return null;
		}
	}
}
