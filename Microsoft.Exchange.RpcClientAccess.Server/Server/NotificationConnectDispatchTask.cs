using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class NotificationConnectDispatchTask : ExchangeDispatchTask
	{
		public NotificationConnectDispatchTask(IExchangeDispatch exchangeDispatch, CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle) : base("NotificationConnectDispatchTask", exchangeDispatch, protocolRequestInfo, asyncCallback, asyncState)
		{
			this.contextHandle = contextHandle;
		}

		internal override IntPtr ContextHandle
		{
			get
			{
				return this.contextHandle;
			}
		}

		internal override int? InternalExecute()
		{
			return new int?(base.ExchangeDispatch.NotificationConnect(base.ProtocolRequestInfo, this.contextHandle, out this.notificationContextHandle));
		}

		public int End(out IntPtr notificationContextHandle)
		{
			int result = base.CheckCompletion();
			notificationContextHandle = this.notificationContextHandle;
			return result;
		}

		private readonly IntPtr contextHandle;

		private IntPtr notificationContextHandle;
	}
}
