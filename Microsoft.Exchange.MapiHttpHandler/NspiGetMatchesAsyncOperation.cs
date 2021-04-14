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
	internal sealed class NspiGetMatchesAsyncOperation : NspiAsyncOperation
	{
		public NspiGetMatchesAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.RequireSequence)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "GetMatches";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new NspiGetMatchesRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			NspiGetMatchesRequest request = (NspiGetMatchesRequest)mapiHttpRequest;
			NspiState state = null;
			int[] mids = null;
			PropertyValue[][] rowSet = null;
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync((AsyncCallback asyncCallback, object asyncState) => NspiHttpHandler.NspiAsyncDispatch.BeginGetMatches(this.ProtocolRequestInfo, this.NspiContextHandle, request.Flags, request.State, request.InputEphemeralIds, (int)request.InterfaceOptions, request.Restriction, IntPtr.Zero, (int)request.MaxCount, request.Columns, delegate(ICancelableAsyncResult cancelableAsyncResult)
			{
				asyncCallback(cancelableAsyncResult);
			}, asyncState), delegate(IAsyncResult asyncResult)
			{
				returnCode = (uint)NspiHttpHandler.NspiAsyncDispatch.EndGetMatches((ICancelableAsyncResult)asyncResult, out state, out mids, out rowSet);
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new NspiGetMatchesResponse(returnCode, state, mids, rowSet.GetColumns(), rowSet, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiGetMatchesAsyncOperation>(this);
		}
	}
}
