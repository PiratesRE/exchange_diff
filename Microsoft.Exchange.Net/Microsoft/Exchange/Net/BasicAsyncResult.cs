using System;
using System.Threading;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Net
{
	internal class BasicAsyncResult : IAsyncResult
	{
		public BasicAsyncResult(AsyncCallback asyncCallback, object asyncState)
		{
			this.internalState = 0;
			this.AsyncCallback = asyncCallback;
			this.AsyncState = asyncState;
		}

		public object AsyncState { get; private set; }

		public bool CompletedSynchronously
		{
			get
			{
				int num = Interlocked.CompareExchange(ref this.internalState, 1, 1);
				return num == 1 || num == 3;
			}
		}

		public bool IsCompleted
		{
			get
			{
				int num = Interlocked.CompareExchange(ref this.internalState, 0, 0);
				return num != 0;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (this.completionEvent == null)
				{
					bool isCompleted = this.IsCompleted;
					ManualResetEvent manualResetEvent = new ManualResetEvent(isCompleted);
					if (Interlocked.CompareExchange<ManualResetEvent>(ref this.completionEvent, manualResetEvent, null) != null)
					{
						manualResetEvent.Close();
					}
					else if (!isCompleted && this.IsCompleted)
					{
						this.completionEvent.Set();
					}
				}
				return this.completionEvent;
			}
		}

		public int InternalState
		{
			get
			{
				return this.internalState;
			}
		}

		private Exception Exception { get; set; }

		public void Complete(Exception exception, bool completedSynchronously = false)
		{
			this.Exception = exception;
			this.Complete(completedSynchronously);
		}

		public void Complete(bool completedSynchronously = false)
		{
			int num = Interlocked.Exchange(ref this.internalState, completedSynchronously ? 1 : 2);
			if (num != 0)
			{
				throw new InvalidOperationException("You can complete a result only once");
			}
			if (this.completionEvent != null)
			{
				this.completionEvent.Set();
			}
			try
			{
				this.InvokeCallback();
			}
			catch (Exception ex)
			{
				if (this.ShouldThrowCallbackException(ex))
				{
					throw;
				}
			}
		}

		private protected AsyncCallback AsyncCallback { protected get; private set; }

		public void End()
		{
			if (!this.IsCompleted)
			{
				this.AsyncWaitHandle.WaitOne();
			}
			int num;
			if (this.CompletedSynchronously)
			{
				num = Interlocked.CompareExchange(ref this.internalState, 3, 1);
			}
			else
			{
				num = Interlocked.CompareExchange(ref this.internalState, 4, 2);
			}
			if (num == 1 || num == 2)
			{
				this.AsyncWaitHandle.Close();
			}
			if (this.Exception != null)
			{
				Exception ex = this.CreateEndException(this.Exception);
				if (ex != null)
				{
					throw ex;
				}
			}
		}

		protected virtual void InvokeCallback()
		{
			if (this.AsyncCallback != null)
			{
				this.AsyncCallback(this);
			}
		}

		protected virtual bool ShouldThrowCallbackException(Exception ex)
		{
			return ex is OutOfMemoryException || ex is StackOverflowException || ex is ThreadAbortException;
		}

		protected virtual Exception CreateEndException(Exception currentException)
		{
			return AsyncExceptionWrapperHelper.GetAsyncWrapper(currentException);
		}

		private const int StatePending = 0;

		private const int StateCompletedSync = 1;

		private const int StateCompletedASync = 2;

		private const int StateEndedSync = 3;

		private const int StateEndedASync = 4;

		private int internalState;

		private ManualResetEvent completionEvent;
	}
}
