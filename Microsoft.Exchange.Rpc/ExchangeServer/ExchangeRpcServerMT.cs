using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal abstract class ExchangeRpcServerMT : ExchangeRpcServerBase
	{
		public ExchangeRpcServerMT()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.emsmdbMT_v0_81_s_ifspec;
	}
}
