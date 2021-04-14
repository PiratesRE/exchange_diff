using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal static class ModernCalendarItemFilteringHelper
	{
		internal static QueryFilter GetDefaultContentsViewFilter(CoreFolder coreFolder, Logon logon)
		{
			if (logon.Session.MailboxOwner.GetConfiguration().RpcClientAccess.FilterModernCalendarItemsMomtView.Enabled && ModernCalendarItemFilteringHelper.FolderNeedsFilter(coreFolder.Id.ObjectId, logon))
			{
				return ModernCalendarItemFilteringHelper.IgnoreModernCalendarItemsFilter;
			}
			return null;
		}

		internal static QueryFilter AddFolderFilterForIcsIfRequired(QueryFilter queryFilter, CoreFolder coreFolder, Logon logon)
		{
			if (logon.Session.MailboxOwner.GetConfiguration().RpcClientAccess.FilterModernCalendarItemsMomtIcs.Enabled && ModernCalendarItemFilteringHelper.FolderNeedsFilter(coreFolder.Id.ObjectId, logon))
			{
				queryFilter = ModernCalendarItemFilteringHelper.AddModernCalendarItemFilter(queryFilter);
			}
			return queryFilter;
		}

		internal static QueryFilter AddModernCalendarItemsFilterForSearch(QueryFilter queryFilter, Logon logon)
		{
			if (logon.Session.MailboxOwner.GetConfiguration().RpcClientAccess.FilterModernCalendarItemsMomtSearch.Enabled)
			{
				return ModernCalendarItemFilteringHelper.AddModernCalendarItemFilter(queryFilter);
			}
			return queryFilter;
		}

		internal static void RemoveModernCalendarItemRestriction(SearchFolderCriteria searchFolderCriteria, Logon logon)
		{
			if (logon.Session.MailboxOwner.GetConfiguration().RpcClientAccess.FilterModernCalendarItemsMomtSearch.Enabled)
			{
				QueryFilter searchQuery;
				if (ModernCalendarItemFilteringHelper.TryRemovingModernCalendarItemAndClause(searchFolderCriteria.SearchQuery, out searchQuery))
				{
					searchFolderCriteria.SearchQuery = searchQuery;
					return;
				}
				if (ModernCalendarItemFilteringHelper.IsModernCalendarItemFilter(searchFolderCriteria.SearchQuery))
				{
					searchFolderCriteria.SearchQuery = null;
				}
			}
		}

		private static QueryFilter AddModernCalendarItemFilter(QueryFilter queryFilter)
		{
			if (queryFilter == null)
			{
				queryFilter = ModernCalendarItemFilteringHelper.IgnoreModernCalendarItemsFilter;
			}
			else
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					ModernCalendarItemFilteringHelper.IgnoreModernCalendarItemsFilter,
					queryFilter
				});
			}
			return queryFilter;
		}

		private static bool FolderNeedsFilter(StoreObjectId folderObjectId, Logon logon)
		{
			return folderObjectId.ObjectType == StoreObjectType.CalendarFolder && !ModernCalendarItemFilteringHelper.IsConnectionWithSupportTool(logon);
		}

		private static bool IsConnectionWithSupportTool(Logon logon)
		{
			string processName = logon.Connection.ClientInformation.ProcessName;
			return processName != null && processName.Contains("mfcmapi");
		}

		private static bool TryRemovingModernCalendarItemAndClause(QueryFilter queryFilter, out QueryFilter newFilter)
		{
			newFilter = null;
			AndFilter andFilter = queryFilter as AndFilter;
			if (andFilter != null && andFilter.FilterCount > 1 && ModernCalendarItemFilteringHelper.IsModernCalendarItemFilter(andFilter.Filters[0]))
			{
				if (andFilter.FilterCount == 2)
				{
					newFilter = andFilter.Filters[1];
				}
				else
				{
					newFilter = new AndFilter(andFilter.IgnoreWhenVerifyingMaxDepth, andFilter.Filters.Skip(1).ToArray<QueryFilter>());
				}
				return true;
			}
			return false;
		}

		private static bool IsModernCalendarItemFilter(QueryFilter queryFilter)
		{
			return ModernCalendarItemFilteringHelper.IgnoreModernCalendarItemsFilter.Equals(queryFilter);
		}

		private static readonly ComparisonFilter IgnoreModernCalendarItemsFilter = new ComparisonFilter(ComparisonOperator.NotEqual, CalendarItemBaseSchema.IsHiddenFromLegacyClients, true);
	}
}
