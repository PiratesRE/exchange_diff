using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class RpcHttpConnectionRegistrationAsyncDispatch : IRpcHttpConnectionRegistrationAsyncDispatch
	{
		public RpcHttpConnectionRegistrationAsyncDispatch(RpcHttpConnectionRegistrationDispatch rpcHttpConnectionRegistrationDispatch, DispatchPool dispatchPool)
		{
			this.rpcHttpConnectionRegistrationDispatch = rpcHttpConnectionRegistrationDispatch;
			this.dispatchPool = dispatchPool;
		}

		public ICancelableAsyncResult BeginRegister(Guid associationGroupId, string token, string serverTarget, string sessionCookie, string clientIp, Guid requestId, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			if (ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::BeginRegister started. Guid={0}. Token={1}. ClientIP={2}. RequestId={3}.", new object[]
				{
					associationGroupId,
					token,
					clientIp,
					requestId
				});
			}
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				RegisterDispatchTask registerDispatchTask = new RegisterDispatchTask(this.rpcHttpConnectionRegistrationDispatch, asyncCallback, asyncState, associationGroupId, token, serverTarget, sessionCookie, clientIp, requestId);
				this.SubmitTask(registerDispatchTask);
				flag = true;
				result = registerDispatchTask;
			}
			finally
			{
				if (!flag)
				{
					ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::BeginRegister failed.");
				}
			}
			return result;
		}

		public int EndRegister(ICancelableAsyncResult asyncResult, out string failureMessage, out string failureDetails)
		{
			failureMessage = null;
			failureDetails = null;
			RegisterDispatchTask registerDispatchTask = (RegisterDispatchTask)asyncResult;
			bool flag = false;
			int result;
			try
			{
				int num = registerDispatchTask.End(out failureMessage, out failureDetails);
				if (ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug<int>(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::EndRegister succeeded. ErrorCode={0}.", num);
				}
				flag = true;
				result = num;
			}
			finally
			{
				if (!flag)
				{
					ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::EndRegister failed.");
				}
			}
			return result;
		}

		public ICancelableAsyncResult BeginUnregister(Guid associationGroupId, Guid requestId, CancelableAsyncCallback asyncCallback, object asyncState)
		{
			if (ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug<Guid, Guid>(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::BeginUnregister started. Guid={0}. RequestId={1}.", associationGroupId, requestId);
			}
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				UnregisterDispatchTask unregisterDispatchTask = new UnregisterDispatchTask(this.rpcHttpConnectionRegistrationDispatch, asyncCallback, asyncState, associationGroupId, requestId);
				this.SubmitTask(unregisterDispatchTask);
				flag = true;
				result = unregisterDispatchTask;
			}
			finally
			{
				if (!flag)
				{
					ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::BeginUnregister failed.");
				}
			}
			return result;
		}

		public int EndUnregister(ICancelableAsyncResult asyncResult)
		{
			UnregisterDispatchTask unregisterDispatchTask = (UnregisterDispatchTask)asyncResult;
			bool flag = false;
			int result;
			try
			{
				int num = unregisterDispatchTask.End();
				if (ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug<int>(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::EndUnregister succeeded. ErrorCode={0}.", num);
				}
				flag = true;
				result = num;
			}
			finally
			{
				if (!flag)
				{
					ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::EndUnregister failed.");
				}
			}
			return result;
		}

		public ICancelableAsyncResult BeginClear(CancelableAsyncCallback asyncCallback, object asyncState)
		{
			if (ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::BeginClear started.");
			}
			bool flag = false;
			ICancelableAsyncResult result;
			try
			{
				ClearDispatchTask clearDispatchTask = new ClearDispatchTask(this.rpcHttpConnectionRegistrationDispatch, asyncCallback, asyncState);
				this.SubmitTask(clearDispatchTask);
				flag = true;
				result = clearDispatchTask;
			}
			finally
			{
				if (!flag)
				{
					ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::BeginClear failed.");
				}
			}
			return result;
		}

		public int EndClear(ICancelableAsyncResult asyncResult)
		{
			ClearDispatchTask clearDispatchTask = (ClearDispatchTask)asyncResult;
			bool flag = false;
			int result;
			try
			{
				int num = clearDispatchTask.End();
				if (ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug<int>(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::EndClear succeeded. ErrorCode={0}.", num);
				}
				flag = true;
				result = num;
			}
			finally
			{
				if (!flag)
				{
					ExTraceGlobals.RpcHttpConnectionRegistrationAsyncDispatchTracer.TraceDebug(0, 0L, "RpcHttpConnectionRegistrationAsyncDispatch::EndClear failed.");
				}
			}
			return result;
		}

		internal RpcHttpConnectionRegistrationDispatch RpcHttpConnectionRegistrationDispatch
		{
			get
			{
				return this.rpcHttpConnectionRegistrationDispatch;
			}
		}

		internal bool IsShuttingDown
		{
			get
			{
				return RpcClientAccessService.IsShuttingDown;
			}
		}

		private void SubmitTask(DispatchTask task)
		{
			if (!this.IsShuttingDown)
			{
				if (!this.dispatchPool.SubmitTask(task))
				{
					task.Completion(new ServerTooBusyException("Unable to submit task; queue full"), 0);
					return;
				}
			}
			else
			{
				task.Completion(new ServerUnavailableException("Shutting down"), 0);
			}
		}

		private readonly RpcHttpConnectionRegistrationDispatch rpcHttpConnectionRegistrationDispatch;

		private readonly DispatchPool dispatchPool;
	}
}
