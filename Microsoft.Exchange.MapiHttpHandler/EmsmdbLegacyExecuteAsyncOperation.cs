using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbLegacyExecuteAsyncOperation : EmsmdbAsyncOperation
	{
		public EmsmdbLegacyExecuteAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.RequireSequence)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "EcDoRpcExt2";
			}
		}

		public override void ParseRequest(WorkBuffer requestBuffer)
		{
			base.CheckDisposed();
			this.parameters = new ExecuteParams(requestBuffer);
		}

		public override async Task ExecuteAsync()
		{
			base.CheckDisposed();
			ArraySegment<byte> segmentExtendedRopOut = Array<byte>.EmptySegment;
			ArraySegment<byte> segmentExtendedAuxOut = Array<byte>.EmptySegment;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr emsmdbContextHandle = base.EmsmdbContextHandle;
				return EmsmdbHttpHandler.ExchangeAsyncDispatch.BeginExecute(base.ProtocolRequestInfo, emsmdbContextHandle, this.parameters.Flags, this.parameters.SegmentExtendedRopIn, this.parameters.SegmentExtendedRopOut, this.parameters.SegmentExtendedAuxIn, this.parameters.SegmentExtendedAuxOut, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				IntPtr emsmdbContextHandle = this.EmsmdbContextHandle;
				this.ErrorCode = EmsmdbHttpHandler.ExchangeAsyncDispatch.EndExecute((ICancelableAsyncResult)asyncResult, out emsmdbContextHandle, out segmentExtendedRopOut, out segmentExtendedAuxOut);
				this.EmsmdbContextHandle = emsmdbContextHandle;
			});
			base.StatusCode = callResult.StatusCode;
			if (base.StatusCode != 0U)
			{
				this.parameters.SetFailedResponse(base.StatusCode);
			}
			else
			{
				this.parameters.SetSuccessResponse(base.ErrorCode, segmentExtendedRopOut, segmentExtendedAuxOut);
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
			return DisposeTracker.Get<EmsmdbLegacyExecuteAsyncOperation>(this);
		}

		private ExecuteParams parameters;
	}
}
