using System;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal interface IEmsmdbClient : IRpcClient, IDisposable
	{
		IAsyncResult BeginConnect(string userDn, TimeSpan timeout, bool useMonitoringContext, AsyncCallback asyncCallback, object asyncState);

		ConnectCallResult EndConnect(IAsyncResult asyncResult);

		IAsyncResult BeginLogon(string mailboxDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		IAsyncResult BeginPublicLogon(string mailboxDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		LogonCallResult EndLogon(IAsyncResult asyncResult);

		LogonCallResult EndPublicLogon(IAsyncResult asyncResult);

		IAsyncResult BeginDummy(TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		DummyCallResult EndDummy(IAsyncResult asyncResult);

		string GetConnectionUriString();
	}
}
