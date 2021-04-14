using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SearchUtil
	{
		internal static QueryFilter BuildQueryFilter(QueryFilter queryFilter, StoreObjectId junkEmailFolderId, StoreObjectId deletedItemsFolderId, bool includeJunkItems, bool includeDeletedItems, ViewFilter viewFilter, ClutterFilter clutterFilter, RestrictionType refinerRestriction, string fromFilter)
		{
			if (!includeJunkItems)
			{
				QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.ParentItemId, junkEmailFolderId);
				queryFilter = SearchUtil.BuildAndFilter(queryFilter, queryFilter2);
			}
			if (!includeDeletedItems)
			{
				QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.ParentItemId, deletedItemsFolderId);
				queryFilter = SearchUtil.BuildAndFilter(queryFilter, queryFilter3);
			}
			return SearchUtil.BuildQueryFilter(queryFilter, viewFilter, clutterFilter, refinerRestriction, fromFilter);
		}

		internal static QueryFilter BuildQueryFilter(QueryFilter queryFilter, ViewFilter viewFilter, ClutterFilter clutterFilter, RestrictionType refinerRestriction, string fromFilter)
		{
			if (refinerRestriction != null)
			{
				ServiceObjectToFilterConverter serviceObjectToFilterConverter = new ServiceObjectToFilterConverter();
				queryFilter = SearchUtil.BuildAndFilter(queryFilter, serviceObjectToFilterConverter.Convert(refinerRestriction.Item));
			}
			if (viewFilter != ViewFilter.All || clutterFilter != ClutterFilter.All)
			{
				ViewFilter viewFilterForSearchFolder = SearchUtil.GetViewFilterForSearchFolder(viewFilter, clutterFilter);
				QueryFilter viewQueryFilter = SearchUtil.GetViewQueryFilter(viewFilterForSearchFolder);
				queryFilter = SearchUtil.BuildAndFilter(queryFilter, viewQueryFilter);
			}
			if (!string.IsNullOrEmpty(fromFilter))
			{
				QueryFilter itemQueryFilter = PeopleIKnowQuery.GetItemQueryFilter(fromFilter);
				queryFilter = SearchUtil.BuildAndFilter(queryFilter, itemQueryFilter);
			}
			return queryFilter;
		}

		internal static SearchFolderCriteria BuildSearchCriteria(List<StoreId> folderScope, QueryFilter queryFilter, SearchScope searchScope, QueryStringType queryStringType)
		{
			SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(queryFilter, folderScope.ToArray());
			searchFolderCriteria.DeepTraversal = (searchScope != SearchScope.SelectedFolder);
			if (queryStringType != null && queryStringType.MaxResultsCount > 0)
			{
				searchFolderCriteria.MaximumResultsCount = new int?(queryStringType.MaxResultsCount);
			}
			return searchFolderCriteria;
		}

		private static QueryFilter BuildQueryFilter(MailboxSession mailboxSession, StoreObjectId sourceFolderId, ViewFilter viewFilter, ClutterFilter clutterFilter, string fromFilter)
		{
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.JunkEmail);
			StoreObjectId defaultFolderId2 = mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
			bool includeJunkItems = sourceFolderId.Equals(defaultFolderId);
			bool includeDeletedItems = sourceFolderId.Equals(defaultFolderId2);
			return SearchUtil.BuildQueryFilter(null, defaultFolderId, defaultFolderId2, includeJunkItems, includeDeletedItems, viewFilter, clutterFilter, null, fromFilter);
		}

		internal static SearchFolder GetOwaViewFilterSearchFolder(OwaSearchContext searchContext, MailboxSession mailboxSession, StoreObjectId searchFoldersRootId, SortBy[] sortByArray, CallContext callContext)
		{
			ExTraceGlobals.SearchTracer.TraceDebug<string>(0L, "[SearchUtil::GetOwaViewFilterSearchFolder] Attempting to get filtered view search folder: view filter {0}", searchContext.ViewFilter.ToString());
			if (callContext != null && callContext.ProtocolLog != null)
			{
				callContext.ProtocolLog.Set(FindConversationAndItemMetadata.ViewFilter, searchContext.ViewFilter.ToString());
			}
			QueryFilter queryFilter = SearchUtil.BuildQueryFilter(mailboxSession, (StoreObjectId)searchContext.FolderIdToSearch, (ViewFilter)searchContext.ViewFilter, ClutterFilter.All, searchContext.FromFilter);
			List<StoreId> list = new List<StoreId>();
			list.Add(searchContext.FolderIdToSearch);
			SearchScope searchScope = SearchScope.SelectedFolder;
			SearchFolderCriteria searchFolderCriteria = SearchUtil.BuildSearchCriteria(list, queryFilter, searchScope, null);
			return OwaFilterState.CreateOrOpenOwaFilteredViewSearchFolder(mailboxSession, searchContext, searchFoldersRootId, searchFolderCriteria, true);
		}

		internal static bool IsViewFilterNonQuery(ViewFilter viewFilter)
		{
			return viewFilter == ViewFilter.All || viewFilter == ViewFilter.TaskOverdue;
		}

		internal static bool IsViewFilterOnClutter(ViewFilter viewFilter)
		{
			return viewFilter == ViewFilter.Clutter || viewFilter == ViewFilter.NoClutter;
		}

		internal static bool IsSearch(QueryStringType queryStringType)
		{
			return queryStringType != null && !string.IsNullOrEmpty(queryStringType.Value);
		}

		internal static bool IsFilteredView(QueryStringType queryStringType, ViewFilter viewFilter, ClutterFilter clutterFilter, string fromFilter)
		{
			return !SearchUtil.IsSearch(queryStringType) && (!SearchUtil.IsViewFilterNonQuery(viewFilter) || !string.IsNullOrEmpty(fromFilter) || clutterFilter != ClutterFilter.All);
		}

		internal static bool IsComplexClutterFilteredView(ViewFilter viewFilter, ClutterFilter clutterFilter)
		{
			return clutterFilter != ClutterFilter.All && viewFilter != ViewFilter.All && viewFilter != ViewFilter.NoClutter && viewFilter != ViewFilter.Clutter;
		}

		internal static bool IsNormalView(QueryStringType queryStringType, ViewFilter viewFilter, ClutterFilter clutterFilter)
		{
			return !SearchUtil.IsSearch(queryStringType) && SearchUtil.IsViewFilterNonQuery(viewFilter) && clutterFilter == ClutterFilter.All;
		}

		internal static bool ShouldReturnDeletedItems(QueryStringType queryStringType)
		{
			return queryStringType != null && queryStringType.ReturnDeletedItems;
		}

		internal static void BuildHighlightTerms(QueryFilter filter, List<HighlightTermType> terms)
		{
			if (filter is CompositeFilter)
			{
				CompositeFilter compositeFilter = (CompositeFilter)filter;
				for (int i = 0; i < compositeFilter.FilterCount; i++)
				{
					SearchUtil.BuildHighlightTerms(compositeFilter.Filters[i], terms);
				}
				return;
			}
			if (filter is TextFilter)
			{
				TextFilter textFilter = filter as TextFilter;
				if (textFilter.Property != ItemSchema.Categories && textFilter.Property != StoreObjectSchema.ItemClass)
				{
					terms.Add(new HighlightTermType
					{
						Scope = textFilter.Property.Name,
						Value = textFilter.Text
					});
				}
			}
		}

		internal static SearchFolder CreateStaticSearchFolder(MailboxSession mailboxSession, StoreId searchFoldersRootId, SearchFolderCriteria searchFolderCriteria)
		{
			string text = "EWS Search " + Guid.NewGuid().ToString();
			SearchFolder searchFolder = SearchFolder.Create(mailboxSession, searchFoldersRootId, text, CreateMode.CreateNew);
			ExTraceGlobals.SearchTracer.TraceDebug(0L, "[SearchUtil::CreateStaticSearchFolder] Creating new static search view search folder: " + text);
			searchFolder[FolderSchema.SearchFolderAllowAgeout] = true;
			searchFolder.Save();
			searchFolder.Load();
			return SearchUtil.ApplyOneTimeSearch(searchFolder, searchFolderCriteria);
		}

		internal static SearchFolder CreateOrOpenStaticOwaSearchFolder(MailboxSession mailboxSession, OwaSearchContext searchContext, StoreId searchFoldersRootId, SearchFolderCriteria searchFolderCriteria)
		{
			StorePerformanceCountersCapture storePerformanceCountersCapture = StorePerformanceCountersCapture.Start(mailboxSession);
			ExDateTime utcNow = ExDateTime.UtcNow;
			Stopwatch stopwatch = Stopwatch.StartNew();
			string text = "EWS Search " + searchContext.ClientSearchFolderIdentity;
			SearchFolder searchFolder = null;
			bool flag = true;
			try
			{
				if (!searchContext.IsResetCache)
				{
					searchFolder = SearchFolder.Create(mailboxSession, searchFoldersRootId, text, CreateMode.OpenIfExists);
					searchFolder[FolderSchema.SearchFolderAllowAgeout] = true;
					searchFolder[FolderSchema.SearchFolderAgeOutTimeout] = 3600;
					searchFolder.Save();
					searchFolder.Load();
					ExTraceGlobals.SearchTracer.TraceDebug(0L, "[SearchUtil::CreateOrOpenStaticOwaSearchFolder] Opening existing static search folder: " + text);
				}
				else
				{
					searchFolder = SearchFolder.Create(mailboxSession, searchFoldersRootId, text, CreateMode.InstantSearch | (searchContext.OptimizedSearch ? CreateMode.OptimizedConversationSearch : CreateMode.CreateNew));
					ExTraceGlobals.SearchTracer.TraceDebug(0L, "[SearchUtil::CreateOrOpenStaticOwaSearchFolder] Creating new static search folder: " + text);
					searchFolder[FolderSchema.SearchFolderAllowAgeout] = true;
					searchFolder[FolderSchema.SearchFolderAgeOutTimeout] = 3600;
					searchFolder.Save();
					searchFolder.Load();
					searchContext.SearchFolderId = searchFolder.StoreObjectId;
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.CorrelationId, searchFolder.PropertyBag.GetValueOrDefault<Guid>(FolderSchema.CorrelationId, Guid.Empty));
					ExDateTime dateTime = utcNow.AddTicks(stopwatch.Elapsed.Ticks);
					StorePerformanceCounters storePerformanceCounters = storePerformanceCountersCapture.Stop();
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.CreateSearchFolderTime, storePerformanceCounters.ElapsedMilliseconds);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.CreateSearchFolderCPUTime, storePerformanceCounters.Cpu);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.CreateSearchFolderRpcCount, storePerformanceCounters.RpcCount);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.CreateSearchFolderRpcLatency, storePerformanceCounters.RpcLatency);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.CreateSearchFolderRpcLatencyOnStore, storePerformanceCounters.RpcLatencyOnStore);
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.CreateSearchFolderStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
					CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.CreateSearchFolderEndTimestamp, SearchUtil.FormatIso8601String(dateTime));
					searchFolder = SearchUtil.ApplyOneTimeOwaSearch(searchFolder, searchContext, searchFolderCriteria, mailboxSession);
				}
				flag = false;
			}
			finally
			{
				if (flag)
				{
					ExTraceGlobals.SearchTracer.TraceDebug(0L, "[SearchUtil::CreateOrOpenStaticOwaSearchFolder] Creating/Opening of static search folder failed. Setting search folder to null and disposing it");
					if (searchFolder != null)
					{
						searchFolder.Dispose();
						searchFolder = null;
					}
				}
			}
			return searchFolder;
		}

		internal static T GetFolderProperty<T>(Folder folder, PropertyDefinition propertyDefinition, T defaultValue)
		{
			object obj = folder.TryGetProperty(propertyDefinition);
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		internal static ViewFilter GetViewFilterForSearchFolder(ViewFilter viewFilter, ClutterFilter clutterFilter)
		{
			if (clutterFilter != ClutterFilter.All && viewFilter == ViewFilter.All)
			{
				switch (clutterFilter)
				{
				case ClutterFilter.NoClutter:
					return ViewFilter.NoClutter;
				case ClutterFilter.Clutter:
					return ViewFilter.Clutter;
				}
			}
			return viewFilter;
		}

		internal static QueryFilter GetViewQueryForComplexClutterFilteredView(ClutterFilter clutterFilter, bool isConversation)
		{
			QueryFilter result = new TrueFilter();
			switch (clutterFilter)
			{
			case ClutterFilter.All:
				return result;
			case ClutterFilter.NoClutter:
				if (!isConversation)
				{
					return new ComparisonFilter(ComparisonOperator.NotEqual, ItemSchema.IsClutter, true);
				}
				return new ComparisonFilter(ComparisonOperator.Equal, ConversationItemSchema.ConversationHasClutter, false);
			case ClutterFilter.Clutter:
				if (!isConversation)
				{
					return new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.IsClutter, true);
				}
				return new ComparisonFilter(ComparisonOperator.Equal, ConversationItemSchema.ConversationHasClutter, true);
			default:
				return result;
			}
		}

		internal static QueryFilter GetViewQueryFilter(ViewFilter viewFilter)
		{
			QueryFilter result = new TrueFilter();
			switch (viewFilter)
			{
			case ViewFilter.All:
			case ViewFilter.DeprecatedSuggestions:
			case ViewFilter.DeprecatedSuggestionsRespond:
			case ViewFilter.DeprecatedSuggestionsDelete:
				return result;
			case ViewFilter.Flagged:
				return new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.FlagStatus, FlagStatus.Flagged);
			case ViewFilter.HasAttachment:
				return new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.HasAttachment, true);
			case ViewFilter.ToOrCcMe:
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, MessageItemSchema.MessageToMe, true),
					new ComparisonFilter(ComparisonOperator.Equal, MessageItemSchema.MessageCcMe, true)
				});
			case ViewFilter.Unread:
				return new BitMaskFilter(MessageItemSchema.Flags, 1UL, false);
			case ViewFilter.TaskActive:
				return SearchUtil.activeTaskFilter;
			case ViewFilter.TaskOverdue:
				return new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.LessThan, TaskSchema.DueDate, Util.GetLocalTime().Date),
					new ExistsFilter(TaskSchema.DueDate),
					SearchUtil.activeTaskFilter
				});
			case ViewFilter.TaskCompleted:
				return SearchUtil.completeTaskFilter;
			case ViewFilter.NoClutter:
				return SearchUtil.NoClutterQueryFilter;
			case ViewFilter.Clutter:
				return SearchUtil.ClutterQueryFilter;
			default:
				return result;
			}
		}

		internal static QueryFilter BuildAndFilter(QueryFilter queryFilter1, QueryFilter queryFilter2)
		{
			if (queryFilter1 == null || queryFilter1 is TrueFilter)
			{
				return queryFilter2;
			}
			if (queryFilter2 is TrueFilter)
			{
				return queryFilter1;
			}
			return new AndFilter(new QueryFilter[]
			{
				queryFilter1,
				queryFilter2
			});
		}

		private static SearchFolder ApplyOneTimeSearch(SearchFolder searchFolder, SearchFolderCriteria searchCriteria)
		{
			IAsyncResult asyncResult = searchFolder.BeginApplyOneTimeSearch(searchCriteria, null, null);
			int searchTimeoutInMilliseconds = Global.SearchTimeoutInMilliseconds;
			bool flag = asyncResult.AsyncWaitHandle.WaitOne(searchTimeoutInMilliseconds);
			if (flag)
			{
				searchFolder.EndApplyOneTimeSearch(asyncResult);
			}
			else
			{
				ExTraceGlobals.SearchTracer.TraceDebug(0L, "[SearchUtil::ApplyOneTimeSearch] Creation of static search folder failed. Setting search folder to null and disposing it");
				if (searchFolder != null)
				{
					searchFolder.Dispose();
					searchFolder = null;
				}
			}
			return searchFolder;
		}

		private static SearchFolder ApplyOneTimeOwaSearch(SearchFolder searchFolder, OwaSearchContext searchContext, SearchFolderCriteria searchCriteria, MailboxSession session)
		{
			bool flag = Util.IsArchiveMailbox(session);
			long notificationCreateTime = 0L;
			long completedEventTimestamp = 0L;
			string notificationEventType = string.Empty;
			QueryResult queryResult = null;
			Subscription subscription = null;
			Subscription subscription2 = null;
			ManualResetEvent completedEvent = new ManualResetEvent(false);
			StorePerformanceCountersCapture storePerformanceCountersCapture = StorePerformanceCountersCapture.Start(session);
			ExDateTime utcNow = ExDateTime.UtcNow;
			Stopwatch stopwatch = Stopwatch.StartNew();
			bool flag2;
			try
			{
				subscription2 = Subscription.Create(session, delegate(Notification notification)
				{
					if ((notification.Type & NotificationType.SearchComplete) == NotificationType.SearchComplete)
					{
						if (string.IsNullOrEmpty(notificationEventType))
						{
							notificationEventType = notification.Type.ToString();
							completedEventTimestamp = Stopwatch.GetTimestamp();
							notificationCreateTime = notification.CreateTime;
						}
						searchContext.IsSearchInProgress = false;
						completedEvent.Set();
					}
				}, NotificationType.SearchComplete, searchFolder.StoreObjectId);
				if (!flag && !searchContext.WaitForSearchComplete)
				{
					queryResult = searchFolder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
					{
						ItemSchema.Id
					});
					subscription = Subscription.Create(queryResult, delegate(Notification notification)
					{
						QueryNotification queryNotification = notification as QueryNotification;
						if (queryNotification != null && (queryNotification.EventType & QueryNotificationType.QueryResultChanged) == QueryNotificationType.QueryResultChanged)
						{
							if (string.IsNullOrEmpty(notificationEventType))
							{
								notificationEventType = queryNotification.EventType.ToString();
								completedEventTimestamp = Stopwatch.GetTimestamp();
								notificationCreateTime = notification.CreateTime;
							}
							completedEvent.Set();
						}
					});
				}
				if (CallContext.Current.OwaCallback != null && !flag && !searchContext.WaitForSearchComplete && !searchContext.IsWarmUpSearch)
				{
					CallContext.Current.OwaCallback.ProcessCallback(searchContext);
				}
				searchContext.IsSearchInProgress = true;
				searchFolder.ApplyOneTimeSearch(searchCriteria);
				flag2 = !completedEvent.WaitOne(Global.SearchTimeoutInMilliseconds);
				StorePerformanceCounters storePerformanceCounters = storePerformanceCountersCapture.Stop();
				double num = (double)(completedEventTimestamp - notificationCreateTime) * SearchUtil.millisecondToTickRatio;
				double num2 = (double)(Stopwatch.GetTimestamp() - completedEventTimestamp) * SearchUtil.millisecondToTickRatio;
				ExDateTime dateTime = utcNow.AddTicks(stopwatch.Elapsed.Ticks);
				searchCriteria = searchFolder.GetSearchCriteria();
				searchContext.IsSearchFailed = (searchCriteria != null && (searchCriteria.SearchState & SearchState.Error) == SearchState.Error);
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderTime, storePerformanceCounters.ElapsedMilliseconds);
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderNotificationQueueTime, string.Format("{0:F4}", num));
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderCPUTime, storePerformanceCounters.Cpu);
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderRpcCount, storePerformanceCounters.RpcCount);
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderRpcLatency, storePerformanceCounters.RpcLatency);
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderRpcLatencyOnStore, storePerformanceCounters.RpcLatencyOnStore);
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderEventType, notificationEventType);
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderEventDelay, string.Format("{0:F4}", num2));
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderStartTimestamp, SearchUtil.FormatIso8601String(utcNow));
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderEndTimestamp, SearchUtil.FormatIso8601String(dateTime));
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderFailed, searchContext.IsSearchFailed);
				CallContext.Current.ProtocolLog.Set(FindConversationAndItemMetadata.PopulateSearchFolderMaxResultsCount, searchCriteria.MaximumResultsCount);
			}
			finally
			{
				if (queryResult != null)
				{
					queryResult.Dispose();
				}
				if (subscription2 != null)
				{
					subscription2.Dispose();
				}
				if (subscription != null)
				{
					subscription.Dispose();
				}
			}
			if (flag2)
			{
				searchContext.IsSearchInProgress = false;
				ExTraceGlobals.SearchTracer.TraceDebug(0L, "[SearchUtil::ApplyOneTimeOwaSearch] Creation of static search folder failed. Setting search folder to null and disposing it");
				if (searchFolder != null)
				{
					searchFolder.Dispose();
					searchFolder = null;
				}
			}
			return searchFolder;
		}

		internal static string FormatIso8601String(ExDateTime dateTime)
		{
			return dateTime.UniversalTime.ToString("yyyy-MM-ddTHH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
		}

		internal static List<object[]> FetchRowsFromQueryResult(QueryResult queryResult, int rowsToGet)
		{
			int estimatedRowCount = queryResult.EstimatedRowCount;
			int num = queryResult.EstimatedRowCount - queryResult.CurrentRow;
			if (rowsToGet > num)
			{
				rowsToGet = num;
			}
			List<object[]> list = new List<object[]>();
			bool flag = rowsToGet > 0;
			while (flag)
			{
				SearchUtil.CheckClientConnection();
				bool flag2;
				object[][] rows = queryResult.GetRows(rowsToGet, out flag2);
				int length = rows.GetLength(0);
				rowsToGet -= length;
				for (int i = 0; i < length; i++)
				{
					list.Add(rows[i]);
				}
				flag = (flag2 && rowsToGet > 0);
			}
			return list;
		}

		internal static SortBy[] AddPropertyToSortBy(PropertyDefinition newProp, SortBy[] originalSortBy)
		{
			bool flag = false;
			int num = 0;
			SortBy[] array;
			if (originalSortBy != null)
			{
				array = new SortBy[originalSortBy.Length + 1];
				foreach (SortBy sortBy in originalSortBy)
				{
					if (sortBy.ColumnDefinition == newProp)
					{
						flag = true;
						break;
					}
					array[num] = sortBy;
					num++;
				}
			}
			else
			{
				array = new SortBy[1];
			}
			if (!flag)
			{
				array[num] = new SortBy(newProp, SortOrder.Ascending);
				return array;
			}
			return originalSortBy;
		}

		internal static void SetHighlightTerms(OwaSearchContext searchContext, HighlightTermType[] highlightTermTypes)
		{
			if (highlightTermTypes == null || highlightTermTypes.Length <= 0)
			{
				return;
			}
			KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[highlightTermTypes.Length];
			for (int i = 0; i < highlightTermTypes.Length; i++)
			{
				HighlightTermType highlightTermType = highlightTermTypes[i];
				array[i] = new KeyValuePair<string, string>(highlightTermType.Scope, highlightTermType.Value);
			}
			searchContext.HighlightTerms = array;
		}

		private static void CheckClientConnection()
		{
			if (CallContext.Current != null && !CallContext.Current.HttpContext.Response.IsClientConnected)
			{
				BailOut.SetHTTPStatusAndClose(HttpStatusCode.NoContent);
			}
		}

		internal const string EwsCISearchFolderPrefix = "EWS Search ";

		internal const string DateTimeFormatPreciseZulu = "yyyy-MM-ddTHH:mm:ss.fff\\Z";

		private const int SearchFolderAgeOutTimeout = 3600;

		private static readonly double millisecondToTickRatio = 1000.0 / (double)Stopwatch.Frequency;

		private static readonly QueryFilter activeTaskFilter = new AndFilter(new QueryFilter[]
		{
			new NotFilter(new ExistsFilter(ItemSchema.CompleteDate)),
			new NotFilter(new ExistsFilter(ItemSchema.FlagCompleteTime))
		});

		private static readonly QueryFilter completeTaskFilter = new OrFilter(new QueryFilter[]
		{
			new ExistsFilter(ItemSchema.CompleteDate),
			new ExistsFilter(ItemSchema.FlagCompleteTime),
			new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.FlagStatus, FlagStatus.Complete)
		});

		private static readonly QueryFilter NoClutterQueryFilter = Folder.GetClutterBasedViewFilter(false);

		private static readonly QueryFilter ClutterQueryFilter = Folder.GetClutterBasedViewFilter(true);
	}
}
