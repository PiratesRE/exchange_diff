using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AllItemsFolderHelper
	{
		static AllItemsFolderHelper()
		{
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.ConversationTopicHash, new SortBy[]
			{
				new SortBy(ItemSchema.ConversationTopicHash, SortOrder.Ascending),
				new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
			});
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.CleanGlobalObjectId, new SortBy[]
			{
				new SortBy(CalendarItemBaseSchema.CleanGlobalObjectId, SortOrder.Ascending),
				new SortBy(InternalSchema.AppointmentSequenceNumber, SortOrder.Descending)
			});
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.InternetMessageIdHash, new SortBy[]
			{
				new SortBy(ItemSchema.InternetMessageIdHash, SortOrder.Ascending)
			});
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.RetentionDate, new SortBy[]
			{
				new SortBy(ItemSchema.RetentionDate, SortOrder.Descending)
			});
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.PolicyTag, new SortBy[]
			{
				new SortBy(StoreObjectSchema.PolicyTag, SortOrder.Descending),
				new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
			});
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.ConversationIdHash, new SortBy[]
			{
				new SortBy(ItemSchema.ConversationIdHash, SortOrder.Ascending),
				new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
			});
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.ReceivedTime, new SortBy[]
			{
				new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
			});
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.ArchiveDate, new SortBy[]
			{
				new SortBy(ItemSchema.ArchiveDate, SortOrder.Descending)
			});
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.SharePointId, new SortBy[]
			{
				new SortBy(CoreItemSchema.LinkedId, SortOrder.Ascending)
			});
			AllItemsFolderHelper.sortOrderMap.Add(AllItemsFolderHelper.SupportedSortBy.EhaMigrationExpiryDate, new SortBy[]
			{
				new SortBy(ItemSchema.EHAMigrationExpiryDate, SortOrder.Descending)
			});
		}

		public static object[] FindItemBySharePointId(MailboxSession session, Guid sharepointId)
		{
			Util.ThrowOnNullArgument(session, "session");
			if (sharepointId == Guid.Empty)
			{
				throw new ArgumentNullException("sharepointId");
			}
			object[] array = AllItemsFolderHelper.RunQueryOnAllItemsFolder<object[]>(session, AllItemsFolderHelper.SupportedSortBy.SharePointId, sharepointId, null, delegate(QueryResult queryResult)
			{
				object[][] rows = queryResult.GetRows(1);
				if (rows == null || rows.Length <= 0)
				{
					return null;
				}
				object[] array2 = new object[AllItemsFolderHelper.findItemBySharePointIdProps.Count];
				array2[0] = ((VersionedId)rows[0][0]).ObjectId;
				array2[1] = (StoreObjectId)rows[0][1];
				string text = (string)rows[0][2];
				if (!string.IsNullOrEmpty(text))
				{
					array2[2] = new Uri(text);
				}
				else
				{
					array2[2] = null;
				}
				return array2;
			}, AllItemsFolderHelper.findItemBySharePointIdProps);
			return array ?? null;
		}

		public static IStorePropertyBag[] FindItemsFromInternetId(MailboxSession session, string internetMessageId, ItemQueryType itemQueryType, params PropertyDefinition[] propertyDefinitions)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullOrEmptyArgument(internetMessageId, "internetMessageId");
			Util.ThrowOnNullOrEmptyArgument(propertyDefinitions, "propertyDefinitions");
			ICollection<PropertyDefinition> properties = InternalSchema.Combine<PropertyDefinition>(propertyDefinitions, new PropertyDefinition[]
			{
				ItemSchema.InternetMessageId,
				ItemSchema.InternetMessageIdHash
			});
			int internetMessageIdHash = (int)AllItemsFolderHelper.GetHashValue(internetMessageId);
			IStorePropertyBag[] array = AllItemsFolderHelper.RunQueryOnAllItemsFolder<IStorePropertyBag[]>(session, AllItemsFolderHelper.SupportedSortBy.InternetMessageIdHash, internetMessageIdHash, null, (QueryResult queryResult) => AllItemsFolderHelper.ProcessQueryResult(queryResult, internetMessageId, internetMessageIdHash), properties, itemQueryType);
			return array ?? Array<IStorePropertyBag>.Empty;
		}

		public static IStorePropertyBag[] FindItemsFromInternetId(MailboxSession session, string internetMessageId, params PropertyDefinition[] propertyDefinitions)
		{
			return AllItemsFolderHelper.FindItemsFromInternetId(session, internetMessageId, ItemQueryType.None, propertyDefinitions);
		}

		internal static IStorePropertyBag[] ProcessQueryResult(QueryResult queryResult, string internetMessageId, int internetMessageIdHash)
		{
			List<IStorePropertyBag> list = new List<IStorePropertyBag>(3);
			for (;;)
			{
				IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(3);
				if (propertyBags == null || propertyBags.Length == 0)
				{
					goto IL_9F;
				}
				for (int i = 0; i < propertyBags.Length; i++)
				{
					int? num = propertyBags[i].TryGetProperty(ItemSchema.InternetMessageIdHash) as int?;
					string strA = propertyBags[i].TryGetProperty(ItemSchema.InternetMessageId) as string;
					if (num == null || !(internetMessageIdHash == num))
					{
						goto IL_89;
					}
					if (string.Compare(strA, internetMessageId, StringComparison.OrdinalIgnoreCase) == 0)
					{
						list.Add(propertyBags[i]);
					}
				}
			}
			IL_89:
			return list.ToArray();
			IL_9F:
			return list.ToArray();
		}

		internal static void InitializeTransportIndexes(Folder folder)
		{
			foreach (SortBy[] sortColumns in AllItemsFolderHelper.sortOrderMap.Values)
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, sortColumns, new PropertyDefinition[]
				{
					ItemSchema.Id
				}))
				{
					queryResult.GetRows(1);
				}
			}
		}

		public static G RunQueryOnAllItemsFolder<G>(MailboxSession session, AllItemsFolderHelper.SupportedSortBy supportedSortBy, AllItemsFolderHelper.DoQueryProcessing<G> queryProcessor, ICollection<PropertyDefinition> properties, ItemQueryType itemQueryType)
		{
			AllItemsFolderHelper.CheckAndCreateDefaultFolders(session);
			G result;
			using (SearchFolder searchFolder = SearchFolder.Bind(session, DefaultFolderType.AllItems))
			{
				using (QueryResult queryResult = searchFolder.ItemQuery(itemQueryType, null, AllItemsFolderHelper.sortOrderMap[supportedSortBy], properties))
				{
					result = queryProcessor(queryResult);
				}
			}
			return result;
		}

		public static G RunQueryOnAllItemsFolder<G>(MailboxSession session, AllItemsFolderHelper.SupportedSortBy supportedSortBy, AllItemsFolderHelper.DoQueryProcessing<G> queryProcessor, ICollection<PropertyDefinition> properties)
		{
			return AllItemsFolderHelper.RunQueryOnAllItemsFolder<G>(session, supportedSortBy, queryProcessor, properties, ItemQueryType.None);
		}

		public static G RunQueryOnAllItemsFolder<G>(MailboxSession session, AllItemsFolderHelper.SupportedSortBy supportedSortBy, object seekToValue, G defaultValue, AllItemsFolderHelper.DoQueryProcessing<G> queryProcessor, ICollection<PropertyDefinition> properties, ItemQueryType itemQueryType)
		{
			return AllItemsFolderHelper.RunQueryOnAllItemsFolder<G>(session, supportedSortBy, delegate(QueryResult queryResult)
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, AllItemsFolderHelper.sortOrderMap[supportedSortBy][0].ColumnDefinition, seekToValue)))
				{
					return queryProcessor(queryResult);
				}
				return defaultValue;
			}, properties, itemQueryType);
		}

		public static G RunQueryOnAllItemsFolder<G>(MailboxSession session, AllItemsFolderHelper.SupportedSortBy supportedSortBy, object seekToValue, G defaultValue, AllItemsFolderHelper.DoQueryProcessing<G> queryProcessor, ICollection<PropertyDefinition> properties)
		{
			return AllItemsFolderHelper.RunQueryOnAllItemsFolder<G>(session, supportedSortBy, seekToValue, defaultValue, queryProcessor, properties, ItemQueryType.None);
		}

		public static void CheckAndCreateDefaultFolders(MailboxSession session)
		{
			if (session.GetDefaultFolderId(DefaultFolderType.AllItems) == null)
			{
				session.CreateDefaultFolder(DefaultFolderType.AllItems);
			}
		}

		public static uint GetHashValue(string input)
		{
			uint num = 0U;
			if (string.IsNullOrEmpty(input))
			{
				return num;
			}
			foreach (uint num2 in input.ToUpperInvariant())
			{
				num ^= num2;
				for (int j = 0; j < 16; j++)
				{
					uint num3 = num & 1U;
					num >>= 1;
					if (num3 != 0U)
					{
						num ^= 3988292384U;
					}
				}
			}
			return num;
		}

		public static uint GetHashValue(byte[] inputBytes)
		{
			uint num = 0U;
			if (inputBytes == null || inputBytes.Length == 0)
			{
				return num;
			}
			foreach (uint num2 in inputBytes)
			{
				num ^= num2;
				for (int j = 0; j < 8; j++)
				{
					uint num3 = num & 1U;
					num >>= 1;
					if (num3 != 0U)
					{
						num ^= 3988292384U;
					}
				}
			}
			return num;
		}

		private static readonly ICollection<PropertyDefinition> findItemBySharePointIdProps = new PropertyDefinition[]
		{
			CoreItemSchema.Id,
			InternalSchema.ParentItemId,
			InternalSchema.LinkedUrl
		};

		private static Dictionary<AllItemsFolderHelper.SupportedSortBy, SortBy[]> sortOrderMap = new Dictionary<AllItemsFolderHelper.SupportedSortBy, SortBy[]>();

		internal enum SupportedSortBy
		{
			ConversationTopicHash,
			InternetMessageIdHash,
			CleanGlobalObjectId,
			RetentionDate,
			PolicyTag,
			ConversationIdHash,
			ReceivedTime,
			ArchiveDate,
			SharePointId,
			EhaMigrationExpiryDate
		}

		public delegate G DoQueryProcessing<G>(QueryResult queryResult);
	}
}
