using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.MapiHttp;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RfriHttpClient : MapiHttpClient, IRfriAsyncDispatch
	{
		public RfriHttpClient(MapiHttpBindingInfo bindingInfo) : base(bindingInfo)
		{
		}

		internal override string VdirPath
		{
			get
			{
				return MapiHttpEndpoints.VdirPathNspi;
			}
		}

		public ICancelableAsyncResult BeginGetAddressBookUrl(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetAddressBookUrlFlags flags, string hostname, string userDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			IntPtr contextHandle = base.CreateNewContextHandle(null);
			return base.BeginWrapper<RfriGetAddressBookUrlClientAsyncOperation>(contextHandle, true, delegate(ClientSessionContext context)
			{
				RfriGetAddressBookUrlClientAsyncOperation rfriGetAddressBookUrlClientAsyncOperation = new RfriGetAddressBookUrlClientAsyncOperation(context, asyncCallback, asyncState);
				rfriGetAddressBookUrlClientAsyncOperation.Begin(new RfriGetAddressBookUrlRequest(flags, userDn, Array<byte>.EmptySegment));
				return rfriGetAddressBookUrlClientAsyncOperation;
			});
		}

		public RfriStatus EndGetAddressBookUrl(ICancelableAsyncResult asyncResult, out string serverUrl)
		{
			RfriGetAddressBookUrlResponse getAddressBookUrlResponse = null;
			IntPtr intPtr;
			ErrorCode result = base.EndWrapper<RfriGetAddressBookUrlClientAsyncOperation>(asyncResult, true, true, out intPtr, (RfriGetAddressBookUrlClientAsyncOperation operation) => operation.End(out getAddressBookUrlResponse));
			serverUrl = getAddressBookUrlResponse.ServerUrl;
			return (RfriStatus)result;
		}

		public ICancelableAsyncResult BeginGetMailboxUrl(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetMailboxUrlFlags flags, string hostname, string serverDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			IntPtr contextHandle = base.CreateNewContextHandle(null);
			return base.BeginWrapper<RfriGetMailboxUrlClientAsyncOperation>(contextHandle, true, delegate(ClientSessionContext context)
			{
				RfriGetMailboxUrlClientAsyncOperation rfriGetMailboxUrlClientAsyncOperation = new RfriGetMailboxUrlClientAsyncOperation(context, asyncCallback, asyncState);
				rfriGetMailboxUrlClientAsyncOperation.Begin(new RfriGetMailboxUrlRequest(flags, serverDn, Array<byte>.EmptySegment));
				return rfriGetMailboxUrlClientAsyncOperation;
			});
		}

		public RfriStatus EndGetMailboxUrl(ICancelableAsyncResult asyncResult, out string serverUrl)
		{
			RfriGetMailboxUrlResponse getMailboxUrlResponse = null;
			IntPtr intPtr;
			ErrorCode result = base.EndWrapper<RfriGetMailboxUrlClientAsyncOperation>(asyncResult, true, true, out intPtr, (RfriGetMailboxUrlClientAsyncOperation operation) => operation.End(out getMailboxUrlResponse));
			serverUrl = getMailboxUrlResponse.ServerUrl;
			return (RfriStatus)result;
		}

		public ICancelableAsyncResult BeginGetNewDSA(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetNewDSAFlags flags, string userDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotSupportedException();
		}

		public RfriStatus EndGetNewDSA(ICancelableAsyncResult asyncResult, out string serverUrl)
		{
			throw new NotSupportedException();
		}

		public ICancelableAsyncResult BeginGetFQDNFromLegacyDN(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetFQDNFromLegacyDNFlags flags, string serverDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotSupportedException();
		}

		public RfriStatus EndGetFQDNFromLegacyDN(ICancelableAsyncResult asyncResult, out string serverUrl)
		{
			throw new NotSupportedException();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriHttpClient>(this);
		}
	}
}
