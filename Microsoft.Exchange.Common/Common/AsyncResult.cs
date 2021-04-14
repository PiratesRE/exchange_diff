using System;
using System.Threading;

namespace Microsoft.Exchange.Common
{
	public class AsyncResult : IAsyncResult, IDisposable
	{
		public AsyncResult(AsyncCallback callback, object asyncState)
		{
			this.Callback = callback;
			this.AsyncState = asyncState;
			this.IsCompleted = false;
			this.completedSynchronously = false;
		}

		public AsyncResult(AsyncResultCallback callback, object asyncState)
		{
			this.AsyncResultCallback = callback;
			this.AsyncState = asyncState;
			this.IsCompleted = false;
			this.completedSynchronously = false;
		}

		public AsyncResult(AsyncCallback callback, object asyncState, bool completedSynchronously)
		{
			this.Callback = callback;
			this.AsyncState = asyncState;
			this.completedSynchronously = completedSynchronously;
			this.IsCompleted = this.completedSynchronously;
		}

		public AsyncResult(AsyncResultCallback callback, object asyncState, bool completedSynchronously)
		{
			this.AsyncResultCallback = callback;
			this.AsyncState = asyncState;
			this.completedSynchronously = completedSynchronously;
			this.IsCompleted = this.completedSynchronously;
		}

		internal AsyncResult(AsyncEnumerator enumerator, AsyncResultCallback callback, object asyncState)
		{
			this.asyncEnumerator = enumerator;
			this.AsyncResultCallback = callback;
			this.AsyncState = asyncState;
		}

		protected AsyncResult(object asyncState, bool completedSynchronously)
		{
			this.AsyncState = asyncState;
			this.completedSynchronously = completedSynchronously;
		}

		public void Dispose()
		{
			if (this.completedEvent != null)
			{
				((IDisposable)this.completedEvent).Dispose();
			}
			if (this.asyncEnumerator != null)
			{
				this.asyncEnumerator.Dispose();
			}
		}

		public object AsyncState { get; protected set; }

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (this.completedEvent == null)
				{
					this.completedEvent = new ManualResetEvent(this.isCompleted);
				}
				return this.completedEvent;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.completedSynchronously;
			}
			internal set
			{
				this.completedSynchronously = value;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
			set
			{
				if (this.isCompleted)
				{
					throw new InvalidOperationException("Can't set completed multiple times");
				}
				this.isCompleted = value;
				if (this.isCompleted)
				{
					if (this.completedEvent != null)
					{
						this.completedEvent.Set();
					}
					this.InvokeCallback();
				}
			}
		}

		public bool IsAborted { get; private set; }

		public Exception Exception { get; internal set; }

		public void Abort()
		{
			if (!this.IsCompleted)
			{
				try
				{
					if (this.OnAbort != null)
					{
						this.OnAbort();
					}
				}
				finally
				{
					this.IsCompleted = true;
					this.IsAborted = true;
				}
			}
		}

		public void End()
		{
			try
			{
				if (this.Exception != null)
				{
					throw AsyncExceptionWrapperHelper.GetAsyncWrapper(this.Exception);
				}
			}
			finally
			{
				this.Dispose();
			}
		}

		protected AsyncCallback Callback { get; set; }

		private protected AsyncResultCallback AsyncResultCallback { protected get; private set; }

		public event Action OnAbort;

		protected virtual void InvokeCallback()
		{
			if (this.AsyncResultCallback != null)
			{
				this.AsyncResultCallback(this);
				return;
			}
			if (this.Callback != null)
			{
				this.Callback(this);
			}
		}

		private volatile ManualResetEvent completedEvent;

		private volatile bool isCompleted;

		private bool completedSynchronously;

		private AsyncEnumerator asyncEnumerator;
	}
}
