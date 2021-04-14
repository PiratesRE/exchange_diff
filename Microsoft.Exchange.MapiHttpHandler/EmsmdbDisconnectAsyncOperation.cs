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
	internal sealed class EmsmdbDisconnectAsyncOperation : EmsmdbAsyncOperation
	{
		public EmsmdbDisconnectAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.RequireSequence | AsyncOperationCookieFlags.DestroySession)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "Disconnect";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new EmsmdbDisconnectRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr emsmdbContextHandle = base.EmsmdbContextHandle;
				return EmsmdbHttpHandler.ExchangeAsyncDispatch.BeginDisconnect(base.ProtocolRequestInfo, emsmdbContextHandle, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				IntPtr emsmdbContextHandle = this.EmsmdbContextHandle;
				returnCode = (uint)EmsmdbHttpHandler.ExchangeAsyncDispatch.EndDisconnect((ICancelableAsyncResult)asyncResult, out emsmdbContextHandle);
				this.EmsmdbContextHandle = emsmdbContextHandle;
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new EmsmdbDisconnectResponse(returnCode, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EmsmdbDisconnectAsyncOperation>(this);
		}
	}
}
