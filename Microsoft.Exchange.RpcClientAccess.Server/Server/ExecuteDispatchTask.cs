using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class ExecuteDispatchTask : ExchangeDispatchTask
	{
		public ExecuteDispatchTask(IExchangeDispatch exchangeDispatch, CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, IntPtr contextHandle, int flags, ArraySegment<byte> segmentRopIn, ArraySegment<byte> segmentRopOut, ArraySegment<byte> segmentAuxIn, ArraySegment<byte> segmentAuxOut) : base("ExecuteDispatchTask", exchangeDispatch, protocolRequestInfo, asyncCallback, asyncState)
		{
			this.contextHandleIn = contextHandle;
			this.contextHandleOut = contextHandle;
			this.flags = flags;
			this.segmentRopIn = segmentRopIn;
			this.segmentRopOut = segmentRopOut;
			this.segmentAuxIn = segmentAuxIn;
			this.segmentAuxOut = segmentAuxOut;
			this.ropOutData = Array<byte>.EmptySegment;
			this.auxOutData = Array<byte>.EmptySegment;
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
			bool flag = false;
			int? result;
			try
			{
				int value = base.ExchangeDispatch.Execute(base.ProtocolRequestInfo, ref this.contextHandleOut, this.flags, this.segmentRopIn, this.segmentRopOut, out this.ropOutData, this.segmentAuxIn, this.segmentAuxOut, out this.auxOutData);
				flag = true;
				result = new int?(value);
			}
			finally
			{
				if (!flag)
				{
					this.ropOutData = Array<byte>.EmptySegment;
					this.auxOutData = Array<byte>.EmptySegment;
				}
			}
			return result;
		}

		public int End(out IntPtr contextHandle, out ArraySegment<byte> segmentRopOut, out ArraySegment<byte> segmentAuxOut)
		{
			int result = base.CheckCompletion();
			contextHandle = this.contextHandleOut;
			segmentRopOut = this.ropOutData;
			segmentAuxOut = this.auxOutData;
			return result;
		}

		private readonly int flags;

		private readonly ArraySegment<byte> segmentRopIn;

		private readonly ArraySegment<byte> segmentAuxIn;

		private readonly ArraySegment<byte> segmentRopOut;

		private readonly ArraySegment<byte> segmentAuxOut;

		private IntPtr contextHandleIn;

		private IntPtr contextHandleOut;

		private ArraySegment<byte> ropOutData;

		private ArraySegment<byte> auxOutData;
	}
}
