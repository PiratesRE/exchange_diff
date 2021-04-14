using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class DummyDispatchTask : ExchangeDispatchTask
	{
		public DummyDispatchTask(IExchangeDispatch exchangeDispatch, CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, ClientBinding clientBinding) : base("DummyDispatchTask", exchangeDispatch, protocolRequestInfo, asyncCallback, asyncState)
		{
			this.clientBinding = clientBinding;
		}

		internal override int? InternalExecute()
		{
			return new int?(base.ExchangeDispatch.Dummy(base.ProtocolRequestInfo, this.clientBinding));
		}

		internal override IntPtr ContextHandle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		public int End()
		{
			return base.CheckCompletion();
		}

		private readonly ClientBinding clientBinding;
	}
}
