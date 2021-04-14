using System;
using System.Threading;

namespace Microsoft.Filtering
{
	internal class FipsDataStreamFilteringAsyncResult : IAsyncResult
	{
		public FipsDataStreamFilteringAsyncResult(Func<AsyncCallback, IAsyncResult> beginOperation, AsyncCallback callback, FipsDataStreamFilteringRequest fipsDataStreamFilteringRequest)
		{
			FipsDataStreamFilteringAsyncResult <>4__this = this;
			this.FipsDataStreamFilteringRequest = fipsDataStreamFilteringRequest;
			this.InnerAsyncResult = beginOperation(delegate(IAsyncResult ar)
			{
				IAsyncResult innerAsyncResult = <>4__this.InnerAsyncResult;
				<>4__this.InnerAsyncResult = ar;
				if (callback != null)
				{
					callback(<>4__this);
				}
			});
		}

		public IAsyncResult InnerAsyncResult { get; private set; }

		public FipsDataStreamFilteringRequest FipsDataStreamFilteringRequest { get; private set; }

		public bool IsCompleted
		{
			get
			{
				return this.InnerAsyncResult.IsCompleted;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return this.InnerAsyncResult.AsyncWaitHandle;
			}
		}

		public object AsyncState
		{
			get
			{
				return this.InnerAsyncResult.AsyncState;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.InnerAsyncResult.CompletedSynchronously;
			}
		}
	}
}
