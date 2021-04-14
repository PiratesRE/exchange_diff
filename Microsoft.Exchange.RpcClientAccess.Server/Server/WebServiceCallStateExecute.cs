using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.XropService;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class WebServiceCallStateExecute : WebServiceCallState<ExecuteRequest, ExecuteResponse>
	{
		public WebServiceCallStateExecute(WebServiceUserInformation userInformation, IExchangeAsyncDispatch exchangeAsyncDispatch, AsyncCallback asyncCallback, object asyncState, IntPtr contextHandle) : base(userInformation, exchangeAsyncDispatch, asyncCallback, asyncState)
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
				return "Execute";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.FailedXropTracer;
			}
		}

		protected override void InternalBegin(ExecuteRequest request)
		{
			this.timeStart = ExDateTime.Now;
			if (request == null)
			{
				throw new ServerInvalidArgumentException("request", null);
			}
			if (this.contextHandle == IntPtr.Zero)
			{
				throw new ServerInvalidBindingException(string.Format("Execute being called when we don't have a valid stored context handle.", new object[0]), null);
			}
			if (request.Context != (uint)this.contextHandle.ToInt64())
			{
				throw new ServerInvalidBindingException(string.Format("Execute called with a context handle that doesn't match stored value; request={0}, stored={1}.", request.Context, (uint)this.contextHandle.ToInt64()), null);
			}
			ArraySegment<byte> segmentExtendedRopIn = WebServiceCall.BuildRequestSegment(request.In);
			ArraySegment<byte> responseRopSegment = WebServiceCall.GetResponseRopSegment((int)request.MaxSizeOut, out this.ropOut);
			ArraySegment<byte> segmentExtendedAuxIn = WebServiceCall.BuildRequestSegment(request.AuxIn);
			ArraySegment<byte> responseAuxSegment = WebServiceCall.GetResponseAuxSegment((int)request.MaxSizeAuxOut, out this.auxOut);
			base.ExchangeAsyncDispatch.BeginExecute(null, this.contextHandle, (int)request.Flags, segmentExtendedRopIn, responseRopSegment, segmentExtendedAuxIn, responseAuxSegment, new CancelableAsyncCallback(base.Complete), this);
		}

		protected override void InternalBeginCleanup(bool isSuccessful)
		{
			if (!isSuccessful)
			{
				if (this.ropOut != null)
				{
					WebServiceCall.ReleaseBuffer(this.ropOut);
					this.ropOut = null;
				}
				if (this.auxOut != null)
				{
					WebServiceCall.ReleaseBuffer(this.auxOut);
					this.auxOut = null;
				}
			}
		}

		protected override ExecuteResponse InternalEnd(ICancelableAsyncResult asyncResult)
		{
			ArraySegment<byte> segment;
			ArraySegment<byte> segment2;
			return new ExecuteResponse
			{
				ErrorCode = (uint)base.ExchangeAsyncDispatch.EndExecute(asyncResult, out this.contextHandle, out segment, out segment2),
				Context = (uint)this.contextHandle.ToInt64(),
				Out = WebServiceCall.BuildResponseArray(segment),
				AuxOut = WebServiceCall.BuildResponseArray(segment2),
				Flags = 0U,
				TransTime = (uint)(ExDateTime.Now - this.timeStart).TotalMilliseconds
			};
		}

		protected override void InternalEndCleanup()
		{
			if (this.ropOut != null)
			{
				WebServiceCall.ReleaseBuffer(this.ropOut);
				this.ropOut = null;
			}
			if (this.auxOut != null)
			{
				WebServiceCall.ReleaseBuffer(this.auxOut);
				this.auxOut = null;
			}
		}

		protected override ExecuteResponse InternalFailure(uint serviceCode)
		{
			return new ExecuteResponse
			{
				ServiceCode = serviceCode
			};
		}

		private IntPtr contextHandle;

		private byte[] ropOut;

		private byte[] auxOut;

		private ExDateTime timeStart;
	}
}
