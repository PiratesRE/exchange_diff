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
	internal sealed class EmsmdbDummyAsyncOperation : EmsmdbSecurityContextAsyncOperation
	{
		public EmsmdbDummyAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.None)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "Dummy";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new EmsmdbDummyRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync((AsyncCallback asyncCallback, object asyncState) => EmsmdbHttpHandler.ExchangeAsyncDispatch.BeginDummy(base.ProtocolRequestInfo, base.ClientBinding, delegate(ICancelableAsyncResult cancelableAsyncResult)
			{
				asyncCallback(cancelableAsyncResult);
			}, asyncState), delegate(IAsyncResult asyncResult)
			{
				returnCode = (uint)EmsmdbHttpHandler.ExchangeAsyncDispatch.EndDummy((ICancelableAsyncResult)asyncResult);
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), () => new EmsmdbDummyResponse(returnCode, Array<byte>.EmptySegment));
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EmsmdbDummyAsyncOperation>(this);
		}
	}
}
