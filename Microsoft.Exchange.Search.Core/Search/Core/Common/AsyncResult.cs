using System;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class AsyncResult : LazyAsyncResult
	{
		public AsyncResult(AsyncCallback asyncCallback, object state) : base(null, state, asyncCallback)
		{
		}

		public static AsyncResult EndAsyncOperation(IAsyncResult asyncResult)
		{
			AsyncResult asyncResult2 = LazyAsyncResult.EndAsyncOperation<AsyncResult>(asyncResult);
			if (asyncResult2.Result is Exception)
			{
				throw (Exception)asyncResult2.Result;
			}
			return asyncResult2;
		}

		public void SetAsCompleted()
		{
			base.InvokeCallback();
		}

		public void SetAsCompleted(ComponentException exception)
		{
			base.InvokeCallback(exception);
		}

		public void End()
		{
			AsyncResult.EndAsyncOperation(this);
		}
	}
}
