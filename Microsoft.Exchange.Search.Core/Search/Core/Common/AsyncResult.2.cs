using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class AsyncResult<TResult> : AsyncResult
	{
		public AsyncResult(AsyncCallback asyncCallback, object state) : base(asyncCallback, state)
		{
		}

		public void SetAsCompleted(TResult result)
		{
			this.result = result;
			base.InvokeCallback();
		}

		public new TResult End()
		{
			base.End();
			return this.result;
		}

		private TResult result;
	}
}
