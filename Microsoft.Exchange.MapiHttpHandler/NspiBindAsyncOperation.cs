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
	internal sealed class NspiBindAsyncOperation : NspiSecurityContextAsyncOperation
	{
		public NspiBindAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.CreateSession)
		{
			context.SetTunnelExpirationTime(MapiHttpHandler.ClientTunnelExpirationTime);
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "Bind";
			}
		}

		protected override MapiHttpRequest InternalParseRequest(Reader reader)
		{
			return new NspiBindRequest(reader);
		}

		protected override async Task<MapiHttpResponse> InternalExecuteAsync(MapiHttpRequest mapiHttpRequest)
		{
			NspiBindRequest request = (NspiBindRequest)mapiHttpRequest;
			Guid? serverGuid = new Guid?(default(Guid));
			uint returnCode = 0U;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync((AsyncCallback asyncCallback, object asyncState) => NspiHttpHandler.NspiAsyncDispatch.BeginBind(this.ProtocolRequestInfo, this.ClientBinding, request.Flags, request.State, null, delegate(ICancelableAsyncResult cancelableAsyncResult)
			{
				asyncCallback(cancelableAsyncResult);
			}, asyncState), delegate(IAsyncResult asyncResult)
			{
				IntPtr zero = IntPtr.Zero;
				returnCode = (uint)NspiHttpHandler.NspiAsyncDispatch.EndBind((ICancelableAsyncResult)asyncResult, out serverGuid, out zero);
				this.NspiContextHandle = zero;
			});
			return callResult.CreateResponse(new Func<ArraySegment<byte>>(base.AllocateErrorBuffer), delegate
			{
				if (serverGuid == null)
				{
					throw ProtocolException.FromResponseCode((LID)35932, "Server guid cannot be null on success", ResponseCode.InvalidResponse, null);
				}
				return new NspiBindResponse(returnCode, serverGuid.Value, Array<byte>.EmptySegment);
			});
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiBindAsyncOperation>(this);
		}
	}
}
