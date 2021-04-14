using System;
using System.Threading;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal sealed class CustomContextAsyncResult : IAsyncResult
	{
		public CustomContextAsyncResult(AsyncCallback originalCallback, object originalState, object customState)
		{
			this.originalState = originalState;
			this.originalCallback = originalCallback;
			this.customState = customState;
		}

		public object AsyncState
		{
			get
			{
				return this.originalState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return this.InnerAsyncResult.AsyncWaitHandle;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.InnerAsyncResult.CompletedSynchronously;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.InnerAsyncResult.IsCompleted;
			}
		}

		internal void CustomCallback(IAsyncResult asyncResult)
		{
			this.InnerAsyncResult = asyncResult;
			if (this.originalCallback != null)
			{
				this.originalCallback(this);
			}
		}

		internal object CustomState
		{
			get
			{
				return this.customState;
			}
		}

		private AsyncCallback originalCallback;

		private object originalState;

		private object customState;

		internal IAsyncResult InnerAsyncResult;
	}
}
