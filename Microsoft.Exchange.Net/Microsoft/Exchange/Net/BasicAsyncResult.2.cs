using System;

namespace Microsoft.Exchange.Net
{
	internal class BasicAsyncResult<TResult> : BasicAsyncResult
	{
		public BasicAsyncResult(AsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
			this.Result = default(TResult);
		}

		public TResult Result { get; set; }

		public void Complete(TResult result, bool completedSynchronously = false)
		{
			this.Result = result;
			base.Complete(completedSynchronously);
		}

		public new TResult End()
		{
			base.End();
			return this.Result;
		}
	}
}
