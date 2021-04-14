using System;
using System.Threading;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class OwaAsyncResult : IAsyncResult
	{
		internal OwaAsyncResult(AsyncCallback callback, object extraData)
		{
			this.callback = callback;
			this.extraData = extraData;
		}

		public object AsyncState
		{
			get
			{
				return this.extraData;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				bool result;
				lock (this)
				{
					result = this.completedSynchronously;
				}
				return result;
			}
		}

		public bool IsCompleted
		{
			get
			{
				bool result;
				lock (this)
				{
					result = this.isCompleted;
				}
				return result;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (this.setEvent == null)
				{
					this.setEvent = new ManualResetEvent(false);
				}
				return this.setEvent;
			}
		}

		public Exception Exception
		{
			get
			{
				Exception result;
				lock (this)
				{
					result = this.exception;
				}
				return result;
			}
			set
			{
				lock (this)
				{
					if (this.isCompleted || this.exception != null)
					{
						throw new OwaInvalidOperationException("The request is already finished, or an exception was already registered for this OwaAsyncResult." + ((this.exception != null) ? ("Previous exception message: " + this.exception.Message) : string.Empty) + ((value != null) ? (" Current exception message: " + value.Message) : string.Empty));
					}
					this.exception = value;
				}
			}
		}

		internal void CompleteRequest(bool completedSynchronously)
		{
			lock (this)
			{
				if (this.isCompleted)
				{
					return;
				}
				this.isCompleted = true;
				this.completedSynchronously = completedSynchronously;
			}
			if (this.callback != null)
			{
				this.callback(this);
			}
			if (this.setEvent != null)
			{
				this.setEvent.Set();
			}
		}

		internal void CompleteRequest(bool completedSynchronously, Exception exception)
		{
			this.Exception = exception;
			this.CompleteRequest(completedSynchronously);
		}

		private bool isCompleted;

		private bool completedSynchronously;

		private AsyncCallback callback;

		private object extraData;

		private ManualResetEvent setEvent;

		private Exception exception;
	}
}
