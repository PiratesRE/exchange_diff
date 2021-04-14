using System;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal abstract class ExchangeAsyncRpcServer : BaseAsyncRpcServer<Microsoft::Exchange::Rpc::IExchangeAsyncDispatch>
	{
		public override void DroppedConnection(IntPtr contextHandle)
		{
			IExchangeAsyncDispatch asyncDispatch = this.GetAsyncDispatch();
			if (asyncDispatch != null)
			{
				asyncDispatch.DroppedConnection(contextHandle);
			}
		}

		public ExchangeAsyncRpcServer()
		{
		}
	}
}
