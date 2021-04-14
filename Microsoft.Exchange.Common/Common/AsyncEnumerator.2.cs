using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Common
{
	public class AsyncEnumerator<T> : AsyncEnumerator
	{
		public AsyncEnumerator(AsyncResultCallback<T> callback, object asyncState, Func<AsyncEnumerator<T>, IEnumerator<int>> enumeratorCallback) : this(callback, asyncState, enumeratorCallback, true)
		{
		}

		public AsyncEnumerator(AsyncCallback callback, object asyncState, Func<AsyncEnumerator<T>, IEnumerator<int>> enumeratorCallback) : this(callback, asyncState, enumeratorCallback, true)
		{
		}

		public AsyncEnumerator(AsyncResultCallback<T> callback, object asyncState, Func<AsyncEnumerator<T>, IEnumerator<int>> enumeratorCallback, bool startAsyncOperation)
		{
			base.AsyncResult = new AsyncResult<T>(this, callback, asyncState);
			this.enumerator = enumeratorCallback(this);
			if (startAsyncOperation)
			{
				base.Begin();
			}
			base.ConstructorDone = true;
		}

		public AsyncEnumerator(AsyncCallback callback, object asyncState, Func<AsyncEnumerator<T>, IEnumerator<int>> enumeratorCallback, bool startAsyncOperation)
		{
			base.AsyncResult = new AsyncResult<T>(this, delegate(AsyncResult<T> ar)
			{
				callback(ar);
			}, asyncState);
			this.enumerator = enumeratorCallback(this);
			if (startAsyncOperation)
			{
				base.Begin();
			}
			base.ConstructorDone = true;
		}

		public new AsyncResult<T> AsyncResult
		{
			get
			{
				return (AsyncResult<T>)base.AsyncResult;
			}
		}

		public void End(T result)
		{
			this.AsyncResult.CompletedSynchronously = !base.ConstructorDone;
			this.AsyncResult.Result = result;
			this.endedWithResult = true;
			base.End();
		}

		protected override void VerifySuccessfullyCompleted()
		{
			if (this.AsyncResult.Exception == null && this.AsyncResult.IsCompleted && !this.endedWithResult)
			{
				throw new InvalidOperationException("Resulted not assigned to async operation");
			}
			base.VerifySuccessfullyCompleted();
		}

		protected override void ThrowForMoreAsyncsAfterCompletion()
		{
			if (this.AsyncResult.IsCompleted)
			{
				throw new InvalidOperationException("Can't do more asyncCalls after End has been called on AsyncEnumerator");
			}
		}

		private bool endedWithResult;
	}
}
