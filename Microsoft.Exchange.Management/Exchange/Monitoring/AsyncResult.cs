using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Monitoring
{
	internal class AsyncResult<T> : IAsyncResult, IDisposable where T : TransactionOutcomeBase
	{
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException("AsyncResult");
				}
				if (this.asyncEvent == null)
				{
					lock (this.lockObject)
					{
						if (this.asyncEvent == null)
						{
							this.asyncEvent = new ManualResetEvent(this.completed);
						}
					}
				}
				return this.asyncEvent;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.completed;
			}
		}

		public object AsyncState
		{
			get
			{
				return this.Outcomes;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		public List<T> Outcomes
		{
			get
			{
				if (this.outcomes == null)
				{
					this.outcomes = new List<T>();
				}
				return this.outcomes;
			}
		}

		internal void Complete()
		{
			if (!this.completed)
			{
				if (this.asyncEvent != null && !this.disposed)
				{
					lock (this.lockObject)
					{
						if (!this.completed)
						{
							this.completed = true;
							if (this.asyncEvent != null && !this.disposed)
							{
								this.asyncEvent.Set();
							}
						}
						return;
					}
				}
				this.completed = true;
			}
		}

		internal void SetTimeout()
		{
			if (!this.timeout)
			{
				lock (this.lockObject)
				{
					this.timeout = true;
				}
			}
		}

		public bool DidTimeout()
		{
			return this.timeout;
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				lock (this.lockObject)
				{
					if (!this.disposed)
					{
						this.disposed = true;
						if (this.asyncEvent != null)
						{
							this.asyncEvent.Close();
							this.asyncEvent = null;
						}
					}
				}
			}
		}

		private bool completed;

		private bool timeout;

		private ManualResetEvent asyncEvent;

		private object lockObject = new object();

		private List<T> outcomes;

		private bool disposed;
	}
}
