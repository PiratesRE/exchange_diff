using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using msclr;

namespace Microsoft.Exchange.Rpc
{
	public abstract class ClientAsyncCallState : ClientAsyncResult, IDisposable
	{
		private static void EnsureAsyncCallCompletionThread()
		{
			@lock @lock = null;
			if (ClientAsyncCallState.m_completionThreadState == null)
			{
				@lock lock2 = new @lock(ClientAsyncCallState.m_lock);
				try
				{
					@lock = lock2;
					if (ClientAsyncCallState.m_completionThreadState == null)
					{
						ClientAsyncCallState.m_completionThreadState = new CompletionThreadState();
					}
				}
				catch
				{
					((IDisposable)@lock).Dispose();
					throw;
				}
				((IDisposable)@lock).Dispose();
			}
		}

		private static IntPtr CreateAsyncCallHandle(ClientAsyncResult clientAsyncResult)
		{
			@lock @lock = null;
			@lock lock2 = new @lock(ClientAsyncCallState.m_lock);
			IntPtr result;
			try
			{
				@lock = lock2;
				result = ClientAsyncCallState.m_completionThreadState.CreateAsyncCallHandle(clientAsyncResult);
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
			return result;
		}

		private void DestroyAsyncCallHandle(IntPtr asyncCallHandle)
		{
			@lock @lock = null;
			@lock lock2 = new @lock(ClientAsyncCallState.m_lock);
			try
			{
				@lock = lock2;
				ClientAsyncCallState.m_completionThreadState.DestroyAsyncCallHandle(asyncCallHandle);
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
		}

		[HandleProcessCorruptedStateExceptions]
		private int Wrapped_InternalBegin()
		{
			int result = 0;
			try
			{
				this.m_fCallOutstanding = true;
				this.InternalBegin();
			}
			catch when (endfilter(true))
			{
				this.m_fCallOutstanding = false;
				result = Marshal.GetExceptionCode();
			}
			return result;
		}

		private void CallCleanup()
		{
			if (this.m_fCallOutstanding)
			{
				throw new InvalidOperationException("Asynchronous RPC call still outstanding");
			}
			if (this.m_asyncCallHandle != IntPtr.Zero)
			{
				this.DestroyAsyncCallHandle(this.m_asyncCallHandle);
				this.m_asyncCallHandle = IntPtr.Zero;
			}
			if (this.m_pRpcAsyncState != IntPtr.Zero)
			{
				initblk(this.m_pRpcAsyncState.ToPointer(), 0, 112L);
				Marshal.FreeHGlobal(this.m_pRpcAsyncState);
				this.m_pRpcAsyncState = IntPtr.Zero;
			}
		}

		protected void CompleteSynchronously(Exception exception)
		{
			this.m_exception = exception;
			this.m_fCallOutstanding = false;
			base.Completion();
		}

		[HandleProcessCorruptedStateExceptions]
		protected unsafe override void InternalCancel()
		{
			if (this.m_pRpcAsyncState != IntPtr.Zero && this.m_fCallOutstanding)
			{
				try
				{
					int num = <Module>.RpcAsyncCancelCall((_RPC_ASYNC_STATE*)this.m_pRpcAsyncState.ToPointer(), 1);
				}
				catch when (endfilter(true))
				{
				}
			}
		}

		public ClientAsyncCallState(string callName, CancelableAsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
			this.m_exception = null;
			this.m_fCallOutstanding = false;
			this.m_pRpcAsyncState = IntPtr.Zero;
			this.m_callName = callName;
			this.m_asyncCallHandle = IntPtr.Zero;
		}

		private void ~ClientAsyncCallState()
		{
			this.CallCleanup();
		}

		public IntPtr RpcAsyncState()
		{
			return this.m_pRpcAsyncState;
		}

		public abstract void InternalBegin();

		public unsafe void Begin()
		{
			this.m_fCallOutstanding = false;
			try
			{
				ClientAsyncCallState.EnsureAsyncCallCompletionThread();
				IntPtr pRpcAsyncState = Marshal.AllocHGlobal(112);
				this.m_pRpcAsyncState = pRpcAsyncState;
				initblk(this.m_pRpcAsyncState.ToPointer(), 0, 112L);
				int num = <Module>.RpcAsyncInitializeHandle((_RPC_ASYNC_STATE*)this.m_pRpcAsyncState.ToPointer(), 112U);
				if (num != null)
				{
					Exception exception = <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num, "ClientAsyncCallState.Begin: RpcAsyncInitializeHandle");
					this.m_exception = exception;
					this.m_fCallOutstanding = false;
					base.Completion();
				}
				else
				{
					IntPtr asyncCallHandle = ClientAsyncCallState.CreateAsyncCallHandle(this);
					this.m_asyncCallHandle = asyncCallHandle;
					_RPC_ASYNC_STATE* ptr = (_RPC_ASYNC_STATE*)this.m_pRpcAsyncState.ToPointer();
					*(long*)(ptr + 24L / (long)sizeof(_RPC_ASYNC_STATE)) = this.m_asyncCallHandle.ToPointer();
					*(int*)(ptr + 44L / (long)sizeof(_RPC_ASYNC_STATE)) = 3;
					*(long*)(ptr + 48L / (long)sizeof(_RPC_ASYNC_STATE)) = ClientAsyncCallState.m_completionThreadState.GetIoCompletionPort();
					*(long*)(ptr + 72L / (long)sizeof(_RPC_ASYNC_STATE)) = this.m_asyncCallHandle.ToPointer();
					int num2 = this.Wrapped_InternalBegin();
					if (num2 != null)
					{
						byte condition = (!this.m_fCallOutstanding) ? 1 : 0;
						ExAssert.Assert(condition != 0, "Failed synchronously, but call marked as outstanding");
						Exception exception2 = <Module>.Microsoft.Exchange.Rpc.GetRpcExceptionWithEEInfo(num2, "ClientAsyncCallState.Begin: InternalBegin");
						this.m_exception = exception2;
						this.m_fCallOutstanding = false;
						base.Completion();
					}
				}
			}
			finally
			{
				if (!this.m_fCallOutstanding)
				{
					this.CallCleanup();
				}
			}
		}

		public unsafe int CheckCompletion()
		{
			int result = 0;
			try
			{
				base.WaitForCompletion();
				if (this.m_exception != null)
				{
					byte condition = (!this.m_fCallOutstanding) ? 1 : 0;
					ExAssert.Assert(condition != 0, "Stored synchronous call failure exception from Begin, but call marked as outstanding");
					throw this.m_exception;
				}
				if (!this.m_fCallOutstanding)
				{
					throw new InvalidOperationException("CheckCompletion being called with no synchronous call failure or outstanding call.");
				}
				this.m_fCallOutstanding = false;
				ExAssert.Assert(true, "Call is outstanding, but there is a synchronous failure exception from Begin");
				int num = <Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)this.m_pRpcAsyncState.ToPointer(), (void*)(&result));
				if (num != null)
				{
					<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "ClientAsyncCallState.CheckCompletion: RpcAsyncCompleteCall");
				}
			}
			finally
			{
				this.CallCleanup();
			}
			return result;
		}

		protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				this.CallCleanup();
			}
			else
			{
				base.Finalize();
			}
		}

		public sealed void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private static object m_lock = new object();

		private static CompletionThreadState m_completionThreadState = null;

		private IntPtr m_pRpcAsyncState;

		private IntPtr m_asyncCallHandle;

		private string m_callName;

		private bool m_fCallOutstanding;

		private Exception m_exception;
	}
}
