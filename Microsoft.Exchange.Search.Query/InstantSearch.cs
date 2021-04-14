using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Query
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstantSearch : IDisposeTrackable, IDisposable
	{
		public InstantSearch(MailboxSession session, IReadOnlyList<StoreId> folderScope, Guid correlationId) : this(session, folderScope, null, correlationId)
		{
		}

		public InstantSearch(MailboxSession session, IReadOnlyList<StoreId> folderScope, ICollection<PropertyDefinition> requestedProperties, Guid correlationId)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			this.Session = session;
			this.RequestedProperties = requestedProperties;
			this.CorrelationId = correlationId;
			lock (this.Session)
			{
				this.PreferredCulture = this.Session.PreferedCulture;
				this.MdbGuid = this.Session.MdbGuid;
				this.MailboxGuid = this.Session.MailboxGuid;
				this.RootFolderId = this.Session.GetDefaultFolderId(DefaultFolderType.Root);
			}
			if (folderScope == null || folderScope.Count == 0)
			{
				this.FolderScope = new StoreId[]
				{
					this.RootFolderId
				};
			}
			else
			{
				this.FolderScope = folderScope;
			}
			this.Config = new FlightingSearchConfig(this.MdbGuid);
			this.Completions = new Completions(this.Config);
			this.disposeTracker = this.GetDisposeTracker();
		}

		public MailboxSession Session { get; private set; }

		public Action<IReadOnlyCollection<string>, ICancelableAsyncResult> HitHighlightingDataAvailableCallback { get; set; }

		public Action<IReadOnlyCollection<QuerySuggestion>, ICancelableAsyncResult> SuggestionsAvailableCallback { get; set; }

		public Action<IReadOnlyCollection<IReadOnlyPropertyBag>, ICancelableAsyncResult> ResultsUpdatedCallback { get; set; }

		public Action<IReadOnlyCollection<RefinerData>, ICancelableAsyncResult> RefinerDataAvailableCallback { get; set; }

		public ICollection<PropertyDefinition> RequestedProperties { get; private set; }

		public IReadOnlyList<StoreId> FolderScope { get; private set; }

		public bool SearchForConversations { get; set; }

		public bool FastQueryPath { get; set; }

		public int MaximumSuggestionsCount
		{
			[DebuggerStepThrough]
			get
			{
				return this.Completions.MaximumSuggestionsCount;
			}
			[DebuggerStepThrough]
			set
			{
				this.Completions.MaximumSuggestionsCount = value;
			}
		}

		public Guid CorrelationId { get; private set; }

		public IRecipientResolver RecipientResolver { get; set; }

		public IPolicyTagProvider PolicyTagProvider { get; set; }

		internal QueryHistoryInputDictionary QueryHistory
		{
			[DebuggerStepThrough]
			get
			{
				return this.queryHistory;
			}
		}

		internal PagingImsFlowExecutor FlowExecutor
		{
			[DebuggerStepThrough]
			get
			{
				return InstantSearch.ServiceProxyWrapper.Value;
			}
		}

		internal Completions Completions { get; private set; }

		internal InstantSearchSchema Schema { get; private set; }

		internal CultureInfo PreferredCulture { get; private set; }

		internal Guid MdbGuid { get; private set; }

		internal Guid MailboxGuid { get; private set; }

		internal StoreId RootFolderId { get; private set; }

		internal SearchConfig Config { get; private set; }

		private static Hookable<PagingImsFlowExecutor> ServiceProxyWrapper
		{
			get
			{
				if (InstantSearch.serviceProxyWrapperInstance == null)
				{
					string hostName = SearchConfig.Instance.HostName;
					int queryServicePort = SearchConfig.Instance.QueryServicePort;
					int fastImsChannelPoolSize = SearchConfig.Instance.FastImsChannelPoolSize;
					TimeSpan fastImsOpenTimeout = SearchConfig.Instance.FastImsOpenTimeout;
					TimeSpan fastImsSendTimeout = SearchConfig.Instance.FastImsSendTimeout;
					TimeSpan fastImsReceiveTimeout = SearchConfig.Instance.FastImsReceiveTimeout;
					int fastSearchRetryCount = SearchConfig.Instance.FastSearchRetryCount;
					long num = (long)SearchConfig.Instance.FastIMSMaxReceivedMessageSize;
					int fastIMSMaxStringContentLength = SearchConfig.Instance.FastIMSMaxStringContentLength;
					TimeSpan fastProxyCacheTimeout = SearchConfig.Instance.FastProxyCacheTimeout;
					Hookable<PagingImsFlowExecutor> value = Hookable<PagingImsFlowExecutor>.Create(true, new PagingImsFlowExecutor(hostName, queryServicePort, fastImsChannelPoolSize, fastImsOpenTimeout, fastImsSendTimeout, fastImsReceiveTimeout, fastProxyCacheTimeout, num, fastIMSMaxStringContentLength, fastSearchRetryCount));
					Interlocked.CompareExchange<Hookable<PagingImsFlowExecutor>>(ref InstantSearch.serviceProxyWrapperInstance, value, null);
				}
				return InstantSearch.serviceProxyWrapperInstance;
			}
		}

		public static IDisposable SetPagingImsFlowExecutorTestHook(PagingImsFlowExecutor testHook)
		{
			return InstantSearch.ServiceProxyWrapper.SetTestHook(testHook);
		}

		public static ICollection<PropertyDefinition> GetDefaultRequestedProperties(MailboxSession session)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			return InstantSearchSchema.DefaultRequestedProperties;
		}

		public static ICollection<PropertyDefinition> GetDefaultRequestedPropertiesConversations(MailboxSession session)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			return InstantSearchSchema.DefaultRequestedPropertiesConversations;
		}

		public static IReadOnlyCollection<PropertyDefinition> GetDefaultRefinersFAST(MailboxSession session)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			return InstantSearchSchema.DefaultRefinersFAST;
		}

		public static IReadOnlyCollection<PropertyDefinition> GetDefaultRefiners(MailboxSession session)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			return InstantSearchSchema.DefaultRefiners;
		}

		public static IReadOnlyCollection<PropertyDefinition> GetDefaultRefinersConversations(MailboxSession session)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			return InstantSearchSchema.DefaultRefinersConversations;
		}

		public static IReadOnlyCollection<SortBy> GetDefaultSortBySpec(MailboxSession session)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			return InstantSearchSchema.DefaultSortBySpec;
		}

		public static IReadOnlyCollection<SortBy> GetDefaultSortBySpecConversations(MailboxSession session)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			return InstantSearchSchema.DefaultSortBySpecConversations;
		}

		public bool RemoveQueryHistoryItem(string query)
		{
			return this.QueryHistory.Remove(query);
		}

		public void ClearQueryHistory()
		{
			lock (this.Session)
			{
				SearchDictionary.ResetDictionary(this.Session, "Search.QueryHistoryInput", UserConfigurationTypes.Stream, true, false);
			}
			lock (this.QueryHistory)
			{
				this.QueryHistory.Clear();
			}
		}

		public IAsyncResult BeginStartSession(AsyncCallback completionCallback, object state)
		{
			this.InitializeSchema();
			LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(null, state, completionCallback);
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.StartSessionWorker), lazyAsyncResult);
			return lazyAsyncResult;
		}

		public void EndStartSession(IAsyncResult asyncResult)
		{
			LazyAsyncResult lazyAsyncResult = LazyAsyncResult.EndAsyncOperation<LazyAsyncResult>(asyncResult);
			Exception ex = lazyAsyncResult.Result as Exception;
			if (ex != null)
			{
				throw new InstantSearchPermanentException(new LocalizedString(ex.Message), ex, null);
			}
		}

		public IAsyncResult BeginStopSession(AsyncCallback completionCallback, object state)
		{
			LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(null, state, completionCallback);
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.StopSessionWorker), lazyAsyncResult);
			return lazyAsyncResult;
		}

		public void EndStopSession(IAsyncResult asyncResult)
		{
			LazyAsyncResult lazyAsyncResult = LazyAsyncResult.EndAsyncOperation<LazyAsyncResult>(asyncResult);
			Exception ex = lazyAsyncResult.Result as Exception;
			if (ex != null)
			{
				throw new InstantSearchPermanentException(new LocalizedString(ex.Message), ex, null);
			}
		}

		public ICancelableAsyncResult BeginInstantSearchRequest(InstantSearchQueryParameters query, AsyncCallback completionCallback, object state)
		{
			ICancelableAsyncResult result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				InstantSearchRequest instantSearchRequest = disposeGuard.Add<InstantSearchRequest>(new InstantSearchRequest(this, query, state, completionCallback));
				bool flag = false;
				lock (this.lockObject)
				{
					try
					{
						if (this.canceled)
						{
							throw new InvalidOperationException("canceled");
						}
						if (query.HasOption(QueryOptions.ExplicitSearch))
						{
							lock (this.QueryHistory)
							{
								this.QueryHistory.Merge(query.KqlQuery);
							}
							this.accumulatedQueries = true;
						}
						this.activeRequests.Add(instantSearchRequest);
						flag = true;
						instantSearchRequest.StartSearch();
						disposeGuard.Success();
						result = instantSearchRequest;
					}
					catch (OutOfMemoryException)
					{
						throw;
					}
					catch (StackOverflowException)
					{
						throw;
					}
					catch (ThreadAbortException)
					{
						throw;
					}
					catch (Exception ex)
					{
						if (flag)
						{
							this.activeRequests.Remove(instantSearchRequest);
						}
						throw new InstantSearchPermanentException(new LocalizedString(ex.Message), ex, null);
					}
				}
			}
			return result;
		}

		public QueryStatistics EndInstantSearchRequest(ICancelableAsyncResult asyncResult)
		{
			InstantSearchRequest instantSearchRequest = LazyAsyncResult.EndAsyncOperation<InstantSearchRequest>(asyncResult);
			lock (this.lockObject)
			{
				this.activeRequests.Remove(instantSearchRequest);
				if (this.canceled && this.activeRequests.Count == 0 && this.cancelCompleteEvent != null)
				{
					this.cancelCompleteEvent.Set();
				}
			}
			object result = instantSearchRequest.Result;
			instantSearchRequest.Dispose();
			QueryStatistics queryStatistics = result as QueryStatistics;
			if (queryStatistics != null)
			{
				return queryStatistics;
			}
			throw (Exception)result;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<InstantSearch>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal static void ThrowOnNullArgument(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		internal static void ThrowOnNullOrEmptyArgument(IEnumerable argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			if (!argument.GetEnumerator().MoveNext())
			{
				throw new ArgumentException(argumentName);
			}
		}

		private void StartSessionWorker(object state)
		{
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)state;
			Exception value = null;
			try
			{
				this.InitializeQueryHistory();
				this.InitializeTopN();
			}
			catch (OutOfMemoryException)
			{
				throw;
			}
			catch (StackOverflowException)
			{
				throw;
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception ex)
			{
				value = ex;
			}
			lazyAsyncResult.InvokeCallback(value);
		}

		private void StopSessionWorker(object state)
		{
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)state;
			Exception value = null;
			try
			{
				this.Cancel(false);
				this.WaitForCancelToComplete();
				this.WriteQueryHistoryToStore();
			}
			catch (OutOfMemoryException)
			{
				throw;
			}
			catch (StackOverflowException)
			{
				throw;
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception ex)
			{
				value = ex;
			}
			lazyAsyncResult.InvokeCallback(value);
		}

		private void Cancel(bool cancelOutstandingRequests)
		{
			lock (this.lockObject)
			{
				if (this.activeRequests.Count != 0)
				{
					if (cancelOutstandingRequests)
					{
						foreach (InstantSearchRequest instantSearchRequest in this.activeRequests)
						{
							instantSearchRequest.Cancel();
						}
					}
					this.cancelCompleteEvent = new ManualResetEvent(false);
				}
				this.canceled = true;
			}
		}

		private void WaitForCancelToComplete()
		{
			lock (this.lockObject)
			{
				if (this.activeRequests.Count == 0 || this.cancelCompleteEvent == null)
				{
					return;
				}
			}
			this.cancelCompleteEvent.WaitOne();
		}

		private void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.Cancel(true);
				this.WaitForCancelToComplete();
				if (this.cancelCompleteEvent != null)
				{
					this.cancelCompleteEvent.Dispose();
					this.cancelCompleteEvent = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
			}
		}

		private void InitializeTopN()
		{
			lock (this.Session)
			{
				this.Completions.InitializeTopN(this.Session);
			}
		}

		private void InitializeQueryHistory()
		{
			lock (this.Session)
			{
				using (UserConfiguration searchDictionaryItem = SearchDictionary.GetSearchDictionaryItem(this.Session, "Search.QueryHistoryInput"))
				{
					using (Stream stream = searchDictionaryItem.GetStream())
					{
						lock (this.QueryHistory)
						{
							this.QueryHistory.InitializeFrom(stream);
						}
					}
				}
			}
		}

		private void WriteQueryHistoryToStore()
		{
			if (this.accumulatedQueries)
			{
				lock (this.Session)
				{
					using (UserConfiguration searchDictionaryItem = SearchDictionary.GetSearchDictionaryItem(this.Session, "Search.QueryHistoryInput"))
					{
						using (Stream stream = searchDictionaryItem.GetStream())
						{
							lock (this.QueryHistory)
							{
								this.QueryHistory.SerializeTo(stream);
							}
							searchDictionaryItem.Save(SaveMode.NoConflictResolutionForceSave);
						}
					}
				}
			}
		}

		private void InitializeSchema()
		{
			if (this.Schema != null)
			{
				return;
			}
			bool value;
			if (this.RequestedProperties != null)
			{
				this.Schema = new InstantSearchSchema(this.RequestedProperties);
				value = !this.Schema.HasUnsupportedXsoProperties;
			}
			else
			{
				value = true;
				if (this.SearchForConversations)
				{
					this.Schema = InstantSearchSchema.DefaultConversationsSchema;
				}
				else
				{
					this.Schema = InstantSearchSchema.DefaultSchema;
				}
			}
			this.FastQueryPath = this.DetermineFastQueryPath(value);
		}

		private bool DetermineFastQueryPath(bool value)
		{
			switch (this.Config.FastQueryPath)
			{
			case 0:
				return false;
			case 1:
				return true;
			default:
				return value;
			}
		}

		private static Hookable<PagingImsFlowExecutor> serviceProxyWrapperInstance;

		private readonly object lockObject = new object();

		private List<InstantSearchRequest> activeRequests = new List<InstantSearchRequest>();

		private QueryHistoryInputDictionary queryHistory = new QueryHistoryInputDictionary();

		private bool accumulatedQueries;

		private volatile bool canceled;

		private ManualResetEvent cancelCompleteEvent;

		private DisposeTracker disposeTracker;
	}
}
