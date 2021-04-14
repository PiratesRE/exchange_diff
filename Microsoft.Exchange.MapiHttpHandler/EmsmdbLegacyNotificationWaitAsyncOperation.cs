using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbLegacyNotificationWaitAsyncOperation : EmsmdbAsyncOperation
	{
		public EmsmdbLegacyNotificationWaitAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.AllowInvalidSession)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "EcDoAsyncWaitEx";
			}
		}

		public override void ParseRequest(WorkBuffer requestBuffer)
		{
			base.CheckDisposed();
			this.parameters = new NotificationWaitParams(requestBuffer);
		}

		public override async Task ExecuteAsync()
		{
			base.CheckDisposed();
			int flags = 0;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr emsmdbContextHandle = base.EmsmdbContextHandle;
				return EmsmdbHttpHandler.ExchangeAsyncDispatch.BeginNotificationWait(base.ProtocolRequestInfo, emsmdbContextHandle, this.parameters.Flags, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				this.ErrorCode = EmsmdbHttpHandler.ExchangeAsyncDispatch.EndNotificationWait((ICancelableAsyncResult)asyncResult, out flags);
			});
			base.StatusCode = callResult.StatusCode;
			if (base.StatusCode != 0U)
			{
				this.parameters.SetFailedResponse(base.StatusCode);
			}
			else
			{
				this.parameters.SetSuccessResponse(base.ErrorCode, flags);
			}
		}

		public override void SerializeResponse(out WorkBuffer[] responseBuffers)
		{
			base.CheckDisposed();
			responseBuffers = this.parameters.Serialize();
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.parameters);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EmsmdbLegacyNotificationWaitAsyncOperation>(this);
		}

		private NotificationWaitParams parameters;
	}
}
