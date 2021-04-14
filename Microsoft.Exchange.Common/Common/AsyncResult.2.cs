using System;

namespace Microsoft.Exchange.Common
{
	public class AsyncResult<T> : AsyncResult
	{
		public AsyncResult(AsyncResultCallback<T> callback, object asyncState) : base(delegate(AsyncResult ar)
		{
			callback((AsyncResult<T>)ar);
		}, asyncState)
		{
			this.AsyncResultCallback = callback;
		}

		public AsyncResult(AsyncCallback callback, object asyncState) : base(callback, asyncState)
		{
		}

		public AsyncResult(AsyncResultCallback<T> callback, object asyncState, T result) : base(asyncState, true)
		{
			this.AsyncResultCallback = callback;
			this.Result = result;
			base.IsCompleted = true;
		}

		public AsyncResult(AsyncCallback callback, object asyncState, T result) : base(asyncState, true)
		{
			base.Callback = callback;
			this.Result = result;
			base.IsCompleted = true;
		}

		public AsyncResult(AsyncEnumerator<T> enumerator, AsyncResultCallback<T> callback, object asyncState) : base(enumerator, null, asyncState)
		{
			this.AsyncResultCallback = callback;
		}

		public T Result { get; internal set; }

		public new T End()
		{
			T result;
			try
			{
				if (base.Exception != null)
				{
					throw AsyncExceptionWrapperHelper.GetAsyncWrapper(base.Exception);
				}
				result = this.Result;
			}
			finally
			{
				base.Dispose();
			}
			return result;
		}

		private protected new AsyncResultCallback<T> AsyncResultCallback { protected get; private set; }

		protected override void InvokeCallback()
		{
			if (this.AsyncResultCallback != null)
			{
				this.AsyncResultCallback(this);
				return;
			}
			base.InvokeCallback();
		}
	}
}
