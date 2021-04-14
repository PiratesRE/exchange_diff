using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.Rpc
{
	internal interface IRfriAsyncDispatch
	{
		ICancelableAsyncResult BeginGetNewDSA(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetNewDSAFlags flags, string userDn, CancelableAsyncCallback asyncCallback, object asyncState);

		RfriStatus EndGetNewDSA(ICancelableAsyncResult asyncResult, out string server);

		ICancelableAsyncResult BeginGetFQDNFromLegacyDN(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetFQDNFromLegacyDNFlags flags, string serverDn, CancelableAsyncCallback asyncCallback, object asyncState);

		RfriStatus EndGetFQDNFromLegacyDN(ICancelableAsyncResult asyncResult, out string serverFqdn);

		ICancelableAsyncResult BeginGetAddressBookUrl(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetAddressBookUrlFlags flags, string hostname, string userDn, CancelableAsyncCallback asyncCallback, object asyncState);

		RfriStatus EndGetAddressBookUrl(ICancelableAsyncResult asyncResult, out string url);

		ICancelableAsyncResult BeginGetMailboxUrl(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetMailboxUrlFlags flags, string hostname, string serverDn, CancelableAsyncCallback asyncCallback, object asyncState);

		RfriStatus EndGetMailboxUrl(ICancelableAsyncResult asyncResult, out string url);
	}
}
