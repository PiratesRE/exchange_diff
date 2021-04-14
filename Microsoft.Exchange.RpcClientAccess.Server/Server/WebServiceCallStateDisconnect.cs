using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.XropService;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class WebServiceCallStateDisconnect : WebServiceCallState<DisconnectRequest, DisconnectResponse>
	{
		public WebServiceCallStateDisconnect(WebServiceUserInformation userInformation, IExchangeAsyncDispatch exchangeAsyncDispatch, AsyncCallback asyncCallback, object asyncState, IntPtr contextHandle) : base(userInformation, exchangeAsyncDispatch, asyncCallback, asyncState)
		{
			this.contextHandle = contextHandle;
		}

		internal IntPtr ContextHandle
		{
			get
			{
				return this.contextHandle;
			}
		}

		protected override string Name
		{
			get
			{
				return "Disconnect";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ConnectXropTracer;
			}
		}

		protected override void InternalBegin(DisconnectRequest request)
		{
			if (request == null)
			{
				throw new ServerInvalidArgumentException("request", null);
			}
			if (this.contextHandle == IntPtr.Zero)
			{
				throw new ServerInvalidBindingException(string.Format("Disconnect being called when we don't have a valid stored context handle.", new object[0]), null);
			}
			if (request.Context != (uint)this.contextHandle.ToInt64())
			{
				throw new ServerInvalidBindingException(string.Format("Disconnect called with a context handle that doesn't match stored value; request={0}, stored={1}.", request.Context, (uint)this.contextHandle.ToInt64()), null);
			}
			base.ExchangeAsyncDispatch.BeginDisconnect(null, this.contextHandle, new CancelableAsyncCallback(base.Complete), this);
		}

		protected override void InternalBeginCleanup(bool isSuccessful)
		{
		}

		protected override DisconnectResponse InternalEnd(ICancelableAsyncResult asyncResult)
		{
			return new DisconnectResponse
			{
				ErrorCode = (uint)base.ExchangeAsyncDispatch.EndDisconnect(asyncResult, out this.contextHandle),
				Context = (uint)this.contextHandle.ToInt64()
			};
		}

		protected override void InternalEndCleanup()
		{
		}

		protected override DisconnectResponse InternalFailure(uint serviceCode)
		{
			return new DisconnectResponse
			{
				ServiceCode = serviceCode
			};
		}

		private IntPtr contextHandle;
	}
}
