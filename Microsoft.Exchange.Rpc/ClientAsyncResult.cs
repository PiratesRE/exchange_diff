using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Net;
using msclr;

namespace Microsoft.Exchange.Rpc
{
	public class ClientAsyncResult : ICancelableAsyncResult
	{
		public ClientAsyncResult(CancelableAsyncCallback asyncCallback, object asyncState)
		{
			this.m_completedEventLock = new object();
			this.m_completedEvent = null;
			this.m_isCompleted = false;
			this.m_isCanceled = false;
			this.m_asyncCallback = asyncCallback;
			this.m_asyncState = asyncState;
		}

		public void Completion()
		{
			@lock @lock = null;
			if (!this.m_isCompleted)
			{
				@lock lock2 = new @lock(this.m_completedEventLock);
				try
				{
					@lock = lock2;
					this.m_isCompleted = true;
				}
				catch
				{
					((IDisposable)@lock).Dispose();
					throw;
				}
				((IDisposable)@lock).Dispose();
				ManualResetEvent completedEvent = this.m_completedEvent;
				if (completedEvent != null)
				{
					completedEvent.Set();
				}
				if (this.m_asyncCallback != null)
				{
					this.m_asyncCallback(this);
				}
			}
		}

		public void WaitForCompletion()
		{
			@lock @lock = null;
			@lock lock2 = null;
			if (!this.m_isCompleted)
			{
				ManualResetEvent manualResetEvent = null;
				if (this.m_completedEvent == null)
				{
					manualResetEvent = new ManualResetEvent(false);
				}
				try
				{
					@lock lock3 = new @lock(this.m_completedEventLock);
					try
					{
						@lock = lock3;
						if (!this.m_isCompleted)
						{
							goto IL_4C;
						}
					}
					catch
					{
						((IDisposable)@lock).Dispose();
						throw;
					}
					((IDisposable)@lock).Dispose();
					return;
					IL_4C:
					try
					{
						if (this.m_completedEvent == null)
						{
							this.m_completedEvent = manualResetEvent;
							manualResetEvent = null;
						}
					}
					catch
					{
						((IDisposable)@lock).Dispose();
						throw;
					}
					((IDisposable)@lock).Dispose();
					this.m_completedEvent.WaitOne();
				}
				finally
				{
					@lock lock4 = new @lock(this.m_completedEventLock);
					ManualResetEvent completedEvent;
					try
					{
						lock2 = lock4;
						completedEvent = this.m_completedEvent;
						this.m_completedEvent = null;
					}
					catch
					{
						((IDisposable)lock2).Dispose();
						goto EndFinally_19;
						throw;
					}
					((IDisposable)lock2).Dispose();
					if (completedEvent != null)
					{
						((IDisposable)completedEvent).Dispose();
					}
					if (manualResetEvent != null)
					{
						((IDisposable)manualResetEvent).Dispose();
					}
					EndFinally_19:;
				}
			}
		}

		public virtual object AsyncState
		{
			get
			{
				return this.m_asyncState;
			}
		}

		public virtual WaitHandle AsyncWaitHandle
		{
			get
			{
				@lock @lock = null;
				if (this.m_completedEvent == null)
				{
					@lock lock2 = new @lock(this.m_completedEventLock);
					try
					{
						@lock = lock2;
						if (this.m_completedEvent == null)
						{
							this.m_completedEvent = new ManualResetEvent(this.m_isCompleted);
						}
					}
					catch
					{
						((IDisposable)@lock).Dispose();
						throw;
					}
					((IDisposable)@lock).Dispose();
				}
				return this.m_completedEvent;
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

		public virtual bool IsCompleted
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_isCompleted;
			}
		}

		public virtual bool IsCanceled
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_isCanceled;
			}
		}

		public virtual void Cancel()
		{
			@lock @lock = null;
			if (!this.m_isCanceled)
			{
				@lock lock2 = new @lock(this.m_completedEventLock);
				try
				{
					@lock = lock2;
					if (!this.m_isCanceled)
					{
						goto IL_33;
					}
				}
				catch
				{
					((IDisposable)@lock).Dispose();
					throw;
				}
				((IDisposable)@lock).Dispose();
				return;
				IL_33:
				try
				{
					this.m_isCanceled = true;
				}
				catch
				{
					((IDisposable)@lock).Dispose();
					throw;
				}
				((IDisposable)@lock).Dispose();
				if (!this.IsCompleted)
				{
					this.InternalCancel();
				}
			}
		}

		protected virtual void InternalCancel()
		{
		}

		private CancelableAsyncCallback m_asyncCallback;

		private object m_asyncState;

		private object m_completedEventLock;

		private ManualResetEvent m_completedEvent;

		private bool m_isCompleted;

		private bool m_isCanceled;
	}
}
