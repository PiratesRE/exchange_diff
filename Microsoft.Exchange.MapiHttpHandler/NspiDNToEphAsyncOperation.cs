using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiDNToEphAsyncOperation : NspiAsyncOperation
	{
		public NspiDNToEphAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.RequireSequence)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "DNToMId";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new NspiDnToEphRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			NspiDnToEphRequest request = (NspiDnToEphRequest)mapiHttpRequest;
			int[] localMids = null;
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr nspiContextHandle = this.NspiContextHandle;
				return NspiHttpHandler.NspiAsyncDispatch.BeginDNToEph(this.ProtocolRequestInfo, nspiContextHandle, request.Flags, request.DistinguishedNames, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				returnCode = (uint)NspiHttpHandler.NspiAsyncDispatch.EndDNToEph((ICancelableAsyncResult)asyncResult, out localMids);
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new NspiDnToEphResponse(returnCode, localMids, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiDNToEphAsyncOperation>(this);
		}
	}
}
