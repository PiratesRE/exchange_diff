using System;
using System.Threading;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class SenderIdAsyncResult : IAsyncResult
	{
		public SenderIdAsyncResult(AsyncCallback asyncCallback, object asyncState)
		{
			this.asyncCallback = asyncCallback;
			this.asyncState = asyncState;
		}

		public SenderIdAsyncResult(AsyncCallback asyncCallback, object asyncState, object result) : this(asyncCallback, asyncState)
		{
			this.InvokeCompleted(result);
		}

		public object GetResult()
		{
			if (!this.isCompleted)
			{
				throw new InvalidOperationException();
			}
			return this.result;
		}

		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
		}

		public void InvokeCompleted(object invokeResult)
		{
			this.result = invokeResult;
			this.isCompleted = true;
			if (this.asyncCallback != null)
			{
				this.asyncCallback(this);
			}
		}

		private AsyncCallback asyncCallback;

		private object asyncState;

		private bool isCompleted;

		private object result;
	}
}
