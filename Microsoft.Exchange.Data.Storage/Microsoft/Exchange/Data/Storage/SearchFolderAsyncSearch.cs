using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SearchFolderAsyncSearch : IDisposeTrackable, IDisposable
	{
		internal SearchFolderAsyncSearch(StoreSession session, StoreObjectId searchFolderId, AsyncCallback userCallback, object state)
		{
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
			this.state = state;
			this.userCallback = userCallback;
			this.asyncResult = new SearchFolderAsyncSearch.SearchFolderAsyncResult(this);
			this.subscription = Subscription.Create(session, new NotificationHandler(this.NotificationHandler), NotificationType.SearchComplete, searchFolderId, false);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SearchFolderAsyncSearch>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal IAsyncResult AsyncResult
		{
			get
			{
				return this.asyncResult;
			}
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.subscription.Dispose();
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private void NotificationHandler(Notification notification)
		{
			this.searchCompleteEvent.Set();
			if (this.userCallback != null)
			{
				this.userCallback(this.asyncResult);
			}
		}

		private readonly Subscription subscription;

		private readonly SearchFolderAsyncSearch.SearchFolderAsyncResult asyncResult;

		private readonly object state;

		private readonly ManualResetEvent searchCompleteEvent = new ManualResetEvent(false);

		private readonly AsyncCallback userCallback;

		private bool isDisposed;

		private readonly DisposeTracker disposeTracker;

		private class SearchFolderAsyncResult : IAsyncResult
		{
			internal SearchFolderAsyncResult(SearchFolderAsyncSearch searchFolderAsyncSearch)
			{
				this.searchFolderAsyncSearch = searchFolderAsyncSearch;
			}

			public object AsyncState
			{
				get
				{
					return this.searchFolderAsyncSearch.state;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.searchFolderAsyncSearch.searchCompleteEvent;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.searchFolderAsyncSearch.searchCompleteEvent.WaitOne(0);
				}
			}

			private readonly SearchFolderAsyncSearch searchFolderAsyncSearch;
		}
	}
}
