using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.Exchange.Net
{
	internal class LazyAsyncResult : IAsyncResult
	{
		private static LazyAsyncResult.ThreadContext CurrentThreadContext
		{
			get
			{
				LazyAsyncResult.ThreadContext threadContext = LazyAsyncResult.threadContext;
				if (threadContext == null)
				{
					threadContext = new LazyAsyncResult.ThreadContext();
					LazyAsyncResult.threadContext = threadContext;
				}
				return threadContext;
			}
		}

		public LazyAsyncResult(object worker, object callerState, AsyncCallback callerCallback)
		{
			this.asyncObject = worker;
			this.asyncState = callerState;
			this.asyncCallback = callerCallback;
			this.result = DBNull.Value;
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				this.userEvent = true;
				Interlocked.CompareExchange(ref this.intCompleted, int.MinValue, 0);
				ManualResetEvent manualResetEvent = (ManualResetEvent)this.internalEvent;
				while (manualResetEvent == null)
				{
					this.LazilyCreateEvent(out manualResetEvent);
				}
				return manualResetEvent;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return Interlocked.CompareExchange(ref this.intCompleted, int.MinValue, 0) > 0;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return (Interlocked.CompareExchange(ref this.intCompleted, int.MinValue, 0) & int.MaxValue) != 0;
			}
		}

		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		public object AsyncObject
		{
			get
			{
				return this.asyncObject;
			}
		}

		internal bool InternalPeekCompleted
		{
			get
			{
				return (this.intCompleted & int.MaxValue) != 0;
			}
		}

		internal object Result
		{
			get
			{
				if (this.result != DBNull.Value)
				{
					return this.result;
				}
				return null;
			}
		}

		internal bool EndCalled
		{
			get
			{
				return this.endCalled != 0;
			}
			set
			{
				this.endCalled = (value ? 1 : 0);
			}
		}

		internal int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
			set
			{
				this.errorCode = value;
			}
		}

		protected AsyncCallback AsyncCallback
		{
			get
			{
				return this.asyncCallback;
			}
			set
			{
				this.asyncCallback = value;
			}
		}

		internal object InternalWaitForCompletion()
		{
			return this.WaitForCompletion(true);
		}

		internal object InternalWaitForCompletionNoSideEffects()
		{
			return this.WaitForCompletion(false);
		}

		internal void InternalCleanup()
		{
			ManualResetEvent manualResetEvent = (ManualResetEvent)this.internalEvent;
			this.internalEvent = null;
			if (manualResetEvent != null)
			{
				manualResetEvent.Close();
			}
			if ((Interlocked.Increment(ref this.intCompleted) & 2147483647) == 1)
			{
				this.result = null;
				this.Cleanup();
			}
		}

		public bool InvokeCallback(object value)
		{
			return this.ProtectedInvokeCallback(value, IntPtr.Zero);
		}

		public bool InvokeCallback()
		{
			return this.ProtectedInvokeCallback(null, IntPtr.Zero);
		}

		public static T EndAsyncOperation<T>(IAsyncResult asyncResult) where T : LazyAsyncResult
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			T t = asyncResult as T;
			if (t == null)
			{
				throw new ArgumentException("asyncResult");
			}
			if (Interlocked.Increment(ref t.endCalled) != 1)
			{
				throw new InvalidOperationException(NetException.EndAlreadyCalled);
			}
			t.InternalWaitForCompletion();
			return t;
		}

		protected bool ProtectedInvokeCallback(object value, IntPtr userToken)
		{
			if ((Interlocked.Increment(ref this.intCompleted) & 2147483647) == 1)
			{
				if (this.result == DBNull.Value)
				{
					this.result = value;
				}
				this.Complete(userToken);
				return true;
			}
			return false;
		}

		protected virtual void Cleanup()
		{
		}

		protected virtual void Complete(IntPtr userToken)
		{
			LazyAsyncResult.ThreadContext currentThreadContext = LazyAsyncResult.CurrentThreadContext;
			try
			{
				currentThreadContext.nestedIOCount++;
				if (this.asyncCallback != null)
				{
					if (currentThreadContext.nestedIOCount >= 50)
					{
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.WorkerThreadComplete));
					}
					else
					{
						this.WorkerThreadComplete(null);
					}
				}
				else
				{
					this.WorkerThreadComplete(null);
				}
			}
			finally
			{
				currentThreadContext.nestedIOCount--;
			}
		}

		private static string HashString(object objectValue)
		{
			if (objectValue == null)
			{
				return "(null)";
			}
			string text = objectValue as string;
			if (!string.IsNullOrEmpty(text))
			{
				return text.GetHashCode().ToString(NumberFormatInfo.InvariantInfo);
			}
			return "(string.empty)";
		}

		private bool LazilyCreateEvent(out ManualResetEvent waitHandle)
		{
			waitHandle = new ManualResetEvent(false);
			bool flag;
			try
			{
				if (Interlocked.CompareExchange(ref this.internalEvent, waitHandle, null) == null)
				{
					if (this.InternalPeekCompleted)
					{
						waitHandle.Set();
					}
					flag = true;
				}
				else
				{
					waitHandle.Close();
					waitHandle = (ManualResetEvent)this.internalEvent;
					flag = false;
				}
			}
			catch
			{
				this.internalEvent = null;
				waitHandle.Close();
				throw;
			}
			return flag;
		}

		private object WaitForCompletion(bool snap)
		{
			ManualResetEvent manualResetEvent = null;
			bool flag = false;
			if (!(snap ? this.IsCompleted : this.InternalPeekCompleted))
			{
				manualResetEvent = (ManualResetEvent)this.internalEvent;
				if (manualResetEvent == null)
				{
					flag = this.LazilyCreateEvent(out manualResetEvent);
				}
			}
			if (manualResetEvent == null)
			{
				goto IL_88;
			}
			try
			{
				try
				{
					if (!manualResetEvent.WaitOne(-1, false))
					{
						throw new TimeoutException(NetException.InternalOperationFailure);
					}
				}
				catch (ObjectDisposedException)
				{
				}
				goto IL_88;
			}
			finally
			{
				if (flag && !this.userEvent)
				{
					ManualResetEvent manualResetEvent2 = (ManualResetEvent)this.internalEvent;
					this.internalEvent = null;
					if (!this.userEvent)
					{
						manualResetEvent2.Close();
					}
				}
			}
			IL_82:
			Thread.SpinWait(1);
			IL_88:
			if (this.result != DBNull.Value)
			{
				return this.result;
			}
			goto IL_82;
		}

		private void WorkerThreadComplete(object state)
		{
			try
			{
				if (this.asyncCallback != null)
				{
					this.asyncCallback(this);
				}
			}
			finally
			{
				ManualResetEvent manualResetEvent = (ManualResetEvent)this.internalEvent;
				if (manualResetEvent != null)
				{
					try
					{
						manualResetEvent.Set();
					}
					catch (ObjectDisposedException)
					{
					}
				}
				this.Cleanup();
			}
		}

		private const bool Trace = false;

		private const int HighBit = -2147483648;

		private const int ForceAsyncCount = 50;

		[ThreadStatic]
		private static LazyAsyncResult.ThreadContext threadContext;

		private object asyncObject;

		private object asyncState;

		private AsyncCallback asyncCallback;

		private object result;

		private int errorCode;

		private int intCompleted;

		private int endCalled;

		private bool userEvent;

		private object internalEvent;

		private class ThreadContext
		{
			internal int nestedIOCount;
		}
	}
}
