using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SearchFolderConversationRetriever : SearchFolderDataRetrieverBase
	{
		static SearchFolderConversationRetriever()
		{
			for (int i = 0; i < SearchFolderConversationRetriever.itemSearchPropertyDefinitions.Length; i++)
			{
				SearchFolderConversationRetriever.itemSearchPropertyDefinitionsOrderDictionary[SearchFolderConversationRetriever.itemSearchPropertyDefinitions[i]] = i;
			}
		}

		public static ConversationType[] GetConversationDataFromSearchFolder(OwaSearchContext searchContext, MailboxSession mailboxSession, out int totalItemCount)
		{
			totalItemCount = 0;
			List<ConversationType> list = new List<ConversationType>(50);
			StoreId searchFolderId = searchContext.SearchFolderId;
			SortBy[] searchSortBy = searchContext.SearchSortBy;
			using (SearchFolder searchFolder = SearchFolder.Bind(mailboxSession, searchFolderId))
			{
				int rowCount = 25;
				using (QueryResult queryResult = searchFolder.ConversationItemQuery(null, searchSortBy, SearchFolderConversationRetriever.itemSearchPropertyDefinitions))
				{
					bool flag = true;
					while (flag)
					{
						object[][] rows = queryResult.GetRows(rowCount, out flag);
						if (rows == null || rows.Length == 0)
						{
							break;
						}
						for (int i = 0; i < rows.Length; i++)
						{
							if (totalItemCount < 50)
							{
								ConversationType conversationFromDataRow = SearchFolderConversationRetriever.GetConversationFromDataRow(searchContext, mailboxSession, rows[i], SearchFolderConversationRetriever.itemSearchPropertyDefinitionsOrderDictionary);
								list.Add(conversationFromDataRow);
							}
							totalItemCount++;
						}
					}
				}
			}
			return list.ToArray();
		}

		private static ConversationType GetConversationFromDataRow(OwaSearchContext searchContext, MailboxSession mailboxSession, object[] row, Dictionary<PropertyDefinition, int> orderDictionary)
		{
			ConversationType conversationType = new ConversationType();
			conversationType.InstanceKey = SearchFolderDataRetrieverBase.GetItemProperty<byte[]>(row, orderDictionary[ItemSchema.InstanceKey], null);
			ConversationId itemProperty = SearchFolderDataRetrieverBase.GetItemProperty<ConversationId>(row, orderDictionary[ConversationItemSchema.ConversationId], null);
			conversationType.ConversationId = new ItemId(IdConverter.ConversationIdToEwsId(mailboxSession.MailboxGuid, itemProperty), null);
			conversationType.ConversationTopic = SearchFolderDataRetrieverBase.GetItemProperty<string>(row, orderDictionary[ConversationItemSchema.ConversationTopic]);
			conversationType.UniqueRecipients = SearchFolderDataRetrieverBase.GetItemProperty<string[]>(row, orderDictionary[ConversationItemSchema.ConversationMVTo]);
			conversationType.UniqueSenders = SearchFolderDataRetrieverBase.GetItemProperty<string[]>(row, orderDictionary[ConversationItemSchema.ConversationMVFrom]);
			conversationType.LastDeliveryTime = SearchFolderDataRetrieverBase.GetDateTimeProperty(searchContext.RequestTimeZone, row, orderDictionary[ConversationItemSchema.ConversationLastDeliveryTime]);
			conversationType.Categories = SearchFolderDataRetrieverBase.GetItemProperty<string[]>(row, orderDictionary[ConversationItemSchema.ConversationCategories]);
			if (SearchFolderDataRetrieverBase.IsPropertyDefined(row, orderDictionary[ConversationItemSchema.ConversationFlagStatus]))
			{
				FlagStatus itemProperty2 = (FlagStatus)SearchFolderDataRetrieverBase.GetItemProperty<int>(row, orderDictionary[ConversationItemSchema.ConversationFlagStatus], 0);
				if (itemProperty2 != FlagStatus.NotFlagged)
				{
					conversationType.FlagStatus = new FlagType
					{
						FlagStatus = itemProperty2
					}.FlagStatus;
				}
			}
			conversationType.HasAttachments = new bool?(SearchFolderDataRetrieverBase.GetItemProperty<bool>(row, orderDictionary[ConversationItemSchema.ConversationHasAttach]));
			conversationType.HasIrm = new bool?(SearchFolderDataRetrieverBase.GetItemProperty<bool>(row, orderDictionary[ConversationItemSchema.ConversationHasIrm]));
			conversationType.MessageCount = new int?(SearchFolderDataRetrieverBase.GetItemProperty<int>(row, orderDictionary[ConversationItemSchema.ConversationMessageCount]));
			conversationType.GlobalMessageCount = new int?(SearchFolderDataRetrieverBase.GetItemProperty<int>(row, orderDictionary[ConversationItemSchema.ConversationGlobalMessageCount]));
			conversationType.UnreadCount = new int?(SearchFolderDataRetrieverBase.GetItemProperty<int>(row, orderDictionary[ConversationItemSchema.ConversationUnreadMessageCount]));
			conversationType.GlobalUnreadCount = new int?(SearchFolderDataRetrieverBase.GetItemProperty<int>(row, orderDictionary[ConversationItemSchema.ConversationGlobalUnreadMessageCount]));
			conversationType.Size = new int?(SearchFolderDataRetrieverBase.GetItemProperty<int>(row, orderDictionary[ConversationItemSchema.ConversationMessageSize]));
			conversationType.ItemClasses = SearchFolderDataRetrieverBase.GetItemProperty<string[]>(row, orderDictionary[ConversationItemSchema.ConversationMessageClasses]);
			conversationType.ImportanceString = ((ImportanceType)SearchFolderDataRetrieverBase.GetItemProperty<int>(row, orderDictionary[ConversationItemSchema.ConversationImportance], 1)).ToString();
			StoreId[] itemProperty3 = SearchFolderDataRetrieverBase.GetItemProperty<StoreId[]>(row, orderDictionary[ConversationItemSchema.ConversationItemIds], new StoreId[0]);
			conversationType.ItemIds = Array.ConvertAll<StoreId, ItemId>(itemProperty3, (StoreId s) => new ItemId(SearchFolderDataRetrieverBase.GetEwsId(s, mailboxSession.MailboxGuid), null));
			StoreId[] itemProperty4 = SearchFolderDataRetrieverBase.GetItemProperty<StoreId[]>(row, orderDictionary[ConversationItemSchema.ConversationGlobalItemIds], new StoreId[0]);
			conversationType.GlobalItemIds = Array.ConvertAll<StoreId, ItemId>(itemProperty4, (StoreId s) => new ItemId(SearchFolderDataRetrieverBase.GetEwsId(s, mailboxSession.MailboxGuid), null));
			conversationType.LastModifiedTime = SearchFolderDataRetrieverBase.GetDateTimeProperty(searchContext.RequestTimeZone, row, orderDictionary[StoreObjectSchema.LastModifiedTime]);
			conversationType.Preview = SearchFolderDataRetrieverBase.GetItemProperty<string>(row, orderDictionary[ConversationItemSchema.ConversationPreview]);
			IconIndex itemProperty5 = (IconIndex)SearchFolderDataRetrieverBase.GetItemProperty<int>(row, orderDictionary[ConversationItemSchema.ConversationReplyForwardState]);
			if (itemProperty5 > (IconIndex)0)
			{
				conversationType.IconIndexString = itemProperty5.ToString();
			}
			itemProperty5 = (IconIndex)SearchFolderDataRetrieverBase.GetItemProperty<int>(row, orderDictionary[ConversationItemSchema.ConversationGlobalReplyForwardState]);
			if (itemProperty5 > (IconIndex)0)
			{
				conversationType.GlobalIconIndexString = itemProperty5.ToString();
			}
			return conversationType;
		}

		private static PropertyDefinition[] itemSearchPropertyDefinitions = new PropertyDefinition[]
		{
			ItemSchema.InstanceKey,
			ConversationItemSchema.ConversationId,
			ConversationItemSchema.ConversationTopic,
			ConversationItemSchema.ConversationMVTo,
			ConversationItemSchema.ConversationMVFrom,
			ConversationItemSchema.ConversationLastDeliveryTime,
			ConversationItemSchema.ConversationCategories,
			ConversationItemSchema.ConversationFlagStatus,
			ConversationItemSchema.ConversationHasAttach,
			ConversationItemSchema.ConversationHasIrm,
			ConversationItemSchema.ConversationMessageCount,
			ConversationItemSchema.ConversationGlobalMessageCount,
			ConversationItemSchema.ConversationUnreadMessageCount,
			ConversationItemSchema.ConversationGlobalUnreadMessageCount,
			ConversationItemSchema.ConversationMessageSize,
			ConversationItemSchema.ConversationMessageClasses,
			ConversationItemSchema.ConversationImportance,
			ConversationItemSchema.ConversationItemIds,
			ConversationItemSchema.ConversationGlobalItemIds,
			StoreObjectSchema.LastModifiedTime,
			ConversationItemSchema.ConversationPreview,
			ConversationItemSchema.ConversationReplyForwardState,
			ConversationItemSchema.ConversationGlobalReplyForwardState
		};

		private static Dictionary<PropertyDefinition, int> itemSearchPropertyDefinitionsOrderDictionary = new Dictionary<PropertyDefinition, int>(SearchFolderConversationRetriever.itemSearchPropertyDefinitions.Length);
	}
}
