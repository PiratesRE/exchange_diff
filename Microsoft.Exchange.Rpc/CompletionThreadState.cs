using System;
using System.Collections.Generic;
using System.Threading;
using msclr;

namespace Microsoft.Exchange.Rpc
{
	public class CompletionThreadState
	{
		private unsafe void StartupThread()
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
				this.m_fShutdown = false;
				this.m_shutdownCompletionEvent.Reset();
				Thread thread = new Thread(new ThreadStart(this.AsyncCallCompletionThread));
				this.m_thread = thread;
				if (thread.Name == null)
				{
					this.m_thread.Name = "AsyncCallCompletionThread";
				}
				this.m_thread.IsBackground = true;
				this.m_thread.Start();
				AppDomain.CurrentDomain.DomainUnload += this.AppDomainUnload;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.ShutdownThread();
				}
			}
		}

		private unsafe void ShutdownThread()
		{
			if (this.m_thread != null)
			{
				this.m_fShutdown = true;
				if (Thread.CurrentThread.ManagedThreadId != this.m_thread.ManagedThreadId)
				{
					<Module>.PostQueuedCompletionStatus(this.m_hAsyncIOCompletionPort, 0, 0UL, null);
					this.m_shutdownCompletionEvent.WaitOne();
				}
				this.m_thread = null;
			}
			ManualResetEvent shutdownCompletionEvent = this.m_shutdownCompletionEvent;
			if (shutdownCompletionEvent != null)
			{
				((IDisposable)shutdownCompletionEvent).Dispose();
				this.m_shutdownCompletionEvent = null;
			}
			void* hAsyncIOCompletionPort = this.m_hAsyncIOCompletionPort;
			if (hAsyncIOCompletionPort != null)
			{
				<Module>.CloseHandle(hAsyncIOCompletionPort);
				this.m_hAsyncIOCompletionPort = null;
			}
		}

		private void AppDomainUnload(object sender, EventArgs e)
		{
			this.ShutdownThread();
		}

		private static void AsyncCompletionRoutine(object @object)
		{
			((ClientAsyncResult)@object).Completion();
		}

		public CompletionThreadState()
		{
			this.m_lock = new object();
			this.m_hAsyncIOCompletionPort = null;
			this.m_thread = null;
			this.m_callDictionary = new Dictionary<IntPtr, ClientAsyncResult>();
			this.m_shutdownCompletionEvent = new ManualResetEvent(false);
			this.m_fShutdown = false;
			this.StartupThread();
		}

		public unsafe void* GetIoCompletionPort()
		{
			return this.m_hAsyncIOCompletionPort;
		}

		public unsafe void AsyncCallCompletionThread()
		{
			ClientAsyncResult clientAsyncResult = null;
			@lock @lock = null;
			try
			{
				while (!this.m_fShutdown)
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
						if (this.m_fShutdown)
						{
							break;
						}
						if (ptr != null)
						{
							IntPtr key = new IntPtr((void*)ptr);
							clientAsyncResult = null;
							@lock lock2 = new @lock(this.m_lock);
							bool flag;
							try
							{
								@lock = lock2;
								flag = this.m_callDictionary.TryGetValue(key, out clientAsyncResult);
							}
							catch
							{
								((IDisposable)@lock).Dispose();
								throw;
							}
							((IDisposable)@lock).Dispose();
							if (flag && clientAsyncResult != null && !ThreadPool.QueueUserWorkItem(new WaitCallback(CompletionThreadState.AsyncCompletionRoutine), clientAsyncResult))
							{
								clientAsyncResult.Completion();
							}
						}
					}
				}
			}
			finally
			{
				ManualResetEvent shutdownCompletionEvent = this.m_shutdownCompletionEvent;
				if (shutdownCompletionEvent != null)
				{
					shutdownCompletionEvent.Set();
				}
			}
		}

		public IntPtr CreateAsyncCallHandle(ClientAsyncResult clientAsyncResult)
		{
			@lock @lock = null;
			@lock lock2 = new @lock(this.m_lock);
			IntPtr intPtr;
			try
			{
				@lock = lock2;
				intPtr = new IntPtr(Interlocked.Increment(ref CompletionThreadState.m_nextAsyncCallHandle));
				this.m_callDictionary.Add(intPtr, clientAsyncResult);
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
			return intPtr;
		}

		public void DestroyAsyncCallHandle(IntPtr asyncCallHandle)
		{
			@lock @lock = null;
			@lock lock2 = new @lock(this.m_lock);
			try
			{
				@lock = lock2;
				this.m_callDictionary.Remove(asyncCallHandle);
			}
			catch
			{
				((IDisposable)@lock).Dispose();
				throw;
			}
			((IDisposable)@lock).Dispose();
		}

		private static int m_nextAsyncCallHandle = 1;

		private object m_lock;

		private unsafe void* m_hAsyncIOCompletionPort;

		private Thread m_thread;

		private Dictionary<IntPtr, ClientAsyncResult> m_callDictionary;

		private ManualResetEvent m_shutdownCompletionEvent;

		private bool m_fShutdown;
	}
}
