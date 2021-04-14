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
	internal sealed class EmsmdbLegacyConnectAsyncOperation : EmsmdbSecurityContextAsyncOperation
	{
		public EmsmdbLegacyConnectAsyncOperation(HttpContextBase context) : base(context, AsyncOperationCookieFlags.AllowSession | AsyncOperationCookieFlags.CreateSession)
		{
		}

		public override string RequestType
		{
			get
			{
				base.CheckDisposed();
				return "EcDoConnectEx";
			}
		}

		public override string GetTraceBeginParameters(MapiHttpRequest mapiHttpRequest)
		{
			base.CheckDisposed();
			return AsyncOperation.CombineTraceParameters(base.GetTraceBeginParameters(mapiHttpRequest), string.Format("UserDn={0}", this.parameters.UserDn));
		}

		public override void ParseRequest(WorkBuffer requestBuffer)
		{
			base.CheckDisposed();
			this.parameters = new ConnectParams(requestBuffer);
		}

		public override async Task ExecuteAsync()
		{
			base.CheckDisposed();
			TimeSpan pollsMax = TimeSpan.MinValue;
			int retryCount = 0;
			TimeSpan retryDelay = TimeSpan.MinValue;
			string dnPrefix = string.Empty;
			string displayName = string.Empty;
			short[] serverVersion = null;
			ArraySegment<byte> segmentExtendedAuxOut = Array<byte>.EmptySegment;
			MapiHttpDispatchedCallResult callResult = await MapiHttpHandler.DispatchCallAsync((AsyncCallback asyncCallback, object asyncState) => EmsmdbHttpHandler.ExchangeAsyncDispatch.BeginConnect(base.ProtocolRequestInfo, base.ClientBinding, this.parameters.UserDn, this.parameters.Flags, this.parameters.ConnectionModulus, this.parameters.CodePage, this.parameters.StringLcid, this.parameters.SortLcid, this.parameters.ClientVersion, this.parameters.SegmentExtendedAuxIn, this.parameters.SegmentExtendedAuxOut, delegate(ICancelableAsyncResult cancelableAsyncResult)
			{
				asyncCallback(cancelableAsyncResult);
			}, asyncState), delegate(IAsyncResult asyncResult)
			{
				IntPtr zero = IntPtr.Zero;
				this.ErrorCode = EmsmdbHttpHandler.ExchangeAsyncDispatch.EndConnect((ICancelableAsyncResult)asyncResult, out zero, out pollsMax, out retryCount, out retryDelay, out dnPrefix, out displayName, out serverVersion, out segmentExtendedAuxOut);
				this.EmsmdbContextHandle = zero;
			});
			base.StatusCode = callResult.StatusCode;
			if (base.StatusCode != 0U)
			{
				this.parameters.SetFailedResponse(base.StatusCode);
			}
			else
			{
				this.parameters.SetSuccessResponse(base.ErrorCode, pollsMax, retryCount, retryDelay, dnPrefix, displayName, serverVersion, segmentExtendedAuxOut);
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
			return DisposeTracker.Get<EmsmdbLegacyConnectAsyncOperation>(this);
		}

		private ConnectParams parameters;
	}
}
