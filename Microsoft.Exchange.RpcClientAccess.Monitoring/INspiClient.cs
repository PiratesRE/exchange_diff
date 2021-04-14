using System;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal interface INspiClient : IRpcClient, IDisposable
	{
		IAsyncResult BeginBind(TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		NspiCallResult EndBind(IAsyncResult asyncResult);

		IAsyncResult BeginUnbind(TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		NspiCallResult EndUnbind(IAsyncResult asyncResult);

		IAsyncResult BeginGetHierarchyInfo(TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		NspiCallResult EndGetHierarchyInfo(IAsyncResult asyncResult, out int version);

		IAsyncResult BeginGetMatches(string primarySmtpAddress, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		NspiCallResult EndGetMatches(IAsyncResult asyncResult, out int[] minimalIds);

		IAsyncResult BeginQueryRows(int[] minimalIds, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		NspiCallResult EndQueryRows(IAsyncResult asyncResult, out string homeMDB, out string userLegacyDN);

		IAsyncResult BeginDNToEph(string serverLegacyDn, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		NspiCallResult EndDNToEph(IAsyncResult asyncResult, out int[] minimalIds);

		IAsyncResult BeginGetNetworkAddresses(int[] minimalIds, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState);

		NspiCallResult EndGetNetworkAddresses(IAsyncResult asyncResult, out string[] networkAddresses);
	}
}
