using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.Rpc.RfriClient
{
	internal class RfriAsyncRpcClient : RpcClientBase, IRfriAsyncDispatch
	{
		public RfriAsyncRpcClient(RpcBindingInfo bindingInfo) : base(bindingInfo.UseKerberosSpn("exchangeRFR", null))
		{
		}

		public virtual ICancelableAsyncResult BeginGetNewDSA(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetNewDSAFlags flags, string userDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_GetNewDSA clientAsyncCallState_GetNewDSA = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				IntPtr bindingHandle = (IntPtr)base.BindingHandle;
				clientAsyncCallState_GetNewDSA = new ClientAsyncCallState_GetNewDSA(asyncCallback, asyncState, bindingHandle, flags, userDn);
				clientAsyncCallState_GetNewDSA.Begin();
				flag = true;
				result = clientAsyncCallState_GetNewDSA;
			}
			finally
			{
				if (!flag && clientAsyncCallState_GetNewDSA != null)
				{
					((IDisposable)clientAsyncCallState_GetNewDSA).Dispose();
				}
			}
			return result;
		}

		public virtual RfriStatus EndGetNewDSA(ICancelableAsyncResult asyncResult, out string server)
		{
			RfriStatus result;
			using (ClientAsyncCallState_GetNewDSA clientAsyncCallState_GetNewDSA = (ClientAsyncCallState_GetNewDSA)asyncResult)
			{
				result = clientAsyncCallState_GetNewDSA.End(out server);
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginGetFQDNFromLegacyDN(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetFQDNFromLegacyDNFlags flags, string serverDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			ClientAsyncCallState_GetFQDNFromLegacyDN clientAsyncCallState_GetFQDNFromLegacyDN = null;
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				IntPtr bindingHandle = (IntPtr)base.BindingHandle;
				clientAsyncCallState_GetFQDNFromLegacyDN = new ClientAsyncCallState_GetFQDNFromLegacyDN(asyncCallback, asyncState, bindingHandle, flags, serverDn);
				clientAsyncCallState_GetFQDNFromLegacyDN.Begin();
				flag = true;
				result = clientAsyncCallState_GetFQDNFromLegacyDN;
			}
			finally
			{
				if (!flag && clientAsyncCallState_GetFQDNFromLegacyDN != null)
				{
					((IDisposable)clientAsyncCallState_GetFQDNFromLegacyDN).Dispose();
				}
			}
			return result;
		}

		public virtual RfriStatus EndGetFQDNFromLegacyDN(ICancelableAsyncResult asyncResult, out string serverFqdn)
		{
			RfriStatus result;
			using (ClientAsyncCallState_GetFQDNFromLegacyDN clientAsyncCallState_GetFQDNFromLegacyDN = (ClientAsyncCallState_GetFQDNFromLegacyDN)asyncResult)
			{
				result = clientAsyncCallState_GetFQDNFromLegacyDN.End(out serverFqdn);
			}
			return result;
		}

		public virtual ICancelableAsyncResult BeginGetAddressBookUrl(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetAddressBookUrlFlags flags, string hostname, string userDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotSupportedException();
		}

		public virtual RfriStatus EndGetAddressBookUrl(ICancelableAsyncResult asyncResult, out string url)
		{
			throw new NotSupportedException();
		}

		public virtual ICancelableAsyncResult BeginGetMailboxUrl(ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding, RfriGetMailboxUrlFlags flags, string hostname, string serverDn, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			throw new NotSupportedException();
		}

		public virtual RfriStatus EndGetMailboxUrl(ICancelableAsyncResult asyncResult, out string url)
		{
			throw new NotSupportedException();
		}
	}
}
