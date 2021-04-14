using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Exchange.Rpc
{
	internal abstract class RpcAsyncResult : IRpcAsyncResult, IDisposable
	{
		protected RpcAsyncResult(AsyncCallback callback, object asyncState)
		{
			this.m_callback = callback;
			this.m_asyncState = asyncState;
			this.m_completedEvent = new ManualResetEvent(false);
			this.m_ptpWait = null;
		}

		private unsafe void ~RpcAsyncResult()
		{
			_TP_WAIT* ptpWait = this.m_ptpWait;
			if (ptpWait != null)
			{
				<Module>.SetThreadpoolWait(ptpWait, null, null);
				<Module>.CloseThreadpoolWait(this.m_ptpWait);
			}
			ManualResetEvent completedEvent = this.m_completedEvent;
			if (completedEvent != null)
			{
				((IDisposable)completedEvent).Dispose();
				this.m_completedEvent = null;
			}
		}

		public virtual bool IsCompleted
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_completedEvent.WaitOne(0);
			}
		}

		public virtual WaitHandle AsyncWaitHandle
		{
			get
			{
				return this.m_completedEvent;
			}
		}

		public virtual object AsyncState
		{
			get
			{
				return this.m_asyncState;
			}
		}

		public virtual bool CompletedSynchronously
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return false;
			}
		}

		public virtual void Cancel()
		{
			throw new NotImplementedException();
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe bool RegisterWait(IntPtr rootedAsyncState)
		{
			if (!(this.m_callback != null))
			{
				return false;
			}
			_TP_WAIT* ptr = <Module>.CreateThreadpoolWait(<Module>.__unep@?OnRpcCompleteCallback@@$$FYAXPEAU_TP_CALLBACK_INSTANCE@@PEAXPEAU_TP_WAIT@@K@Z, rootedAsyncState.ToPointer(), null);
			this.m_ptpWait = ptr;
			if (ptr == null)
			{
				string message = "CreateThreadpoolWait failed";
				throw <Module>.GetRpcException(<Module>.GetLastError(), message);
			}
			IntPtr handle = this.m_completedEvent.Handle;
			<Module>.SetThreadpoolWait(this.m_ptpWait, handle.ToPointer(), null);
			return true;
		}

		public void OnRpcComplete()
		{
			if (this.m_callback != null)
			{
				this.m_callback(this);
			}
		}

		protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				this.~RpcAsyncResult();
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

		private object m_asyncState;

		private ManualResetEvent m_completedEvent;

		private unsafe _TP_WAIT* m_ptpWait;

		protected AsyncCallback m_callback;
	}
}
