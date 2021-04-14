using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DumpsterFolderHelper
	{
		public static QueryFilter ExcludeAuditFoldersFilter
		{
			get
			{
				return DumpsterFolderHelper.excludeAuditFoldersFilter;
			}
		}

		public static bool RunQueryOnDiscoveryHoldsFolder(MailboxSession session, SortBy sortBy, Func<QueryResult, bool> queryProcessor, ICollection<PropertyDefinition> properties)
		{
			StoreId folderId = session.CowSession.CheckAndCreateDiscoveryHoldsFolder(session);
			bool result;
			using (Folder folder = Folder.Bind(session, folderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, new SortBy[]
				{
					sortBy
				}, properties))
				{
					result = queryProcessor(queryResult);
				}
			}
			return result;
		}

		public static bool RunQueryOnMigratedMessagesFolder(MailboxSession session, SortBy sortBy, Func<QueryResult, bool> queryProcessor, ICollection<PropertyDefinition> properties)
		{
			StoreId folderId = session.CowSession.CheckAndCreateMigratedMessagesFolder();
			bool result;
			using (Folder folder = Folder.Bind(session, folderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, new SortBy[]
				{
					sortBy
				}, properties))
				{
					result = queryProcessor(queryResult);
				}
			}
			return result;
		}

		public static IStorePropertyBag[] FindItemsFromInternetId(MailboxSession session, string internetMessageId, params PropertyDefinition[] propertyDefinitions)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullOrEmptyArgument(internetMessageId, "internetMessageId");
			Util.ThrowOnNullOrEmptyArgument(propertyDefinitions, "propertyDefinitions");
			DumpsterFolderHelper.CheckAndCreateFolder(session);
			ICollection<PropertyDefinition> dataColumns = InternalSchema.Combine<PropertyDefinition>(propertyDefinitions, new PropertyDefinition[]
			{
				ItemSchema.InternetMessageId
			});
			IStorePropertyBag[] result;
			using (Folder folder = Folder.Bind(session, DefaultFolderType.RecoverableItemsDeletions))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, DumpsterFolderHelper.searchSortBy, dataColumns))
				{
					if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, DumpsterFolderHelper.searchSortBy[0].ColumnDefinition, internetMessageId)))
					{
						result = DumpsterFolderHelper.ProcessQueryResult(queryResult, internetMessageId);
					}
					else
					{
						result = Array<IStorePropertyBag>.Empty;
					}
				}
			}
			return result;
		}

		internal static IStorePropertyBag[] ProcessQueryResult(QueryResult queryResult, string internetMessageId)
		{
			List<IStorePropertyBag> list = new List<IStorePropertyBag>(3);
			for (;;)
			{
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(3);
				for (int i = 0; i < propertyBags.Length; i++)
				{
					string b = propertyBags[i].TryGetProperty(ItemSchema.InternetMessageId) as string;
					if (!string.Equals(internetMessageId, b, StringComparison.OrdinalIgnoreCase))
					{
						goto IL_3B;
					}
					list.Add(propertyBags[i]);
				}
				if (propertyBags.Length == 0)
				{
					goto Block_3;
				}
			}
			IL_3B:
			return list.ToArray();
			Block_3:
			return list.ToArray();
		}

		public static void CheckAndCreateFolder(MailboxSession session)
		{
			Util.ThrowOnNullArgument(session, "session");
			if (session.LogonType == LogonType.Delegated)
			{
				return;
			}
			foreach (DefaultFolderType defaultFolderType in DumpsterFolderHelper.dumpsterFoldersTypes)
			{
				if (session.GetDefaultFolderId(defaultFolderType) == null)
				{
					StoreObjectId storeObjectId = session.CreateDefaultFolder(defaultFolderType);
					session.GetDefaultFolderId(defaultFolderType);
				}
			}
		}

		public static bool IsDumpsterFolder(MailboxSession session, StoreObjectId folderId)
		{
			Util.ThrowOnNullArgument(session, "session");
			if (folderId == null)
			{
				return false;
			}
			foreach (DefaultFolderType defaultFolderType in DumpsterFolderHelper.dumpsterFoldersTypes)
			{
				if (folderId.Equals(session.GetDefaultFolderId(defaultFolderType)))
				{
					return true;
				}
			}
			return folderId.Equals(session.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDiscoveryHolds)) || folderId.Equals(session.GetDefaultFolderId(DefaultFolderType.RecoverableItemsMigratedMessages)) || DumpsterFolderHelper.IsAuditFolder(session, folderId);
		}

		public static bool IsAuditFolder(MailboxSession session, StoreObjectId folderId)
		{
			Util.ThrowOnNullArgument(session, "session");
			if (folderId == null)
			{
				return false;
			}
			StoreObjectId auditsFolder = null;
			StoreObjectId adminAuditFolder = null;
			session.BypassAuditsFolderAccessChecking(delegate
			{
				auditsFolder = session.GetAuditsFolderId();
				adminAuditFolder = session.GetAdminAuditLogsFolderId();
			});
			return folderId.Equals(auditsFolder) || folderId.Equals(adminAuditFolder);
		}

		public static MiddleTierStoragePerformanceCountersInstance GetPerfCounters()
		{
			return NamedPropMap.GetPerfCounters();
		}

		private static readonly QueryFilter excludeAuditFoldersFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.DisplayName, "Audits"),
			new ComparisonFilter(ComparisonOperator.NotEqual, StoreObjectSchema.DisplayName, "AdminAuditLogs")
		});

		private static SortBy[] searchSortBy = new SortBy[]
		{
			new SortBy(ItemSchema.InternetMessageId, SortOrder.Ascending)
		};

		private static DefaultFolderType[] dumpsterFoldersTypes = new DefaultFolderType[]
		{
			DefaultFolderType.RecoverableItemsRoot,
			DefaultFolderType.RecoverableItemsDeletions,
			DefaultFolderType.RecoverableItemsVersions,
			DefaultFolderType.RecoverableItemsPurges,
			DefaultFolderType.CalendarLogging
		};
	}
}
