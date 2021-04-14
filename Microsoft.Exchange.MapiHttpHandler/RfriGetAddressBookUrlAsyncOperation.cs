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
	internal class RfriGetAddressBookUrlAsyncOperation : RfriSecurityContextAsyncOperation
	{
		public RfriGetAddressBookUrlAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.None)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "GetNspiUrl";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new RfriGetAddressBookUrlRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			RfriGetAddressBookUrlRequest request = (RfriGetAddressBookUrlRequest)mapiHttpRequest;
			string serverUrl = null;
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync((AsyncCallback asyncCallback, object asyncState) => NspiHttpHandler.RfriAsyncDispatch.BeginGetAddressBookUrl(this.ProtocolRequestInfo, this.ClientBinding, request.Flags, this.Context.GetOriginalHost(), request.UserDn, delegate(ICancelableAsyncResult cancelableAsyncResult)
			{
				asyncCallback(cancelableAsyncResult);
			}, asyncState), delegate(IAsyncResult asyncResult)
			{
				returnCode = (uint)NspiHttpHandler.RfriAsyncDispatch.EndGetAddressBookUrl((ICancelableAsyncResult)asyncResult, out serverUrl);
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new RfriGetAddressBookUrlResponse(returnCode, serverUrl ?? string.Empty, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriGetAddressBookUrlAsyncOperation>(this);
		}
	}
}
