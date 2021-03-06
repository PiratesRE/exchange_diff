using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiUpdateStatAsyncOperation : NspiAsyncOperation
	{
		public NspiUpdateStatAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.RequireSequence)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "UpdateStat";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new NspiUpdateStatRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			NspiUpdateStatRequest request = (NspiUpdateStatRequest)mapiHttpRequest;
			NspiState localState = null;
			int? localDelta = null;
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr nspiContextHandle = this.NspiContextHandle;
				return NspiHttpHandler.NspiAsyncDispatch.BeginUpdateStat(this.ProtocolRequestInfo, nspiContextHandle, request.Flags, request.State, request.DeltaRequested, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				returnCode = (uint)NspiHttpHandler.NspiAsyncDispatch.EndUpdateStat((ICancelableAsyncResult)asyncResult, out localState, out localDelta);
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new NspiUpdateStatResponse(returnCode, localState, localDelta, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiUpdateStatAsyncOperation>(this);
		}
	}
}
