using System;
using System.Threading;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ServiceAsyncResult<T> : IAsyncResult
	{
		internal ServiceAsyncResult()
		{
			this.isCompleted = false;
		}

		internal void Complete(object state)
		{
			if (!this.isCompleted)
			{
				lock (this.instanceLock)
				{
					if (!this.isCompleted)
					{
						this.completionState = state;
						this.isCompleted = true;
						if (this.asyncCallback != null)
						{
							this.asyncCallback(this);
						}
						if (this.completedEvent != null)
						{
							this.completedEvent.Set();
						}
					}
				}
			}
		}

		public object CompletionState
		{
			get
			{
				return this.completionState;
			}
		}

		public T Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		public AsyncCallback AsyncCallback
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

		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
			set
			{
				this.asyncState = value;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (this.completedEvent == null)
				{
					lock (this.instanceLock)
					{
						if (this.completedEvent == null)
						{
							this.completedEvent = new ManualResetEvent(false);
						}
					}
				}
				return this.completedEvent;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
		}

		private AsyncCallback asyncCallback;

		private object asyncState;

		private T data;

		private bool isCompleted;

		private object completionState;

		private object instanceLock = new object();

		private volatile ManualResetEvent completedEvent;
	}
}
