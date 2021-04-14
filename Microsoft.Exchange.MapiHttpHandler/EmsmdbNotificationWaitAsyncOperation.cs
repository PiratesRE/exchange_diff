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
	internal sealed class EmsmdbNotificationWaitAsyncOperation : EmsmdbAsyncOperation
	{
		public EmsmdbNotificationWaitAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.AllowInvalidSession)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "NotificationWait";
			}
		}

		public override TimeSpan InitialFlushPeriod
		{
			get
			{
				return TimeSpan.Zero;
			}
		}

		public override object DroppedConnectionContextHandle
		{
			get
			{
				base.CheckDisposed();
				return base.ContextHandle;
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new EmsmdbNotificationWaitRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			int flags = 0;
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr emsmdbContextHandle = base.EmsmdbContextHandle;
				return EmsmdbHttpHandler.ExchangeAsyncDispatch.BeginNotificationWait(base.ProtocolRequestInfo, emsmdbContextHandle, 0, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				returnCode = (uint)EmsmdbHttpHandler.ExchangeAsyncDispatch.EndNotificationWait((ICancelableAsyncResult)asyncResult, out flags);
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new EmsmdbNotificationWaitResponse(returnCode, (uint)flags, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EmsmdbNotificationWaitAsyncOperation>(this);
		}
	}
}
