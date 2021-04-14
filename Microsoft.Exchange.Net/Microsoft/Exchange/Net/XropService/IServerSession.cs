using System;

namespace Microsoft.Exchange.Net.XropService
{
	internal interface IServerSession
	{
		IAsyncResult BeginConnect(ConnectRequest request, AsyncCallback asyncCallback, object asyncState);

		ConnectResponse EndConnect(IAsyncResult asyncResult);

		IAsyncResult BeginExecute(ExecuteRequest request, AsyncCallback asyncCallback, object asyncState);

		ExecuteResponse EndExecute(IAsyncResult asyncResult);

		IAsyncResult BeginDisconnect(DisconnectRequest request, AsyncCallback asyncCallback, object asyncState);

		DisconnectResponse EndDisconnect(IAsyncResult asyncResult);
	}
}
