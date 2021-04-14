using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal abstract class MailboxSearchGroup : DisposeTrackableBase
	{
		protected MailboxSearchGroup(MailboxInfo[] mailboxes, SearchCriteria searchCriteria, PagingInfo pagingInfo, CallerInfo executingUser)
		{
			this.mailboxes = mailboxes;
			this.searchCriteria = searchCriteria;
			this.pagingInfo = pagingInfo;
			this.executingUser = executingUser;
			this.resultAggregator = new ResultAggregator();
		}

		protected ISearchResult ResultAggregator
		{
			get
			{
				return this.resultAggregator;
			}
		}

		protected SearchCriteria SearchCriteria
		{
			get
			{
				return this.searchCriteria;
			}
		}

		protected PagingInfo PagingInfo
		{
			get
			{
				return this.pagingInfo;
			}
		}

		protected CallerInfo ExecutingUser
		{
			get
			{
				return this.executingUser;
			}
		}

		protected MailboxInfo[] Mailboxes
		{
			get
			{
				return this.mailboxes;
			}
		}

		public static IDisposable SetAbortSearchTestHook(Action action)
		{
			return MailboxSearchGroup.abortSearchTestHook.SetTestHook(action);
		}

		public IAsyncResult BeginExecuteSearch(AsyncCallback callback, object state)
		{
			if (this.searchState != MailboxSearchGroup.SearchState.NotStarted)
			{
				throw new InvalidOperationException();
			}
			this.ExecuteSearch();
			this.asyncResult = new AsyncResult(callback, state);
			this.searchState = MailboxSearchGroup.SearchState.Running;
			return this.asyncResult;
		}

		public ISearchResult EndExecuteSearch(IAsyncResult result)
		{
			Util.ThrowOnNull(result, "result");
			if (!this.asyncResult.Equals(result))
			{
				throw new ArgumentException("asyncResult doesn't match with the group's async result");
			}
			this.asyncResult.AsyncWaitHandle.WaitOne();
			return this.resultAggregator;
		}

		public void Abort()
		{
			if (this.searchState != MailboxSearchGroup.SearchState.Running)
			{
				return;
			}
			if (MailboxSearchGroup.abortSearchTestHook.Value != null)
			{
				MailboxSearchGroup.abortSearchTestHook.Value();
			}
			this.StopSearch();
			this.searchState = MailboxSearchGroup.SearchState.Stopped;
		}

		protected virtual void ReportCompletion()
		{
			this.searchState = MailboxSearchGroup.SearchState.Completed;
			this.asyncResult.ReportCompletion();
		}

		protected abstract void ExecuteSearch();

		protected abstract void StopSearch();

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				try
				{
					this.Abort();
				}
				finally
				{
					if (this.asyncResult != null)
					{
						this.asyncResult.Dispose();
						this.asyncResult = null;
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxSearchGroup>(this);
		}

		private static Hookable<Action> abortSearchTestHook = Hookable<Action>.Create(false, null);

		private readonly ISearchResult resultAggregator;

		private readonly MailboxInfo[] mailboxes;

		private readonly SearchCriteria searchCriteria;

		private readonly PagingInfo pagingInfo;

		private readonly CallerInfo executingUser;

		private MailboxSearchGroup.SearchState searchState;

		private AsyncResult asyncResult;

		protected enum SearchState
		{
			NotStarted,
			Running,
			Stopped,
			Completed
		}
	}
}
