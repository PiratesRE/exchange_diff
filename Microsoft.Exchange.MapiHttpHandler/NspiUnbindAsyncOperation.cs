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
	internal sealed class NspiUnbindAsyncOperation : NspiAsyncOperation
	{
		public NspiUnbindAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.RequireSequence | AsyncOperationCookieFlags.DestroySession)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "Unbind";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new NspiUnbindRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			NspiUnbindRequest request = (NspiUnbindRequest)mapiHttpRequest;
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr nspiContextHandle = this.NspiContextHandle;
				return NspiHttpHandler.NspiAsyncDispatch.BeginUnbind(this.ProtocolRequestInfo, nspiContextHandle, request.Flags, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				IntPtr nspiContextHandle = this.NspiContextHandle;
				returnCode = (uint)NspiHttpHandler.NspiAsyncDispatch.EndUnbind((ICancelableAsyncResult)asyncResult, out nspiContextHandle);
				this.NspiContextHandle = nspiContextHandle;
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new NspiUnbindResponse(returnCode, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiUnbindAsyncOperation>(this);
		}
	}
}
