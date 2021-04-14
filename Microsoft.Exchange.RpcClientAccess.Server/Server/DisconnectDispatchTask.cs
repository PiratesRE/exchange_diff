using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class DisconnectDispatchTask : ExchangeDispatchTask
	{
		public DisconnectDispatchTask(IExchangeDispatch exchangeDispatch, CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle) : base("DisconnectDispatchTask", exchangeDispatch, protocolRequestInfo, asyncCallback, asyncState)
		{
			this.contextHandleIn = contextHandle;
			this.contextHandleOut = contextHandle;
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
			return new int?(base.ExchangeDispatch.Disconnect(base.ProtocolRequestInfo, ref this.contextHandleOut, false));
		}

		public int End(out IntPtr contextHandle)
		{
			int result = base.CheckCompletion();
			contextHandle = this.contextHandleOut;
			return result;
		}

		private IntPtr contextHandleIn;

		private IntPtr contextHandleOut;
	}
}
