using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class NotificationWaitDispatchTask : ExchangeDispatchTask
	{
		public NotificationWaitDispatchTask(IExchangeDispatch exchangeDispatch, CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, IntPtr notificationContextHandle, int flagsIn) : base("NotificationWaitDispatchTask", exchangeDispatch, protocolRequestInfo, asyncCallback, asyncState)
		{
			this.notificationContextHandle = notificationContextHandle;
			this.flagsIn = flagsIn;
			this.flagsOut = 1;
		}

		internal override IntPtr ContextHandle
		{
			get
			{
				return this.notificationContextHandle;
			}
		}

		internal override int? InternalExecute()
		{
			base.ExchangeDispatch.NotificationWait(base.ProtocolRequestInfo, this.notificationContextHandle, this.flagsIn, delegate(bool notificationsAvailable, int errorCode)
			{
				if (notificationsAvailable)
				{
					this.flagsOut = 1;
				}
				else
				{
					this.flagsOut = 0;
				}
				base.Completion(null, errorCode);
			});
			return null;
		}

		public int End(out int flagsOut)
		{
			int result = base.CheckCompletion();
			flagsOut = this.flagsOut;
			return result;
		}

		private readonly IntPtr notificationContextHandle;

		private readonly int flagsIn;

		private int flagsOut;
	}
}
