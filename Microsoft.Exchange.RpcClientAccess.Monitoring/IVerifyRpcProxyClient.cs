using System;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal interface IVerifyRpcProxyClient
	{
		IAsyncResult BeginVerifyRpcProxy(bool makeHangingRequest, AsyncCallback asyncCallback, object asyncState);

		VerifyRpcProxyResult EndVerifyRpcProxy(IAsyncResult asyncResult);
	}
}
