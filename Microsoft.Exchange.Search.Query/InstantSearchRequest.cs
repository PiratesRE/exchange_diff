using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Query
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class InstantSearchRequest : LazyAsyncResult, ICancelableAsyncResult, IAsyncResult, IDisposeTrackable, IDisposable
	{
		internal InstantSearchRequest(InstantSearch session, InstantSearchQueryParameters query, object callerState, AsyncCallback callback) : base(null, callerState, callback)
		{
			if (query.HasOption(QueryOptions.SuggestionsPrimer) && session.SuggestionsAvailableCallback == null)
			{
				throw new ArgumentException("QueryOptions.SuggestionsPrimer without SuggestionsAvailableCallback");
			}
			if (query.HasOption(QueryOptions.Suggestions) && session.SuggestionsAvailableCallback == null)
			{
				throw new ArgumentException("QueryOptions.Suggestions without SuggestionsAvailableCallback");
			}
			if (query.HasOption(QueryOptions.Results) && session.ResultsUpdatedCallback == null)
			{
				throw new ArgumentException("QueryOptions.Results without ResultsUpdatedCallback");
			}
			if (query.HasOption(QueryOptions.Refiners) && session.RefinerDataAvailableCallback == null)
			{
				throw new ArgumentException("QueryOptions.Refiners without RefinerDataAvailableCallback");
			}
			if (query.HasOption(QueryOptions.SearchTerms) && session.HitHighlightingDataAvailableCallback == null)
			{
				throw new ArgumentException("QueryOptions.SearchTerms without HitHighlightingDataAvailableCallback");
			}
			this.session = session;
			this.query = query;
			this.traceContext = session.GetHashCode();
			this.queryStatistics = new QueryStatistics(this.traceContext);
			this.queryStatistics.LightningEnabled = !this.session.Config.DisableInstantSearch;
			this.activityScope = ActivityContext.GetCurrentActivityScope();
			if (this.session.Config.InstantSearchMaxResultsOverride > 0)
			{
				this.maxResultsCount = new int?(this.session.Config.InstantSearchMaxResultsOverride);
			}
			else
			{
				this.maxResultsCount = this.query.MaximumResultCount;
			}
			lock (this.session.Session)
			{
				this.sortSpec = (this.query.SortSpec ?? (this.session.SearchForConversations ? InstantSearch.GetDefaultSortBySpecConversations(this.session.Session) : InstantSearch.GetDefaultSortBySpec(this.session.Session))).ToArray<SortBy>();
				this.useFastQueryPath = (session.FastQueryPath && session.Schema.SortOrderSupportedByFast(this.sortSpec));
				this.queryStatistics.StoreBypassed = this.useFastQueryPath;
				this.requestedRefiners = (this.query.Refiners ?? (this.useFastQueryPath ? InstantSearch.GetDefaultRefinersFAST(this.session.Session) : (this.session.SearchForConversations ? InstantSearch.GetDefaultRefinersConversations(this.session.Session) : InstantSearch.GetDefaultRefiners(this.session.Session))));
			}
			this.disposeTracker = this.GetDisposeTracker();
		}

		public bool IsCanceled { get; private set; }

		internal static Action<QueryFilter> TestQueryHandler { get; set; }

		public void Cancel()
		{
			lock (this.loopLock)
			{
				if (!this.IsCanceled)
				{
					this.IsCanceled = true;
					if (!this.loopRunning)
					{
						this.loopRunning = true;
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.MainLoop));
					}
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<InstantSearchRequest>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal void StartSearch()
		{
			this.outstandingOperations = 1;
			this.NotifyLoop(InstantSearchRequest.LoopNotifications.Startup);
		}

		private static void GetHitHighlightingInfo(QueryFilter filter, HashSet<string> resultSink)
		{
			AndFilter andFilter = filter as AndFilter;
			if (andFilter != null)
			{
				foreach (QueryFilter filter2 in andFilter.Filters)
				{
					InstantSearchRequest.GetHitHighlightingInfo(filter2, resultSink);
				}
				return;
			}
			OrFilter orFilter = filter as OrFilter;
			if (orFilter != null)
			{
				foreach (QueryFilter filter3 in orFilter.Filters)
				{
					InstantSearchRequest.GetHitHighlightingInfo(filter3, resultSink);
				}
				return;
			}
			NearFilter nearFilter = filter as NearFilter;
			if (nearFilter != null)
			{
				InstantSearchRequest.GetHitHighlightingInfo(nearFilter.Filter, resultSink);
				return;
			}
			TextFilter textFilter = filter as TextFilter;
			if (textFilter != null)
			{
				InstantSearchRequest.GetHitHighlightingInfo(textFilter, resultSink);
				return;
			}
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter != null)
			{
				InstantSearchRequest.GetHitHighlightingInfo(comparisonFilter, resultSink);
			}
		}

		private static void GetHitHighlightingInfo(TextFilter filter, HashSet<string> resultSink)
		{
			if (!string.IsNullOrEmpty(filter.Text) && InstantSearchRequest.HitHighlightingProperties.Contains(filter.Property))
			{
				resultSink.Add(filter.Text);
			}
		}

		private static void GetHitHighlightingInfo(ComparisonFilter filter, HashSet<string> resultSink)
		{
			if (filter.ComparisonOperator == ComparisonOperator.Equal)
			{
				string text = filter.PropertyValue as string;
				if (!string.IsNullOrEmpty(text) && InstantSearchRequest.HitHighlightingProperties.Contains(filter.Property))
				{
					resultSink.Add(text);
				}
			}
		}

		private void AnalyzeQuery()
		{
			ExTraceGlobals.InstantSearchTracer.TraceDebug((long)this.traceContext, "Analyzing query");
			QueryExecutionStep step = this.queryStatistics.StartNewStep(QueryExecutionStepType.AnalyzeQuery);
			bool flag = false;
			this.realQueryFilter = this.query.AdditionalFilter;
			this.warmingQueryFilter = this.realQueryFilter;
			if (this.query.RequestType != InstantSearchQueryParameters.QueryType.PureQueryFilter)
			{
				QueryFilter queryFilter;
				if (this.query.EmptyPrewarmingQuery)
				{
					flag = true;
					queryFilter = InstantSearchRequest.PrewarmQueryFilter;
				}
				else
				{
					queryFilter = AqsParser.ParseAndBuildQuery(this.query.KqlQuery, AqsParser.ParseOption.SuppressError | AqsParser.ParseOption.UseCiKeywordOnly, this.session.PreferredCulture, RescopedAll.Default, this.session.RecipientResolver, this.session.PolicyTagProvider);
				}
				if (this.query.HasOption(QueryOptions.AllowFuzzing))
				{
					queryFilter = this.FuzzFilter(queryFilter);
				}
				if (this.query.RequestType == InstantSearchQueryParameters.QueryType.KqlWithQueryFilter)
				{
					if (!flag)
					{
						this.realQueryFilter = new AndFilter(new QueryFilter[]
						{
							queryFilter,
							this.realQueryFilter
						});
					}
					this.warmingQueryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						this.warmingQueryFilter
					});
				}
				else
				{
					this.realQueryFilter = queryFilter;
					this.warmingQueryFilter = queryFilter;
				}
			}
			QueryFilter queryFilter2 = flag ? this.warmingQueryFilter : this.realQueryFilter;
			QueryFilter queryFilter3 = this.BuildFolderQueryFilter();
			if (queryFilter3 != null)
			{
				queryFilter2 = new AndFilter(new QueryFilter[]
				{
					queryFilter2,
					queryFilter3
				});
			}
			this.fqlQuery = FqlQueryBuilder.ToFqlString(queryFilter2, this.session.PreferredCulture);
			ExTraceGlobals.InstantSearchTracer.TraceDebug<string>((long)this.traceContext, "FQL query: {0}", this.fqlQuery);
			if (this.query.RefinementFilter != null && !this.useFastQueryPath)
			{
				QueryFilter queryFilter4;
				lock (this.session.Session)
				{
					queryFilter4 = this.query.RefinementFilter.GetQueryFilter(this.session.PreferredCulture, this.session.Session, this.session.RecipientResolver, this.session.PolicyTagProvider);
				}
				if (!flag)
				{
					this.realQueryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter4,
						this.realQueryFilter
					});
				}
				this.warmingQueryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter4,
					this.warmingQueryFilter
				});
			}
			KeyValuePair<string, object>[] additionalStatistics = new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("RefinementFilter", this.query.RefinementFilter),
				new KeyValuePair<string, object>("Refiners", this.query.Refiners)
			};
			this.queryStatistics.CompleteStep(step, additionalStatistics);
		}

		private QueryFilter BuildFolderQueryFilter()
		{
			IReadOnlyList<StoreId> readOnlyList = this.query.FolderScope ?? this.session.FolderScope;
			if (readOnlyList == null || readOnlyList.Count == 0)
			{
				return null;
			}
			if (readOnlyList.Count != 1)
			{
				QueryFilter[] array = new QueryFilter[readOnlyList.Count];
				for (int i = 0; i < readOnlyList.Count; i++)
				{
					array[i] = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ParentEntryId, readOnlyList[i]);
				}
				return new OrFilter(array);
			}
			if (readOnlyList[0] == null || readOnlyList[0].Equals(this.session.RootFolderId))
			{
				return null;
			}
			return new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ParentEntryId, readOnlyList[0]);
		}

		private void StartExecution()
		{
			ExTraceGlobals.InstantSearchTracer.TraceDebug<InstantSearchQueryParameters>((long)this.traceContext, "Starting query execution, query: {0}.", this.query);
			if (this.query.HasOption(QueryOptions.Results | QueryOptions.Refiners))
			{
				this.AnalyzeQuery();
			}
			if (this.query.HasOption(QueryOptions.SuggestionsPrimer))
			{
				this.InitiateSuggestionsPrimer();
			}
			if (this.query.HasOption(QueryOptions.Suggestions))
			{
				this.InitiateGetSuggestions();
			}
			if (this.query.HasOption(QueryOptions.SearchTerms))
			{
				this.InitiateGetHitHighlighting();
			}
			if (this.query.HasOption(QueryOptions.Results | QueryOptions.Refiners) || (this.query.RequestType != InstantSearchQueryParameters.QueryType.PureQueryFilter && this.query.EmptyPrewarmingQuery))
			{
				if (InstantSearchRequest.TestQueryHandler != null)
				{
					InstantSearchRequest.TestQueryHandler(this.realQueryFilter);
				}
				else if (this.useFastQueryPath)
				{
					this.InitiateGetFastResults();
				}
				else
				{
					this.StartStoreQueryExecution();
				}
			}
			Interlocked.Decrement(ref this.outstandingOperations);
		}

		private QueryFilter FuzzFilter(QueryFilter filter)
		{
			if (!this.IsFuzzable(filter))
			{
				return filter;
			}
			AndFilter andFilter = filter as AndFilter;
			if (andFilter != null)
			{
				return this.FuzzFilter(andFilter);
			}
			return this.FuzzFilter((TextFilter)filter);
		}

		private QueryFilter FuzzFilter(AndFilter filter)
		{
			int num = 0;
			int num2 = 0;
			QueryFilter[] array = new QueryFilter[filter.Filters.Count];
			foreach (QueryFilter queryFilter in filter.Filters)
			{
				TextFilter textFilter = (TextFilter)queryFilter;
				QueryFilter queryFilter2 = textFilter;
				if (num < 2)
				{
					queryFilter2 = this.FuzzFilter(textFilter);
					if (queryFilter2 != textFilter)
					{
						num++;
					}
				}
				array[num2++] = queryFilter2;
			}
			return new AndFilter(array);
		}

		private QueryFilter FuzzFilter(TextFilter filter)
		{
			if (filter.MatchOptions == MatchOptions.ExactPhrase)
			{
				return filter;
			}
			IDictionary<string, QuerySuggestion> suggestions = this.session.Completions.GetSuggestions(filter.Text, QuerySuggestionSources.Fuzzy, this.session.Session.PreferedCulture.TwoLetterISOLanguageName);
			if (suggestions.Count > 0)
			{
				List<TextFilter> list = new List<TextFilter>();
				foreach (string text in suggestions.Keys)
				{
					if (!text.StartsWith(filter.Text))
					{
						list.Add(new TextFilter(filter.Property, text, filter.MatchOptions, filter.MatchFlags));
					}
				}
				if (list.Count > 0)
				{
					list.Add(filter);
					return new OrFilter(list.ToArray());
				}
			}
			return filter;
		}

		private bool IsFuzzable(QueryFilter filter)
		{
			AndFilter andFilter = filter as AndFilter;
			if (andFilter == null)
			{
				return filter is TextFilter;
			}
			foreach (QueryFilter queryFilter in andFilter.Filters)
			{
				if (!(queryFilter is TextFilter))
				{
					return false;
				}
			}
			return true;
		}

		private void InitiateGetHitHighlighting()
		{
			Interlocked.Increment(ref this.outstandingOperations);
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				this.NotifyLoop(InstantSearchRequest.LoopNotifications.HitHighlightingDataAvailable | InstantSearchRequest.LoopNotifications.OperationCompleted);
			});
		}

		private void InitiateGetSuggestions()
		{
			Interlocked.Increment(ref this.outstandingOperations);
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				this.NotifyLoop(InstantSearchRequest.LoopNotifications.SuggestionsAvailable | InstantSearchRequest.LoopNotifications.OperationCompleted);
			});
		}

		private void InitiateSuggestionsPrimer()
		{
			Interlocked.Increment(ref this.outstandingOperations);
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				this.NotifyLoop(InstantSearchRequest.LoopNotifications.SuggestionsPrimerRequest | InstantSearchRequest.LoopNotifications.OperationCompleted);
			});
		}

		private void InitiateGetFastResults()
		{
			Interlocked.Increment(ref this.outstandingOperations);
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				this.NotifyLoop(InstantSearchRequest.LoopNotifications.FastResultsAvailable | InstantSearchRequest.LoopNotifications.OperationCompleted);
			});
		}

		private void InitiateGetRefiners()
		{
			Interlocked.Increment(ref this.outstandingOperations);
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				this.NotifyLoop(InstantSearchRequest.LoopNotifications.RefinersAvailable | InstantSearchRequest.LoopNotifications.OperationCompleted);
			});
		}

		private void StartStoreQueryExecution()
		{
			QueryExecutionStep step = this.queryStatistics.StartNewStep(QueryExecutionStepType.CreateSearchFolder);
			IReadOnlyList<StoreId> source = this.query.FolderScope ?? this.session.FolderScope;
			SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(this.warmingQueryFilter, source.ToArray<StoreId>())
			{
				DeepTraversal = this.query.DeepTraversal,
				MaximumResultsCount = this.maxResultsCount
			};
			lock (this.session.Session)
			{
				StoreObjectId defaultFolderId = this.session.Session.GetDefaultFolderId(DefaultFolderType.SearchFolders);
				this.searchFolder = SearchFolder.Create(this.session.Session, defaultFolderId, "XSO Instant Search " + Guid.NewGuid(), CreateMode.InstantSearch | (this.session.SearchForConversations ? CreateMode.OptimizedConversationSearch : CreateMode.CreateNew));
				this.searchFolder[FolderSchema.SearchFolderAllowAgeout] = true;
				this.searchFolder[FolderSchema.SearchFolderAgeOutTimeout] = 3600;
				this.searchFolder[FolderSchema.CorrelationId] = this.session.CorrelationId;
				this.searchFolder.Save();
				this.searchFolder.Load();
				this.queryStatistics.CompleteStep(step);
				QueryExecutionStep step2 = this.queryStatistics.StartNewStep(QueryExecutionStepType.GetSearchFolderView);
				if (this.query.HasOption(QueryOptions.Results | QueryOptions.Refiners))
				{
					Interlocked.Increment(ref this.outstandingOperations);
					this.searchCompleteSubscription = Subscription.Create(this.session.Session, delegate(Notification notification)
					{
						if ((notification.Type & NotificationType.SearchComplete) == NotificationType.SearchComplete)
						{
							this.NotifyLoop(InstantSearchRequest.LoopNotifications.StoreResultsAvailable | InstantSearchRequest.LoopNotifications.FinalResultsAvailable | InstantSearchRequest.LoopNotifications.OperationCompleted);
						}
					}, NotificationType.SearchComplete, this.searchFolder.StoreObjectId);
				}
				this.searchFolder.ApplyOneTimeSearch(searchFolderCriteria);
				this.queryStatistics.CompleteStep(step2);
			}
		}

		private void FetchHitHighlighting()
		{
			QueryExecutionStep step = this.queryStatistics.StartNewStep(QueryExecutionStepType.GetHitHighlighiting);
			HashSet<string> hashSet = new HashSet<string>();
			InstantSearchRequest.GetHitHighlightingInfo(this.realQueryFilter, hashSet);
			this.queryStatistics.CompleteStep(step);
			QueryExecutionStep step2 = this.queryStatistics.StartNewStep(QueryExecutionStepType.HitHighlightingCallback);
			this.session.HitHighlightingDataAvailableCallback(hashSet.ToArray<string>(), this);
			this.queryStatistics.CompleteStep(step2);
		}

		private void FetchSuggestionsPrimer()
		{
			QueryExecutionStep step = this.queryStatistics.StartNewStep(QueryExecutionStepType.GetSuggestionsPrimer);
			List<QuerySuggestion> queryHistorySortedByRank = this.GetQueryHistorySortedByRank();
			if (queryHistorySortedByRank.Count > 0)
			{
				this.session.SuggestionsAvailableCallback(queryHistorySortedByRank, this);
			}
			this.queryStatistics.CompleteStep(step);
		}

		private void FetchMoreSuggestions()
		{
			Dictionary<string, QuerySuggestion> dictionary = new Dictionary<string, QuerySuggestion>();
			if ((this.query.QuerySuggestionSources & QuerySuggestionSources.RecentSearches) != QuerySuggestionSources.None)
			{
				this.PopulateQueryHistorySuggestions(this.query.KqlQuery, dictionary, 0.8999);
			}
			if (this.session.Config.TopNEnabled && (this.query.QuerySuggestionSources & QuerySuggestionSources.TopN) != QuerySuggestionSources.None)
			{
				if (this.session.Completions.ReloadTopN)
				{
					lock (this.session.Session)
					{
						QueryExecutionStep step = this.queryStatistics.StartNewStep(QueryExecutionStepType.TopNInitialization);
						this.session.Completions.InitializeTopN(this.session.Session);
						this.queryStatistics.CompleteStep(step, this.session.Completions.ReadAndResetTopNInitializationStatistics());
						goto IL_104;
					}
				}
				List<KeyValuePair<string, object>> list = this.session.Completions.ReadAndResetTopNInitializationStatistics();
				if (list != null)
				{
					QueryExecutionStep step2 = this.queryStatistics.StartNewStep(QueryExecutionStepType.TopNInitialization);
					this.queryStatistics.CompleteStep(step2, list);
				}
			}
			IL_104:
			QueryExecutionStep step3 = this.queryStatistics.StartNewStep(QueryExecutionStepType.GetSuggestions);
			List<KeyValuePair<string, object>> additionalStatistics = this.session.Completions.AddSuggestions(dictionary, this.query.KqlQuery, this.query.QuerySuggestionSources, this.session.PreferredCulture.TwoLetterISOLanguageName);
			this.queryStatistics.CompleteStep(step3, additionalStatistics);
			if (dictionary.Values.Count > 0)
			{
				QueryExecutionStep step4 = this.queryStatistics.StartNewStep(QueryExecutionStepType.SuggestionsCallback);
				this.session.SuggestionsAvailableCallback(dictionary.Values.ToArray<QuerySuggestion>(), this);
				this.queryStatistics.CompleteStep(step4);
			}
		}

		private void PopulateQueryHistorySuggestions(string queryString, Dictionary<string, QuerySuggestion> suggestions, double currentSuggestionRank)
		{
			bool flag = false;
			lock (this.session.QueryHistory)
			{
				foreach (QueryHistoryInputDictionaryEntry queryHistoryInputDictionaryEntry in this.session.QueryHistory)
				{
					if (queryHistoryInputDictionaryEntry.Query.StartsWith(queryString, StringComparison.InvariantCultureIgnoreCase))
					{
						flag = true;
						if (!suggestions.Keys.Contains(queryHistoryInputDictionaryEntry.Query))
						{
							QuerySuggestion value = new QuerySuggestion(queryHistoryInputDictionaryEntry.Query, currentSuggestionRank + queryHistoryInputDictionaryEntry.Rank, QuerySuggestionSources.RecentSearches);
							suggestions.Add(queryHistoryInputDictionaryEntry.Query, value);
						}
					}
					else if (flag)
					{
						break;
					}
				}
			}
		}

		private List<QuerySuggestion> GetQueryHistorySortedByRank()
		{
			SortedDictionary<double, string> sortedDictionary = new SortedDictionary<double, string>(InstantSearchRequest.DescendingSort.Instance);
			lock (this.session.QueryHistory)
			{
				foreach (QueryHistoryInputDictionaryEntry queryHistoryInputDictionaryEntry in this.session.QueryHistory)
				{
					try
					{
						sortedDictionary.Add(queryHistoryInputDictionaryEntry.Rank + (double)queryHistoryInputDictionaryEntry.LastUsed / 9.2233720368547758E+18, queryHistoryInputDictionaryEntry.Query);
					}
					catch (ArgumentException)
					{
					}
				}
			}
			List<QuerySuggestion> list = new List<QuerySuggestion>(sortedDictionary.Count);
			foreach (KeyValuePair<double, string> keyValuePair in sortedDictionary)
			{
				QuerySuggestion item = new QuerySuggestion(keyValuePair.Value, keyValuePair.Key, QuerySuggestionSources.RecentSearches);
				list.Add(item);
			}
			return list;
		}

		private void FetchUpdatedResults(bool finalResultsAvailable)
		{
			QueryExecutionStep step = this.queryStatistics.StartNewStep(QueryExecutionStepType.QuerySearchFolder);
			IStorePropertyBag[] propertyBags;
			lock (this.session.Session)
			{
				using (QueryResult queryResult = this.session.SearchForConversations ? this.searchFolder.ConversationItemQuery(null, this.sortSpec, this.session.Schema.XsoProperties) : this.searchFolder.ItemQuery(ItemQueryType.None, null, this.sortSpec, this.session.Schema.XsoProperties))
				{
					propertyBags = queryResult.GetPropertyBags(int.MaxValue);
				}
			}
			this.queryStatistics.CompleteStep(step);
			if (this.query.HasOption(QueryOptions.Results) && (propertyBags.Length > 0 || finalResultsAvailable))
			{
				QueryExecutionStep step2 = this.queryStatistics.StartNewStep(QueryExecutionStepType.QueryResultsCallback);
				this.session.ResultsUpdatedCallback(propertyBags, this);
				this.queryStatistics.CompleteStep(step2);
			}
			if (finalResultsAvailable && this.query.HasOption(QueryOptions.Refiners))
			{
				this.InitiateGetRefiners();
			}
		}

		private void FetchFastResults()
		{
			if (this.query.HasOption(QueryOptions.Results))
			{
				QueryExecutionStep step = this.queryStatistics.StartNewStep(QueryExecutionStepType.GetFastResults);
				Guid guid = Guid.NewGuid();
				AdditionalParameters additionalParameters = new AdditionalParameters();
				if (this.query.RefinementFilter != null)
				{
					additionalParameters.RefinementFilters = this.query.RefinementFilter.Filters;
				}
				QueryParameters queryParameters = new QueryParameters(this.session.Config.QueryOperationTimeout, FlowDescriptor.GetImsFlowDescriptor(this.session.Config, FastIndexVersion.GetIndexSystemName(this.session.MdbGuid)).DisplayName, this.fqlQuery, this.session.MailboxGuid, guid, additionalParameters);
				queryParameters.ExtraFields = this.session.Schema.FastProperties;
				queryParameters.Sort = this.session.Schema.GetFastSortOrder(this.sortSpec);
				queryParameters.Config = this.session.Config;
				if (this.session.SearchForConversations && !string.IsNullOrEmpty(this.session.Config.InstantSearchConversationCollapseSpec))
				{
					queryParameters.Collapse = this.session.Config.InstantSearchConversationCollapseSpec;
				}
				if (this.query.EmptyPrewarmingQuery)
				{
					queryParameters.PageSize = 1;
					if (this.session.Config.CachePreWarmingEnabled)
					{
						queryParameters.CachePreWarmingMode = this.session.Config.CachePreWarmingMode;
						queryParameters.ClientFunction = "InstantItemWarmup";
					}
				}
				else if (this.maxResultsCount != null)
				{
					queryParameters.PageSize = this.maxResultsCount.Value;
				}
				else
				{
					queryParameters.PageSize = 250;
				}
				SearchResultItem[] fastResults = null;
				Exception ex = null;
				try
				{
					fastResults = this.session.FlowExecutor.ReadPage(queryParameters);
				}
				catch (CommunicationException ex2)
				{
					ex = ex2;
				}
				catch (TimeoutException ex3)
				{
					ex = ex3;
				}
				KeyValuePair<string, object>[] additionalStatistics = new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("CorrelationId", guid)
				};
				this.queryStatistics.CompleteStep(step, additionalStatistics);
				if (ex != null)
				{
					this.TerminateRequest(ex);
					return;
				}
				if (this.query.EmptyPrewarmingQuery)
				{
					return;
				}
				QueryExecutionStep step2 = this.queryStatistics.StartNewStep(QueryExecutionStepType.ConvertFastResultsToPropertyBags);
				IReadOnlyCollection<IReadOnlyPropertyBag> arg;
				lock (this.session.Session)
				{
					arg = this.session.Schema.ConvertSearchResultItemsToPropertyBags(this.session.Session, this.session.SearchForConversations, fastResults, this.session.Config);
				}
				this.queryStatistics.CompleteStep(step2);
				QueryExecutionStep step3 = this.queryStatistics.StartNewStep(QueryExecutionStepType.QueryResultsCallback);
				this.session.ResultsUpdatedCallback(arg, this);
				this.queryStatistics.CompleteStep(step3);
			}
			if (this.query.HasOption(QueryOptions.Refiners))
			{
				this.InitiateGetRefiners();
			}
		}

		private void FetchRefiners(RefinerDataProvider provider)
		{
			QueryExecutionStep step = this.queryStatistics.StartNewStep(QueryExecutionStepType.GetRefiners);
			IReadOnlyCollection<RefinerData> refiners = provider.GetRefiners(this.requestedRefiners, this.query.MaximumRefinersCount);
			KeyValuePair<string, object>[] additionalStatistics = new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("RefinerDataProvider", provider.Name),
				new KeyValuePair<string, object>("CorrelationId", provider.CorrelationId),
				new KeyValuePair<string, object>("RefinersData", refiners)
			};
			this.queryStatistics.CompleteStep(step, additionalStatistics);
			QueryExecutionStep step2 = this.queryStatistics.StartNewStep(QueryExecutionStepType.RefinersCallback);
			this.session.RefinerDataAvailableCallback(refiners, this);
			this.queryStatistics.CompleteStep(step2);
		}

		private void NotifyLoop(InstantSearchRequest.LoopNotifications notifications)
		{
			bool flag = false;
			lock (this.loopLock)
			{
				if (this.IsCanceled)
				{
					return;
				}
				if ((notifications & InstantSearchRequest.LoopNotifications.OperationCompleted) == InstantSearchRequest.LoopNotifications.OperationCompleted)
				{
					Interlocked.Decrement(ref this.outstandingOperations);
				}
				this.loopNotifications |= notifications;
				if (!this.loopRunning)
				{
					this.loopRunning = true;
					flag = true;
				}
			}
			if (flag)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.MainLoop));
			}
		}

		private void MainLoop(object state)
		{
			using (new ActivityScopeThreadGuard(this.activityScope))
			{
				for (;;)
				{
					InstantSearchRequest.LoopNotifications loopNotifications;
					lock (this.loopLock)
					{
						if (this.IsCanceled)
						{
							ExTraceGlobals.InstantSearchTracer.TraceDebug((long)this.traceContext, "Query execution cancelled.");
							this.queryStatistics.Complete();
							base.InvokeCallback(this.queryStatistics);
							break;
						}
						if (this.loopNotifications == InstantSearchRequest.LoopNotifications.None)
						{
							this.loopRunning = false;
							break;
						}
						loopNotifications = this.loopNotifications;
						this.loopNotifications = InstantSearchRequest.LoopNotifications.None;
					}
					try
					{
						if ((loopNotifications & InstantSearchRequest.LoopNotifications.Startup) == InstantSearchRequest.LoopNotifications.Startup)
						{
							this.StartExecution();
						}
						if ((loopNotifications & InstantSearchRequest.LoopNotifications.SuggestionsPrimerRequest) == InstantSearchRequest.LoopNotifications.SuggestionsPrimerRequest)
						{
							this.FetchSuggestionsPrimer();
						}
						if ((loopNotifications & InstantSearchRequest.LoopNotifications.SuggestionsAvailable) == InstantSearchRequest.LoopNotifications.SuggestionsAvailable)
						{
							this.FetchMoreSuggestions();
						}
						if ((loopNotifications & InstantSearchRequest.LoopNotifications.HitHighlightingDataAvailable) == InstantSearchRequest.LoopNotifications.HitHighlightingDataAvailable)
						{
							this.FetchHitHighlighting();
						}
						if ((loopNotifications & InstantSearchRequest.LoopNotifications.StoreResultsAvailable) == InstantSearchRequest.LoopNotifications.StoreResultsAvailable)
						{
							this.FetchUpdatedResults((loopNotifications & InstantSearchRequest.LoopNotifications.FinalResultsAvailable) == InstantSearchRequest.LoopNotifications.FinalResultsAvailable);
						}
						if ((loopNotifications & InstantSearchRequest.LoopNotifications.FastResultsAvailable) != InstantSearchRequest.LoopNotifications.None)
						{
							this.FetchFastResults();
						}
						if ((loopNotifications & InstantSearchRequest.LoopNotifications.RefinersAvailable) == InstantSearchRequest.LoopNotifications.RefinersAvailable)
						{
							this.FetchRefiners(RefinerDataProvider.Create(this.session.Config, this.session.FlowExecutor, this.session.MdbGuid, this.session.MailboxGuid, this.fqlQuery, this.query.RefinementFilter));
						}
						if ((loopNotifications & InstantSearchRequest.LoopNotifications.OperationCompleted) != InstantSearchRequest.LoopNotifications.OperationCompleted || Interlocked.CompareExchange(ref this.outstandingOperations, 0, 0) != 0 || this.loopNotifications != InstantSearchRequest.LoopNotifications.None)
						{
							continue;
						}
						ExTraceGlobals.InstantSearchTracer.TraceDebug((long)this.traceContext, "Query execution complete.");
						this.queryStatistics.Complete();
						base.InvokeCallback(this.queryStatistics);
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
					catch (StoragePermanentException ex)
					{
						this.TerminateRequest(ex);
					}
					catch (StorageTransientException ex2)
					{
						this.TerminateRequest(ex2);
					}
					catch (Exception ex3)
					{
						this.TerminateRequest(ex3);
					}
					break;
				}
			}
		}

		private void TerminateRequest(Exception ex)
		{
			ExTraceGlobals.InstantSearchTracer.TraceError<Exception>((long)this.traceContext, "Query execution failed, exception: {0}.", ex);
			this.queryStatistics.Complete();
			Exception value;
			if (ex is StoragePermanentException)
			{
				value = new InstantSearchPermanentException(ServerStrings.SearchOperationFailed, ex, this.queryStatistics);
			}
			else
			{
				value = new InstantSearchTransientException(ServerStrings.SearchOperationFailed, ex, this.queryStatistics);
			}
			base.InvokeCallback(value);
		}

		private void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				lock (this.session.Session)
				{
					if (this.searchCompleteSubscription != null)
					{
						this.searchCompleteSubscription.Dispose();
						this.searchCompleteSubscription = null;
					}
					if (this.searchFolder != null)
					{
						this.searchFolder.Dispose();
						this.searchFolder = null;
					}
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
			}
		}

		private const string DefaultSearchFolderNamePrefix = "XSO Instant Search ";

		private const int SearchFolderAgeoutTimeout = 3600;

		private const string DefaultPrewarmingQuery = "InstantItemWarmUpSearch31febf7b418e44878df6e5623e37c828";

		private const int DefaultMaxResultsCount = 250;

		private static readonly PropertyDefinition[] InitialRequestedProperties = new StorePropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static readonly HashSet<PropertyDefinition> HitHighlightingProperties = new HashSet<PropertyDefinition>
		{
			ItemSchema.Categories,
			ItemSchema.SearchSender,
			ItemSchema.SearchRecipientsTo,
			ItemSchema.SearchRecipientsBcc,
			ItemSchema.SearchRecipientsCc,
			ItemSchema.SearchRecipients,
			ItemSchema.Subject,
			ItemSchema.TextBody,
			ItemSchema.AttachmentContent,
			ItemSchema.SearchAllIndexedProps
		};

		private static readonly List<QuerySuggestion> EmptySuggestionList = new List<QuerySuggestion>();

		private static readonly QueryFilter PrewarmQueryFilter = new TextFilter(ItemSchema.SearchAllIndexedProps, "InstantItemWarmUpSearch31febf7b418e44878df6e5623e37c828", MatchOptions.ExactPhrase, MatchFlags.Loose);

		private readonly int traceContext;

		private readonly InstantSearch session;

		private readonly InstantSearchQueryParameters query;

		private readonly IReadOnlyCollection<PropertyDefinition> requestedRefiners;

		private readonly SortBy[] sortSpec;

		private readonly object loopLock = new object();

		private readonly QueryStatistics queryStatistics;

		private readonly bool useFastQueryPath;

		private readonly int? maxResultsCount;

		private readonly IActivityScope activityScope;

		private QueryFilter realQueryFilter;

		private string fqlQuery;

		private QueryFilter warmingQueryFilter;

		private SearchFolder searchFolder;

		private Subscription searchCompleteSubscription;

		private volatile InstantSearchRequest.LoopNotifications loopNotifications;

		private bool loopRunning;

		private int outstandingOperations;

		private DisposeTracker disposeTracker;

		[Flags]
		private enum LoopNotifications
		{
			None = 0,
			Startup = 1,
			StoreResultsAvailable = 2,
			SuggestionsAvailable = 4,
			SuggestionsPrimerRequest = 8,
			HitHighlightingDataAvailable = 16,
			FinalResultsAvailable = 32,
			RefinersAvailable = 64,
			FastResultsAvailable = 128,
			OperationCompleted = 256
		}

		private class DescendingSort : IComparer<double>
		{
			public int Compare(double x, double y)
			{
				return y.CompareTo(x);
			}

			public static readonly InstantSearchRequest.DescendingSort Instance = new InstantSearchRequest.DescendingSort();
		}
	}
}
