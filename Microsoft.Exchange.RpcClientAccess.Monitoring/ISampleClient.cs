using System;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal interface ISampleClient : IDisposable
	{
		IAsyncResult BeginSomeOperation(AsyncCallback asyncCallback, object asyncState);

		int EndSomeOperation(IAsyncResult asyncResult);
	}
}
