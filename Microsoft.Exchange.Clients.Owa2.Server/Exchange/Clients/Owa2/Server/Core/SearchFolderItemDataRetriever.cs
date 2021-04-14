using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SearchFolderItemDataRetriever : SearchFolderDataRetrieverBase
	{
		static SearchFolderItemDataRetriever()
		{
			for (int i = 0; i < SearchFolderItemDataRetriever.itemSearchPropertyDefinitions.Length; i++)
			{
				SearchFolderItemDataRetriever.itemSearchPropertyDefinitionsOrderDictionary[SearchFolderItemDataRetriever.itemSearchPropertyDefinitions[i]] = i;
			}
		}

		public static ItemType[] GetItemDataFromSearchFolder(OwaSearchContext searchContext, MailboxSession mailboxSession, out int totalRowCount)
		{
			List<ItemType> list = new List<ItemType>(50);
			totalRowCount = 0;
			StoreId searchFolderId = searchContext.SearchFolderId;
			SortBy[] searchSortBy = searchContext.SearchSortBy;
			using (SearchFolder searchFolder = SearchFolder.Bind(mailboxSession, searchFolderId))
			{
				int rowCount = 25;
				using (QueryResult queryResult = searchFolder.ItemQuery(ItemQueryType.None, null, searchSortBy, SearchFolderItemDataRetriever.itemSearchPropertyDefinitions))
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
							if (totalRowCount < 50)
							{
								ItemType itemFromDataRow = SearchFolderItemDataRetriever.GetItemFromDataRow(searchContext, mailboxSession, rows[i], SearchFolderItemDataRetriever.itemSearchPropertyDefinitionsOrderDictionary);
								list.Add(itemFromDataRow);
							}
							totalRowCount++;
						}
					}
				}
			}
			return list.ToArray();
		}

		private static ItemType GetItemFromDataRow(OwaSearchContext searchContext, MailboxSession mailboxSession, object[] row, Dictionary<PropertyDefinition, int> orderDictionary)
		{
			StoreId itemProperty = SearchFolderDataRetrieverBase.GetItemProperty<StoreId>(row, orderDictionary[ItemSchema.Id], null);
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(itemProperty);
			ItemType itemType = ItemType.CreateFromStoreObjectType(storeObjectId.ObjectType);
			itemType.ItemId = SearchFolderDataRetrieverBase.StoreIdToEwsItemId(itemProperty, new MailboxId(mailboxSession.MailboxGuid, false));
			byte[] itemProperty2 = SearchFolderDataRetrieverBase.GetItemProperty<byte[]>(row, orderDictionary[ItemSchema.InstanceKey], new byte[0]);
			itemType.InstanceKey = itemProperty2;
			itemType.InstanceKeyString = Convert.ToBase64String(itemProperty2);
			itemType.ParentFolderId = new FolderId(SearchFolderDataRetrieverBase.GetEwsId(SearchFolderDataRetrieverBase.GetItemProperty<StoreId>(row, orderDictionary[StoreObjectSchema.ParentItemId], null), mailboxSession.MailboxGuid), null);
			itemType.ConversationId = new ItemId(IdConverter.ConversationIdToEwsId(mailboxSession.MailboxGuid, SearchFolderDataRetrieverBase.GetItemProperty<ConversationId>(row, orderDictionary[ItemSchema.ConversationId], null)), null);
			itemType.Subject = SearchFolderDataRetrieverBase.GetItemProperty<string>(row, orderDictionary[ItemSchema.Subject], string.Empty);
			itemType.ImportanceString = SearchFolderDataRetrieverBase.GetItemProperty<Importance>(row, orderDictionary[ItemSchema.Importance], Importance.Normal).ToString();
			itemType.SensitivityString = SearchFolderDataRetrieverBase.GetItemProperty<Sensitivity>(row, orderDictionary[ItemSchema.Sensitivity], Sensitivity.Normal).ToString();
			string dateTimeProperty = SearchFolderDataRetrieverBase.GetDateTimeProperty(searchContext.RequestTimeZone, row, orderDictionary[ItemSchema.ReceivedTime]);
			if (!string.IsNullOrEmpty(dateTimeProperty))
			{
				itemType.DateTimeReceived = dateTimeProperty;
			}
			string dateTimeProperty2 = SearchFolderDataRetrieverBase.GetDateTimeProperty(searchContext.RequestTimeZone, row, orderDictionary[ItemSchema.SentTime]);
			if (!string.IsNullOrEmpty(dateTimeProperty2))
			{
				itemType.DateTimeSent = dateTimeProperty2;
			}
			itemType.HasAttachments = new bool?(SearchFolderDataRetrieverBase.GetItemProperty<bool>(row, orderDictionary[ItemSchema.HasAttachment], false));
			itemType.IsDraft = new bool?(SearchFolderDataRetrieverBase.GetItemProperty<bool>(row, orderDictionary[MessageItemSchema.IsDraft], false));
			itemType.ItemClass = SearchFolderDataRetrieverBase.GetItemProperty<string>(row, orderDictionary[StoreObjectSchema.ItemClass], string.Empty);
			itemType.Preview = SearchFolderDataRetrieverBase.GetItemProperty<string>(row, orderDictionary[ItemSchema.Preview], string.Empty);
			MessageType messageType = itemType as MessageType;
			if (messageType != null)
			{
				Participant itemProperty3 = SearchFolderDataRetrieverBase.GetItemProperty<Participant>(row, orderDictionary[ItemSchema.From], null);
				if (itemProperty3 != null)
				{
					messageType.From = RowNotificationHandler.CreateRecipientFromParticipant(itemProperty3);
				}
				Participant itemProperty4 = SearchFolderDataRetrieverBase.GetItemProperty<Participant>(row, orderDictionary[ItemSchema.Sender], null);
				if (itemProperty4 != null)
				{
					messageType.Sender = RowNotificationHandler.CreateRecipientFromParticipant(itemProperty4);
				}
				messageType.IsRead = new bool?(SearchFolderDataRetrieverBase.GetItemProperty<bool>(row, orderDictionary[MessageItemSchema.IsRead], false));
			}
			IconIndex itemProperty5 = SearchFolderDataRetrieverBase.GetItemProperty<IconIndex>(row, orderDictionary[ItemSchema.IconIndex], IconIndex.Default);
			if (itemProperty5 != IconIndex.Default)
			{
				itemType.IconIndexString = itemProperty5.ToString();
			}
			FlagType flagType = new FlagType();
			itemType.Flag = flagType;
			flagType.FlagStatus = SearchFolderDataRetrieverBase.GetItemProperty<FlagStatus>(row, orderDictionary[ItemSchema.FlagStatus], FlagStatus.NotFlagged);
			itemType.DateTimeCreated = SearchFolderDataRetrieverBase.GetDateTimeProperty(searchContext.RequestTimeZone, row, orderDictionary[StoreObjectSchema.CreationTime]);
			itemType.LastModifiedTime = SearchFolderDataRetrieverBase.GetDateTimeProperty(searchContext.RequestTimeZone, row, orderDictionary[StoreObjectSchema.LastModifiedTime]);
			itemType.Size = SearchFolderDataRetrieverBase.GetItemProperty<int?>(row, orderDictionary[ItemSchema.Size], null);
			itemType.DisplayTo = SearchFolderDataRetrieverBase.GetItemProperty<string>(row, orderDictionary[ItemSchema.DisplayTo], null);
			itemType.Categories = SearchFolderDataRetrieverBase.GetItemProperty<string[]>(row, orderDictionary[ItemSchema.Categories], null);
			return itemType;
		}

		private const int MAXITEMCOUNT = 50;

		private static PropertyDefinition[] itemSearchPropertyDefinitions = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.InstanceKey,
			ItemSchema.Subject,
			ItemSchema.Importance,
			ItemSchema.Sensitivity,
			ItemSchema.ReceivedTime,
			ItemSchema.SentTime,
			ItemSchema.HasAttachment,
			StoreObjectSchema.ParentItemId,
			ItemSchema.ConversationId,
			ItemSchema.Categories,
			MessageItemSchema.IsDraft,
			StoreObjectSchema.ItemClass,
			ItemSchema.Preview,
			ItemSchema.Sender,
			ItemSchema.From,
			ItemSchema.FlagStatus,
			ItemSchema.CompleteDate,
			TaskSchema.StartDate,
			TaskSchema.DueDate,
			StoreObjectSchema.CreationTime,
			StoreObjectSchema.LastModifiedTime,
			MessageItemSchema.IsRead,
			ItemSchema.Size,
			ItemSchema.DisplayTo,
			ItemSchema.IconIndex
		};

		private static Dictionary<PropertyDefinition, int> itemSearchPropertyDefinitionsOrderDictionary = new Dictionary<PropertyDefinition, int>(SearchFolderItemDataRetriever.itemSearchPropertyDefinitions.Length);
	}
}
