using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbExecuteAsyncOperation : EmsmdbAsyncOperation
	{
		public EmsmdbExecuteAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.RequireSequence)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "Execute";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new EmsmdbExecuteRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			EmsmdbExecuteRequest request = (EmsmdbExecuteRequest)mapiHttpRequest;
			ArraySegment<byte> outputRopBuffer = base.AllocateBuffer((int)request.MaxOutputBufferSize, EmsmdbConstants.MaxChainedExtendedRopBufferSize);
			ArraySegment<byte> outputAuxiliaryBuffer = base.AllocateBuffer(4104, 4104);
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr emsmdbContextHandle = this.EmsmdbContextHandle;
				return EmsmdbHttpHandler.ExchangeAsyncDispatch.BeginExecute(this.ProtocolRequestInfo, emsmdbContextHandle, (int)request.Flags, request.RopBuffer, outputRopBuffer, request.AuxiliaryBuffer, outputAuxiliaryBuffer, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				IntPtr emsmdbContextHandle = this.EmsmdbContextHandle;
				returnCode = (uint)EmsmdbHttpHandler.ExchangeAsyncDispatch.EndExecute((ICancelableAsyncResult)asyncResult, out emsmdbContextHandle, out outputRopBuffer, out outputAuxiliaryBuffer);
				this.EmsmdbContextHandle = emsmdbContextHandle;
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new EmsmdbExecuteResponse(returnCode, 0U, outputRopBuffer, outputAuxiliaryBuffer));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EmsmdbExecuteAsyncOperation>(this);
		}
	}
}
