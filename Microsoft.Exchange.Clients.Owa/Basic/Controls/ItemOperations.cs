using System;
using System.Collections;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal static class ItemOperations
	{
		public static ItemOperations.Result GetNextViewItem(UserContext userContext, ItemOperations.Action action, StoreObjectId itemId, StoreObjectId folderId)
		{
			Folder folder = null;
			ItemOperations.Result result;
			try
			{
				MessageModuleSearchViewState messageModuleSearchViewState = userContext.LastClientViewState as MessageModuleSearchViewState;
				SortBy[] sortOrder;
				if (messageModuleSearchViewState != null)
				{
					using (Folder folder2 = Folder.Bind(userContext.MailboxSession, messageModuleSearchViewState.FolderId))
					{
						sortOrder = ItemOperations.GetSortOrder(userContext, folder2);
						FolderSearch folderSearch = new FolderSearch();
						folder = folderSearch.Execute(userContext, folder2, messageModuleSearchViewState.SearchScope, messageModuleSearchViewState.SearchString, userContext.ForceNewSearch, false);
						userContext.ForceNewSearch = false;
						goto IL_77;
					}
				}
				folder = Folder.Bind(userContext.MailboxSession, folderId);
				sortOrder = ItemOperations.GetSortOrder(userContext, folder);
				IL_77:
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, sortOrder, new PropertyDefinition[]
				{
					ItemSchema.Id
				}))
				{
					queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Id, itemId));
					int num = queryResult.CurrentRow;
					int num2 = queryResult.EstimatedRowCount;
					int num3 = 0;
					if (num == num2 || num2 == 1)
					{
						result = null;
					}
					else
					{
						switch (action)
						{
						case ItemOperations.Action.Next:
							if (num + 1 == num2)
							{
								return null;
							}
							num3 = queryResult.SeekToOffset(SeekReference.OriginCurrent, 1);
							break;
						case ItemOperations.Action.Prev:
							if (num == 0)
							{
								return null;
							}
							num3 = queryResult.SeekToOffset(SeekReference.OriginCurrent, -1);
							break;
						case ItemOperations.Action.Delete:
							if (userContext.UserOptions.NextSelection == NextSelectionDirection.Previous)
							{
								if (num == 0)
								{
									num3 = queryResult.SeekToOffset(SeekReference.OriginCurrent, 1);
									num3 = 0;
								}
								else
								{
									num3 = queryResult.SeekToOffset(SeekReference.OriginCurrent, -1);
								}
							}
							else if (userContext.UserOptions.NextSelection == NextSelectionDirection.Next)
							{
								if (num2 - (num + 1) == 0)
								{
									num3 = queryResult.SeekToOffset(SeekReference.OriginCurrent, -1);
								}
								else
								{
									num3 = queryResult.SeekToOffset(SeekReference.OriginCurrent, 1);
									num3 = 0;
								}
							}
							else if (userContext.UserOptions.NextSelection == NextSelectionDirection.ReturnToView)
							{
								return null;
							}
							num2--;
							break;
						}
						num += num3;
						if (messageModuleSearchViewState != null)
						{
							messageModuleSearchViewState.PageNumber = num / userContext.UserOptions.BasicViewRowCount + 1;
						}
						else
						{
							ListViewViewState listViewViewState = userContext.LastClientViewState as ListViewViewState;
							if (listViewViewState != null && folderId.Equals(listViewViewState.FolderId))
							{
								listViewViewState.PageNumber = num / userContext.UserOptions.BasicViewRowCount + 1;
							}
						}
						object[][] rows = queryResult.GetRows(1);
						result = new ItemOperations.Result(((VersionedId)rows[0][0]).ObjectId, folderId);
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				result = null;
			}
			finally
			{
				if (folder != null)
				{
					folder.Dispose();
				}
			}
			return result;
		}

		private static SortBy[] GetSortOrder(UserContext userContext, Folder folder)
		{
			ColumnId columnId = ColumnId.DeliveryTime;
			SortOrder sortOrder = SortOrder.Descending;
			WebPartModuleViewState webPartModuleViewState = userContext.LastClientViewState as WebPartModuleViewState;
			if (webPartModuleViewState != null && folder.Id.ObjectId.Equals(webPartModuleViewState.FolderId))
			{
				sortOrder = webPartModuleViewState.SortOrder;
				columnId = webPartModuleViewState.SortedColumn;
			}
			else if (!userContext.IsWebPartRequest)
			{
				using (UserConfiguration folderConfiguration = UserConfigurationUtilities.GetFolderConfiguration("Owa.BasicFolderOption", userContext, folder.Id))
				{
					if (folderConfiguration != null)
					{
						IDictionary dictionary = folderConfiguration.GetDictionary();
						object obj = dictionary["SortColumn"];
						if (obj != null)
						{
							columnId = ColumnIdParser.Parse((string)obj);
						}
						obj = dictionary["SortOrder"];
						if (obj != null)
						{
							sortOrder = (SortOrder)obj;
						}
					}
				}
			}
			if (!ListViewColumns.IsSupportedColumnId(columnId))
			{
				columnId = ColumnId.DeliveryTime;
			}
			SortBy[] result;
			if (columnId == ColumnId.DeliveryTime)
			{
				result = new SortBy[]
				{
					new SortBy(ItemSchema.ReceivedTime, sortOrder)
				};
			}
			else
			{
				Column column = ListViewColumns.GetColumn(columnId);
				result = new SortBy[]
				{
					new SortBy(column[0], sortOrder),
					new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
				};
			}
			return result;
		}

		public static ItemOperations.Result DeleteItem(UserContext userContext, StoreObjectId itemId, StoreObjectId folderId)
		{
			ItemOperations.Result result = null;
			ListViewViewState listViewViewState = userContext.LastClientViewState as ListViewViewState;
			if (listViewViewState != null && folderId.Equals(listViewViewState.FolderId))
			{
				result = ItemOperations.GetNextViewItem(userContext, ItemOperations.Action.Delete, itemId, folderId);
			}
			if (Utilities.IsDefaultFolderId(userContext.MailboxSession, folderId, DefaultFolderType.DeletedItems))
			{
				Utilities.DeleteItems(userContext, DeleteItemFlags.SoftDelete, new StoreId[]
				{
					itemId
				});
			}
			else
			{
				Utilities.DeleteItems(userContext, DeleteItemFlags.MoveToDeletedItems, new StoreId[]
				{
					itemId
				});
			}
			return result;
		}

		public static PreFormActionResponse GetPreFormActionResponse(UserContext userContext, ItemOperations.Result result)
		{
			if (result == null)
			{
				return userContext.LastClientViewState.ToPreFormActionResponse();
			}
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Item;
			preFormActionResponse.AddParameter("id", result.ItemId.ToBase64String());
			if (!Utilities.IsDefaultFolderId(userContext.MailboxSession, result.FolderId, DefaultFolderType.JunkEmail))
			{
				using (Item item = Item.Bind(userContext.MailboxSession, result.ItemId, new PropertyDefinition[]
				{
					StoreObjectSchema.ItemClass,
					MessageItemSchema.IsDraft,
					CalendarItemBaseSchema.IsMeeting,
					CalendarItemBaseSchema.IsOrganizer
				}))
				{
					preFormActionResponse.Type = item.ClassName;
					if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(item.ClassName))
					{
						if (ItemUtility.GetProperty<bool>(item, CalendarItemBaseSchema.IsMeeting, false) && !ItemUtility.GetProperty<bool>(item, CalendarItemBaseSchema.IsOrganizer, false))
						{
							preFormActionResponse.Action = "Read";
						}
						else
						{
							preFormActionResponse.Action = "Open";
						}
					}
					else if ((ObjectClass.IsMessage(item.ClassName, false) || ObjectClass.IsMeetingMessage(item.ClassName)) && ItemUtility.GetProperty<bool>(item, MessageItemSchema.IsDraft, false))
					{
						preFormActionResponse.Action = "Open";
						preFormActionResponse.State = "Draft";
					}
				}
			}
			return preFormActionResponse;
		}

		public enum Action
		{
			None,
			Next,
			Prev,
			Delete
		}

		public class Result
		{
			public Result(StoreObjectId itemId, StoreObjectId folderId)
			{
				this.ItemId = itemId;
				this.FolderId = folderId;
			}

			public readonly StoreObjectId FolderId;

			public readonly StoreObjectId ItemId;
		}
	}
}
