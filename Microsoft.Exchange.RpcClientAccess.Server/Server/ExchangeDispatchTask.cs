using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal abstract class ExchangeDispatchTask : DispatchTask
	{
		public ExchangeDispatchTask(string description, IExchangeDispatch exchangeDispatch, ProtocolRequestInfo protocolRequestInfo, CancelableAsyncCallback asyncCallback, object asyncState) : base(description, asyncCallback, asyncState)
		{
			this.exchangeDispatch = exchangeDispatch;
			this.protocolRequestInfo = protocolRequestInfo;
		}

		internal IExchangeDispatch ExchangeDispatch
		{
			get
			{
				return this.exchangeDispatch;
			}
		}

		internal ProtocolRequestInfo ProtocolRequestInfo
		{
			get
			{
				return this.protocolRequestInfo;
			}
		}

		private readonly IExchangeDispatch exchangeDispatch;

		private readonly ProtocolRequestInfo protocolRequestInfo;
	}
}
