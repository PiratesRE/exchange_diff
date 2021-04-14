using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Search
{
	public class NoGrouping : BaseGroupByType
	{
		internal override BasePageResult IssueQuery(QueryFilter query, Folder folder, SortBy[] sortBy, BasePagingType paging, ItemQueryTraversal traversal, PropertyDefinition[] propsToFetch, RequestDetailsLogger logger)
		{
			CalendarFolder calendarFolder = folder as CalendarFolder;
			CalendarPageView calendarPageView = paging as CalendarPageView;
			if (calendarPageView != null)
			{
				calendarPageView.Validate(calendarFolder);
				CalendarViewLatencyInformation calendarViewLatencyInformation = new CalendarViewLatencyInformation();
				object[][] calendarView = calendarFolder.GetCalendarView(calendarPageView.StartDateEx, calendarPageView.EndDateEx, calendarViewLatencyInformation, propsToFetch);
				if (logger != null)
				{
					logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsViewUsed, calendarViewLatencyInformation.IsNewView);
					logger.Set(FindConversationAndItemMetadata.CalendarViewTime, calendarViewLatencyInformation.ViewTime);
					logger.Set(FindConversationAndItemMetadata.CalendarSingleItemsTotalTime, calendarViewLatencyInformation.SingleItemTotalTime);
					logger.Set(FindConversationAndItemMetadata.CalendarSingleItemsCount, calendarViewLatencyInformation.SingleItemQueryCount);
					logger.Set(FindConversationAndItemMetadata.CalendarSingleItemsGetRowsTime, calendarViewLatencyInformation.SingleItemGetRowsTime);
					logger.Set(FindConversationAndItemMetadata.CalendarSingleItemsQueryRowsTime, calendarViewLatencyInformation.SingleItemQueryTime);
					logger.Set(FindConversationAndItemMetadata.CalendarSingleItemsSeekToConditionTime, calendarViewLatencyInformation.SingleQuerySeekToTime);
					logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsCount, calendarViewLatencyInformation.RecurringItemQueryCount);
					logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsTotalTime, calendarViewLatencyInformation.RecurringItemTotalTime);
					logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsExpansionTime, calendarViewLatencyInformation.RecurringExpansionTime);
					logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsGetRowsTime, calendarViewLatencyInformation.RecurringItemGetRowsTime);
					logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsQueryTime, calendarViewLatencyInformation.RecurringItemQueryTime);
					logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsNoInstancesInWindow, calendarViewLatencyInformation.RecurringItemsNoInstancesInWindow);
					if (calendarViewLatencyInformation.MaxRecurringItemLatencyInformation != null)
					{
						logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsMaxSubject, calendarViewLatencyInformation.MaxRecurringItemLatencyInformation.Subject);
						logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsMaxParseTime, calendarViewLatencyInformation.MaxRecurringItemLatencyInformation.BlobParseTime);
						logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsMaxBlobStreamTime, calendarViewLatencyInformation.MaxRecurringItemLatencyInformation.BlobStreamTime);
						logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsMaxExpansionTime, calendarViewLatencyInformation.MaxRecurringItemLatencyInformation.BlobExpansionTime);
						logger.Set(FindConversationAndItemMetadata.CalendarRecurringItemsMaxAddRowsTime, calendarViewLatencyInformation.MaxRecurringItemLatencyInformation.AddRowsForInstancesTime);
					}
				}
				return calendarPageView.ApplyPostQueryPaging(calendarView);
			}
			BasePageResult result;
			using (QueryResult queryResult = folder.ItemQuery((ItemQueryType)traversal, query, sortBy, propsToFetch))
			{
				result = BasePagingType.ApplyPostQueryPaging(queryResult, paging);
			}
			return result;
		}

		internal override PropertyDefinition[] GetAdditionalFetchProperties()
		{
			return null;
		}

		internal override QueryType QueryType
		{
			get
			{
				return QueryType.Items;
			}
		}
	}
}
