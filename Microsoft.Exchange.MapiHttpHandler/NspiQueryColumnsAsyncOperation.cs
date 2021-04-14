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
	internal sealed class NspiQueryColumnsAsyncOperation : NspiAsyncOperation
	{
		public NspiQueryColumnsAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.RequireSequence)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "QueryColumns";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new NspiQueryColumnsRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			NspiQueryColumnsRequest request = (NspiQueryColumnsRequest)mapiHttpRequest;
			PropertyTag[] localColumns = null;
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr nspiContextHandle = this.NspiContextHandle;
				return NspiHttpHandler.NspiAsyncDispatch.BeginQueryColumns(this.ProtocolRequestInfo, nspiContextHandle, request.Flags, request.MapiFlags, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				returnCode = (uint)NspiHttpHandler.NspiAsyncDispatch.EndQueryColumns((ICancelableAsyncResult)asyncResult, out localColumns);
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new NspiQueryColumnsResponse(returnCode, localColumns, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiQueryColumnsAsyncOperation>(this);
		}
	}
}
