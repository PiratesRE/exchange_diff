using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class UnregisterPushNotificationDispatchTask : ExchangeDispatchTask
	{
		public UnregisterPushNotificationDispatchTask(IExchangeDispatch exchangeDispatch, CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, int notificationHandle) : base("UnregisterPushNotificationDispatchTask", exchangeDispatch, protocolRequestInfo, asyncCallback, asyncState)
		{
			this.contextHandleIn = contextHandle;
			this.contextHandleOut = contextHandle;
			this.notificationHandle = notificationHandle;
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
			return new int?(base.ExchangeDispatch.UnregisterPushNotification(base.ProtocolRequestInfo, ref this.contextHandleOut, this.notificationHandle));
		}

		public int End(out IntPtr contextHandle)
		{
			int result = base.CheckCompletion();
			contextHandle = this.contextHandleOut;
			return result;
		}

		private readonly int notificationHandle;

		private IntPtr contextHandleIn;

		private IntPtr contextHandleOut;
	}
}
