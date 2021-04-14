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
	internal class RfriGetMailboxUrlAsyncOperation : RfriSecurityContextAsyncOperation
	{
		public RfriGetMailboxUrlAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.None)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "GetMailboxUrl";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new RfriGetMailboxUrlRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			RfriGetMailboxUrlRequest request = (RfriGetMailboxUrlRequest)mapiHttpRequest;
			string serverUrl = null;
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync((AsyncCallback asyncCallback, object asyncState) => NspiHttpHandler.RfriAsyncDispatch.BeginGetMailboxUrl(this.ProtocolRequestInfo, this.ClientBinding, request.Flags, this.Context.GetOriginalHost(), request.ServerDn, delegate(ICancelableAsyncResult cancelableAsyncResult)
			{
				asyncCallback(cancelableAsyncResult);
			}, asyncState), delegate(IAsyncResult asyncResult)
			{
				returnCode = (uint)NspiHttpHandler.RfriAsyncDispatch.EndGetMailboxUrl((ICancelableAsyncResult)asyncResult, out serverUrl);
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new RfriGetMailboxUrlResponse(returnCode, serverUrl ?? string.Empty, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriGetMailboxUrlAsyncOperation>(this);
		}
	}
}
