using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using msclr;

namespace Microsoft.Exchange.Rpc
{
	internal abstract class BaseAsyncRpcServer<Microsoft::Exchange::Rpc::IRfriAsyncDispatch> : RpcServerBase
	{
		private void RundownThread(object state)
		{
			@lock @lock = null;
			@lock lock2 = null;
			bool flag = false;
			try
			{
				for (;;)
				{
					IntPtr intPtr = 0;
					@lock lock3 = new @lock(this.m_rundownQueueLock);
					try
					{
						@lock = lock3;
						if (this.m_rundownQueue.Count != 0)
						{
							goto IL_4E;
						}
						this.m_rundownThreadCount--;
						flag = true;
					}
					catch
					{
						((IDisposable)@lock).Dispose();
						throw;
					}
					break;
					IL_4E:
					IntPtr contextHandle;
					try
					{
						contextHandle = this.m_rundownQueue.Dequeue();
					}
					catch
					{
						((IDisposable)@lock).Dispose();
						throw;
					}
					((IDisposable)@lock).Dispose();
					this.RundownContext(contextHandle);
				}
				((IDisposable)@lock).Dispose();
			}
			finally
			{
				if (!flag)
				{
					@lock lock4 = new @lock(this.m_rundownQueueLock);
					try
					{
						lock2 = lock4;
						this.m_rundownThreadCount--;
					}
					catch
					{
						((IDisposable)lock2).Dispose();
						goto EndFinally_14;
						throw;
					}
					((IDisposable)lock2).Dispose();
				}
				EndFinally_14:;
			}
		}

		public BaseAsyncRpcServer<Microsoft::Exchange::Rpc::IRfriAsyncDispatch>()
		{
			this.m_hAsyncIOCompletionPort = null;
			this.m_fAsyncShutdown = false;
			this.m_thread = null;
			this.m_rundownQueueLock = new object();
			this.m_rundownQueue = new Queue<IntPtr>(1000);
			this.m_rundownThreadCount = 0;
			this.m_waitCallback = new WaitCallback(this.RundownThread);
		}

		public unsafe void DroppedConnectionThread()
		{
			if (!this.m_fAsyncShutdown)
			{
				do
				{
					uint num = 0;
					ulong num2 = 0UL;
					_OVERLAPPED* ptr = null;
					if (<Module>.GetQueuedCompletionStatus(this.m_hAsyncIOCompletionPort, &num, &num2, &ptr, -1) == null)
					{
						<Module>.Sleep(500);
					}
					else
					{
						if (this.m_fAsyncShutdown)
						{
							break;
						}
						if (ptr != null)
						{
							IntPtr contextHandle = new IntPtr((void*)ptr);
							this.DroppedConnection(contextHandle);
						}
					}
				}
				while (!this.m_fAsyncShutdown);
			}
		}

		public unsafe virtual void StartDroppedConnectionNotificationThread()
		{
			bool flag = false;
			try
			{
				void* ptr = <Module>.CreateIoCompletionPort(-1L, null, 0UL, 1);
				this.m_hAsyncIOCompletionPort = ptr;
				if (ptr == null)
				{
					throw new OutOfMemoryException();
				}
				Thread thread = new Thread(new ThreadStart(this.DroppedConnectionThread));
				this.m_thread = thread;
				thread.Name = "DroppedConnectionNotificationThread";
				this.m_thread.IsBackground = true;
				this.m_thread.Start();
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.StopDroppedConnectionNotificationThread();
				}
			}
		}

		public unsafe virtual void StopDroppedConnectionNotificationThread()
		{
			Thread thread = this.m_thread;
			if (thread != null)
			{
				this.m_fAsyncShutdown = true;
				if (thread.IsAlive)
				{
					<Module>.PostQueuedCompletionStatus(this.m_hAsyncIOCompletionPort, 0, 0UL, null);
					if (this.m_thread.IsAlive)
					{
						do
						{
							<Module>.Sleep(500);
						}
						while (this.m_thread.IsAlive);
					}
				}
				this.m_thread = null;
			}
			void* hAsyncIOCompletionPort = this.m_hAsyncIOCompletionPort;
			if (hAsyncIOCompletionPort != null)
			{
				<Module>.CloseHandle(hAsyncIOCompletionPort);
				this.m_hAsyncIOCompletionPort = null;
			}
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public virtual bool CanClientConnect(WindowsIdentity windowsIdentity)
		{
			return windowsIdentity != null && windowsIdentity.IsAuthenticated && !windowsIdentity.IsGuest && !windowsIdentity.IsAnonymous;
		}

		public abstract void RundownContext(IntPtr contextHandle);

		public abstract void DroppedConnection(IntPtr contextHandle);

		public abstract IRfriAsyncDispatch GetAsyncDispatch();

		public abstract void StartRundownQueue();

		public abstract void StopRundownQueue();

		private unsafe void* m_hAsyncIOCompletionPort;

		private bool m_fAsyncShutdown;

		private Thread m_thread;

		private static readonly int m_rundownThreadMax = 4;

		private object m_rundownQueueLock;

		private Queue<IntPtr> m_rundownQueue;

		private int m_rundownThreadCount;

		private WaitCallback m_waitCallback;
	}
}
