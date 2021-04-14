using System;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal interface IRfriClient : IRpcClient, IDisposable
	{
		IAsyncResult BeginGetNewDsa(string userLegacyDN, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		RfriCallResult EndGetNewDsa(IAsyncResult asyncResult, out string server);

		IAsyncResult BeginGetFqdnFromLegacyDn(string serverDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		RfriCallResult EndGetFqdnFromLegacyDn(IAsyncResult asyncResult, out string serverFqdn);
	}
}
