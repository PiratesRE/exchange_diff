using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AsyncResult<TState, TResultData> : LazyAsyncResult where TResultData : class
	{
		public AsyncResult(object asyncOperator, TState state, AsyncCallback callback, object callbackState) : base(asyncOperator, callbackState, callback)
		{
			this.State = state;
		}

		public TState State { get; private set; }

		public new bool CompletedSynchronously
		{
			get
			{
				return base.CompletedSynchronously && (this.completedSynchronously || base.CompletedSynchronously);
			}
		}

		public new AsyncCallback AsyncCallback
		{
			get
			{
				return base.AsyncCallback;
			}
		}

		public IAsyncResult PendingAsyncResult { get; set; }

		public bool IsCanceled
		{
			get
			{
				return this.Result != null && this.Result.IsCanceled;
			}
		}

		public Exception Exception
		{
			get
			{
				if (this.Result != null)
				{
					return this.Result.Exception;
				}
				return null;
			}
		}

		public new AsyncOperationResult<TResultData> Result
		{
			get
			{
				return (AsyncOperationResult<TResultData>)base.Result;
			}
		}

		public bool IsRetryable
		{
			get
			{
				return this.Exception is TransientException;
			}
		}

		public void SetCompletedSynchronously()
		{
			this.completedSynchronously = true;
		}

		public AsyncOperationResult<TResultData> WaitForCompletion()
		{
			return (AsyncOperationResult<TResultData>)base.InternalWaitForCompletion();
		}

		public void ProcessCompleted(TResultData result)
		{
			this.InternalProcessCompleted(result, null);
		}

		public void ProcessCompleted(Exception exception)
		{
			this.InternalProcessCompleted(default(TResultData), exception);
		}

		public void ProcessCompleted(TResultData result, Exception exception)
		{
			this.InternalProcessCompleted(result, exception);
		}

		public void ProcessCompleted()
		{
			this.InternalProcessCompleted(default(TResultData), null);
		}

		public void ProcessCanceled()
		{
			this.InternalProcessCompleted(default(TResultData), AsyncOperationResult<TResultData>.CanceledException);
		}

		protected virtual void ProtectedProcessCompleted(TResultData result, Exception exception)
		{
		}

		private void InternalProcessCompleted(TResultData result, Exception exception)
		{
			this.PendingAsyncResult = null;
			this.ProtectedProcessCompleted(result, exception);
			AsyncOperationResult<TResultData> value = new AsyncOperationResult<TResultData>(result, exception);
			base.InvokeCallback(value);
		}

		private bool completedSynchronously;
	}
}
