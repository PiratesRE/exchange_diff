using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AsyncResult<TState, TResultData> : LazyAsyncResult where TResultData : class
	{
		public AsyncResult(object asyncOperator, TState state, AsyncCallback callback, object callbackState, object syncPoisonContext) : base(asyncOperator, callbackState, callback)
		{
			this.state = state;
			this.syncPoisonContext = syncPoisonContext;
			this.SetPoisonContextOnCurrentThread();
		}

		public object SyncPoisonContext
		{
			get
			{
				return this.syncPoisonContext;
			}
		}

		public object AsyncOperator
		{
			get
			{
				return base.AsyncObject;
			}
		}

		public TState State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

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

		public IAsyncResult PendingAsyncResult
		{
			get
			{
				return this.pendingAsyncResult;
			}
			set
			{
				this.pendingAsyncResult = value;
			}
		}

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
				if (this.Result == null)
				{
					return null;
				}
				return this.Result.Exception;
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

		private new WaitHandle AsyncWaitHandle
		{
			get
			{
				throw new InvalidOperationException("Invalid usage of AsyncResult. This method is not to be used outside of internal AsyncResult implementation.");
			}
		}

		public static WaitCallback GetWaitCallbackWithClearPoisonContext(WaitCallback userCallback)
		{
			if (userCallback == null)
			{
				return null;
			}
			return delegate(object state)
			{
				userCallback(state);
				AsyncResult<TState, TResultData>.ClearPoisonContextFromCurrentThread();
			};
		}

		private static void ClearPoisonContextFromCurrentThread()
		{
			SyncPoisonHandler.ClearSyncPoisonContextFromCurrentThread();
		}

		public WaitCallback GetWaitCallbackWithPoisonContext(WaitCallback userCallback)
		{
			if (userCallback == null)
			{
				return null;
			}
			return delegate(object state)
			{
				this.SetPoisonContextOnCurrentThread();
				userCallback(state);
				AsyncResult<TState, TResultData>.ClearPoisonContextFromCurrentThread();
			};
		}

		public AsyncCallback GetAsyncCallbackWithPoisonContext(AsyncCallback userCallback)
		{
			if (userCallback == null)
			{
				return null;
			}
			return delegate(IAsyncResult asyncResult)
			{
				this.SetPoisonContextOnCurrentThread();
				userCallback(asyncResult);
				AsyncResult<TState, TResultData>.ClearPoisonContextFromCurrentThread();
			};
		}

		public AsyncCallback GetAsyncCallbackWithPoisonContextAndUnhandledExceptionRedirect(AsyncCallback userCallback)
		{
			if (userCallback == null)
			{
				return null;
			}
			return delegate(IAsyncResult asyncResult)
			{
				this.SetPoisonContextOnCurrentThread();
				try
				{
					userCallback(asyncResult);
					AsyncResult<TState, TResultData>.ClearPoisonContextFromCurrentThread();
				}
				catch (Exception exception)
				{
					Exception exception2;
					Exception exception = exception2;
					ThreadPool.QueueUserWorkItem(delegate(object state)
					{
						this.SetPoisonContextOnCurrentThread();
						throw new SyncUnhandledException(exception.GetType(), exception);
					});
				}
			};
		}

		public CancelableAsyncCallback GetCancelableAsyncCallbackWithPoisonContextAndUnhandledExceptionRedirect(CancelableAsyncCallback userCallback)
		{
			if (userCallback == null)
			{
				return null;
			}
			return delegate(ICancelableAsyncResult asyncResult)
			{
				this.SetPoisonContextOnCurrentThread();
				try
				{
					userCallback(asyncResult);
					AsyncResult<TState, TResultData>.ClearPoisonContextFromCurrentThread();
				}
				catch (Exception exception)
				{
					Exception exception2;
					Exception exception = exception2;
					SyncUtilities.RunUserWorkItemOnNewThreadAndBlockCurrentThread(delegate
					{
						this.SetPoisonContextOnCurrentThread();
						throw new SyncUnhandledException(exception.GetType(), exception);
					});
				}
			};
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
			this.SetPoisonContextOnCurrentThread();
			this.pendingAsyncResult = null;
			this.ProtectedProcessCompleted(result, exception);
			AsyncOperationResult<TResultData> value = new AsyncOperationResult<TResultData>(result, exception);
			base.InvokeCallback(value);
			AsyncResult<TState, TResultData>.ClearPoisonContextFromCurrentThread();
		}

		private void SetPoisonContextOnCurrentThread()
		{
			SyncPoisonHandler.SetSyncPoisonContextOnCurrentThread(this.syncPoisonContext);
		}

		private new void InvokeCallback()
		{
			throw new InvalidOperationException("Invalid usage of AsyncResult. Use ProcessCompleted.");
		}

		private new void InvokeCallback(object value)
		{
			throw new InvalidOperationException("Invalid usage of AsyncResult. Use ProcessCompleted.");
		}

		private new void InternalCleanup()
		{
			throw new InvalidOperationException("Invalid usage of AsyncResult. This method is not to be used outside of internal AsyncResult implementation.");
		}

		private new object InternalWaitForCompletion()
		{
			throw new InvalidOperationException("Invalid usage of AsyncResult. This method is not to be used outside of internal AsyncResult implementation.");
		}

		private new object InternalWaitForCompletionNoSideEffects()
		{
			throw new InvalidOperationException("Invalid usage of AsyncResult. This method is not to be used outside of internal AsyncResult implementation.");
		}

		private readonly object syncPoisonContext;

		private TState state;

		private bool completedSynchronously;

		private IAsyncResult pendingAsyncResult;
	}
}
