using System;
using System.Threading;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal abstract class RpcHttpConnectionRegistrationAsyncDispatchRpcState
	{
		private static void Callback(ICancelableAsyncResult asyncResult)
		{
			RpcHttpConnectionRegistrationAsyncDispatchRpcState rpcHttpConnectionRegistrationAsyncDispatchRpcState = (RpcHttpConnectionRegistrationAsyncDispatchRpcState)asyncResult.AsyncState;
			rpcHttpConnectionRegistrationAsyncDispatchRpcState.InternalCallback(asyncResult);
			rpcHttpConnectionRegistrationAsyncDispatchRpcState.Reset();
			rpcHttpConnectionRegistrationAsyncDispatchRpcState.ReleaseToPool();
		}

		protected RpcHttpConnectionRegistrationAsyncDispatchRpcState()
		{
			this.asyncState = null;
			this.asyncServer = null;
		}

		protected RpcHttpConnectionRegistrationAsyncRpcServer AsyncServer
		{
			get
			{
				return this.asyncServer;
			}
		}

		protected SafeRpcAsyncStateHandle AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		protected void InternalInitialize(SafeRpcAsyncStateHandle asyncState, RpcHttpConnectionRegistrationAsyncRpcServer asyncServer)
		{
			this.asyncState = asyncState;
			this.asyncServer = asyncServer;
		}

		protected void InternalReset()
		{
			this.asyncState = null;
			this.asyncServer = null;
		}

		protected void InternalCallback(ICancelableAsyncResult asyncResult)
		{
			try
			{
				IRpcHttpConnectionRegistrationAsyncDispatch rpcHttpConnectionRegistrationAsyncDispatch = this.asyncServer.GetRpcHttpConnectionRegistrationAsyncDispatch();
				if (rpcHttpConnectionRegistrationAsyncDispatch != null && !this.asyncServer.IsShuttingDown())
				{
					int result = this.InternalEnd(asyncResult, rpcHttpConnectionRegistrationAsyncDispatch);
					this.asyncState.CompleteCall(result);
				}
				else
				{
					this.asyncState.AbortCall(1722U);
				}
			}
			catch (RpcException ex)
			{
				this.asyncState.AbortCall((uint)ex.ErrorCode);
			}
			catch (FailRpcException ex2)
			{
				this.asyncState.CompleteCall(ex2.ErrorCode);
			}
			catch (ThreadAbortException)
			{
				this.asyncState.AbortCall(1726U);
			}
			catch (OutOfMemoryException)
			{
				this.asyncState.AbortCall(1130U);
			}
			catch (Exception e)
			{
				<Module>.Microsoft.Exchange.Rpc.ManagedExceptionCrashWrapper.CrashMeNow(e);
			}
			catch (object o)
			{
				<Module>.Microsoft.Exchange.Rpc.ManagedExceptionCrashWrapper.CrashMeNow(o);
			}
			this.asyncState = null;
			this.asyncServer = null;
		}

		public void Begin()
		{
			try
			{
				IRpcHttpConnectionRegistrationAsyncDispatch rpcHttpConnectionRegistrationAsyncDispatch = this.asyncServer.GetRpcHttpConnectionRegistrationAsyncDispatch();
				if (rpcHttpConnectionRegistrationAsyncDispatch != null && !this.asyncServer.IsShuttingDown())
				{
					this.InternalBegin(RpcHttpConnectionRegistrationAsyncDispatchRpcState.asyncCallback, rpcHttpConnectionRegistrationAsyncDispatch);
				}
				else
				{
					this.asyncState.AbortCall(1722U);
				}
			}
			catch (RpcException ex)
			{
				this.asyncState.AbortCall((uint)ex.ErrorCode);
			}
			catch (FailRpcException ex2)
			{
				this.asyncState.CompleteCall(ex2.ErrorCode);
			}
			catch (ThreadAbortException)
			{
				this.asyncState.AbortCall(1726U);
			}
			catch (OutOfMemoryException)
			{
				this.asyncState.AbortCall(1130U);
			}
			catch (Exception e)
			{
				<Module>.Microsoft.Exchange.Rpc.ManagedExceptionCrashWrapper.CrashMeNow(e);
			}
			catch (object o)
			{
				<Module>.Microsoft.Exchange.Rpc.ManagedExceptionCrashWrapper.CrashMeNow(o);
			}
		}

		public abstract void InternalBegin(CancelableAsyncCallback asyncCallback, IRpcHttpConnectionRegistrationAsyncDispatch asyncDispatch);

		public abstract int InternalEnd(ICancelableAsyncResult asyncResult, IRpcHttpConnectionRegistrationAsyncDispatch asyncDispatch);

		public abstract void Reset();

		public abstract void ReleaseToPool();

		private static readonly CancelableAsyncCallback asyncCallback = new CancelableAsyncCallback(RpcHttpConnectionRegistrationAsyncDispatchRpcState.Callback);

		private SafeRpcAsyncStateHandle asyncState;

		private RpcHttpConnectionRegistrationAsyncRpcServer asyncServer;
	}
}
