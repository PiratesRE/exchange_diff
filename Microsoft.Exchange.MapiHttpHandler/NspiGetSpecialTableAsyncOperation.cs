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
	internal sealed class NspiGetSpecialTableAsyncOperation : NspiAsyncOperation
	{
		public NspiGetSpecialTableAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.RequireSession | AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.RequireSequence)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "GetSpecialTable";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new NspiGetSpecialTableRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			NspiGetSpecialTableRequest request = (NspiGetSpecialTableRequest)mapiHttpRequest;
			int localCodePage = 0;
			int localVersion = 0;
			PropertyValue[][] localRowset = null;
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync(delegate(AsyncCallback asyncCallback, object asyncState)
			{
				IntPtr nspiContextHandle = this.NspiContextHandle;
				return NspiHttpHandler.NspiAsyncDispatch.BeginGetHierarchyInfo(this.ProtocolRequestInfo, nspiContextHandle, request.Flags, request.State, (int)request.Version.Value, delegate(ICancelableAsyncResult cancelableAsyncResult)
				{
					asyncCallback(cancelableAsyncResult);
				}, asyncState);
			}, delegate(IAsyncResult asyncResult)
			{
				returnCode = (uint)NspiHttpHandler.NspiAsyncDispatch.EndGetHierarchyInfo((ICancelableAsyncResult)asyncResult, out localCodePage, out localVersion, out localRowset);
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new NspiGetSpecialTableResponse(returnCode, (uint)localCodePage, new uint?((uint)localVersion), localRowset, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiGetSpecialTableAsyncOperation>(this);
		}
	}
}
