using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class RegisterPushNotificationDispatchTask : ExchangeDispatchTask
	{
		public RegisterPushNotificationDispatchTask(IExchangeDispatch exchangeDispatch, CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, ArraySegment<byte> segmentContext, int adviseBits, ArraySegment<byte> segmentClientBlob) : base("RegisterPushNotificationDispatchTask", exchangeDispatch, protocolRequestInfo, asyncCallback, asyncState)
		{
			this.contextHandleIn = contextHandle;
			this.contextHandleOut = contextHandle;
			this.segmentContext = segmentContext;
			this.adviseBits = adviseBits;
			this.segmentClientBlob = segmentClientBlob;
		}

		internal override IntPtr ContextHandle
		{
			get
			{
				return this.contextHandleIn;
			}
		}

		internal override int? InternalExecute()
		{
			return new int?(base.ExchangeDispatch.RegisterPushNotification(base.ProtocolRequestInfo, ref this.contextHandleOut, this.segmentContext, this.adviseBits, this.segmentClientBlob, out this.notificationHandle));
		}

		public int End(out IntPtr contextHandle, out int notificationHandle)
		{
			int result = base.CheckCompletion();
			contextHandle = this.contextHandleOut;
			notificationHandle = this.notificationHandle;
			return result;
		}

		private ArraySegment<byte> segmentContext;

		private int adviseBits;

		private ArraySegment<byte> segmentClientBlob;

		private IntPtr contextHandleIn;

		private IntPtr contextHandleOut;

		private int notificationHandle;
	}
}
