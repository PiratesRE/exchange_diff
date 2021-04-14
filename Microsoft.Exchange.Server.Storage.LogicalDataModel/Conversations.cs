using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class Conversations
	{
		public static IList<object> GetConversationsInFolder(Context context, Mailbox mailbox, ExchangeId folderId)
		{
			MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(mailbox.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				folderId.To26ByteArray(),
				false
			});
			SearchCriteria restriction = Factory.CreateSearchCriteriaCompare(messageTable.ConversationDocumentId, SearchCriteriaCompare.SearchRelOp.NotEqual, Factory.CreateConstantColumn(null, messageTable.ConversationDocumentId));
			SimpleQueryOperator query = Factory.CreateTableOperator(context.Culture, context, messageTable.Table, messageTable.MessageUnique, new PhysicalColumn[]
			{
				messageTable.ConversationDocumentId
			}, restriction, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true);
			return Conversations.GetConversationsInQuery(context, query, messageTable.ConversationDocumentId);
		}

		public static IList<object> GetConversationsInSearchFolder(Context context, Mailbox mailbox, ExchangeId folderId)
		{
			SearchFolder searchFolder = (SearchFolder)Folder.OpenFolder(context, mailbox, folderId);
			MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(mailbox.Database);
			SearchCriteria criteria = Factory.CreateSearchCriteriaCompare(messageTable.ConversationDocumentId, SearchCriteriaCompare.SearchRelOp.NotEqual, Factory.CreateConstantColumn(null, messageTable.ConversationDocumentId));
			SimpleQueryOperator query = searchFolder.BaseViewOperator(context, mailbox, new PhysicalColumn[]
			{
				messageTable.ConversationDocumentId
			}, criteria, new bool?(false));
			return Conversations.GetConversationsInQuery(context, query, messageTable.ConversationDocumentId);
		}

		public static void TrackConversationUpdate(Context context, Mailbox mailbox, TopMessage message, LogicalIndex.LogicalOperation operation, ModifiedSearchFolders modifiedSearchFolders)
		{
			if (!mailbox.GetConversationEnabled(context))
			{
				return;
			}
			if (message is ConversationItem)
			{
				ConversationItem conversationItem = (ConversationItem)message;
				switch (operation)
				{
				case LogicalIndex.LogicalOperation.Insert:
					Conversations.TrackConversationIndexUpdateForConversationCreation(context, mailbox, conversationItem);
					break;
				case LogicalIndex.LogicalOperation.Update:
					Conversations.TrackConversationIndexUpdateForConversationUpdate(context, mailbox, conversationItem);
					break;
				case LogicalIndex.LogicalOperation.Delete:
					Conversations.TrackConversationIndexUpdateForConversationDeletion(context, mailbox, conversationItem);
					break;
				}
				conversationItem.ModifiedMessage = null;
				conversationItem.ModifiedSearchFolders = null;
				return;
			}
			switch (operation)
			{
			case LogicalIndex.LogicalOperation.Insert:
				Conversations.TrackConversationUpdateForMessageInsert(context, mailbox, message, modifiedSearchFolders);
				return;
			case LogicalIndex.LogicalOperation.Update:
				Conversations.TrackConversationUpdateForMessageReplace(context, mailbox, message, modifiedSearchFolders);
				return;
			case LogicalIndex.LogicalOperation.Delete:
				Conversations.TrackConversationUpdateForMessageDelete(context, mailbox, message, modifiedSearchFolders);
				return;
			default:
				throw new ArgumentOutOfRangeException("operation", operation, "Unexpected operation");
			}
		}

		public static ConversationItem OpenOrCreateConversation(Context context, Mailbox mailbox, TopMessage message)
		{
			if (message is ConversationItem || !mailbox.GetConversationEnabled(context))
			{
				return null;
			}
			if (Folder.OpenFolder(context, mailbox, ConversationItem.GetConversationFolderId(context, mailbox)) == null)
			{
				throw new StoreException((LID)38940U, ErrorCodeValue.CorruptStore);
			}
			byte[] messageConversationId = Conversations.GetMessageConversationId(context, message);
			if (messageConversationId == null || Conversations.MessageIsInNonIpmFolder(context, message))
			{
				if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Message is not in Conversation");
				}
				return null;
			}
			ConversationItem conversationItem;
			if (!message.IsNew)
			{
				byte[] x = (byte[])message.GetOriginalPropertyValue(context, PropTag.Message.ConversationId);
				if (ValueHelper.ValuesEqual(x, messageConversationId) && message.GetConversationDocumentId(context) != null)
				{
					conversationItem = ConversationItem.OpenConversationItem(context, mailbox, message.GetConversationDocumentId(context).Value);
					if (conversationItem != null)
					{
						if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Updating Conversation");
						}
						return conversationItem;
					}
					throw new StoreException((LID)43036U, ErrorCodeValue.CorruptStore);
				}
			}
			conversationItem = ConversationItem.OpenConversationItem(context, mailbox, messageConversationId);
			if (conversationItem != null)
			{
				if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Updating Conversation");
				}
				bool flag = false;
				if (conversationItem.IsMaxSize)
				{
					if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Conversation exceeds maximal size. Changing message ConversationIndex.");
					}
					flag = true;
				}
				if (mailbox.SharedState.UnifiedState != null && (int)message.GetPropertyValue(context, PropTag.Message.MailboxNum) != (int)conversationItem.GetPropertyValue(context, PropTag.Message.MailboxNum))
				{
					if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Unified mailbox: conversation collision.");
					}
					flag = true;
				}
				if (flag)
				{
					conversationItem.Dispose();
					conversationItem = null;
					message.SetProperty(context, PropTag.Message.ConversationIndex, ConversationIdHelpers.GenerateNewConversationIndex());
					message.SetProperty(context, PropTag.Message.ConversationIndexTracking, true);
					messageConversationId = Conversations.GetMessageConversationId(context, message);
				}
			}
			if (conversationItem == null)
			{
				if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Creating new Conversation");
				}
				conversationItem = ConversationItem.CreateConversationItem(context, mailbox, messageConversationId, message);
			}
			return conversationItem;
		}

		private static bool MessageIsInNonIpmFolder(Context context, TopMessage message)
		{
			return !message.ParentFolder.IsIpmFolder(context);
		}

		private static bool MessageWasInNonIpmFolder(Context context, TopMessage message)
		{
			return !message.OriginalFolder.IsIpmFolder(context);
		}

		private static IList<object> GetConversationsInQuery(Context context, SimpleQueryOperator query, Column conversationIdColumn)
		{
			HashSet<object> hashSet = new HashSet<object>();
			using (Reader reader = query.ExecuteReader(true))
			{
				while (reader.Read())
				{
					hashSet.Add(reader.GetValue(conversationIdColumn));
				}
			}
			List<object> list = new List<object>(hashSet);
			list.Sort((object x, object y) => ((int)x).CompareTo((int)y));
			return list;
		}

		public static byte[] GetMessageConversationId(Context context, TopMessage message)
		{
			if (!Conversations.MessageIsInNonIpmFolder(context, message))
			{
				return (byte[])message.GetPropertyValue(context, PropTag.Message.ConversationId);
			}
			return null;
		}

		public static byte[] GetMessageConversationId(Context context, Mailbox mailbox, int documentId)
		{
			MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(mailbox.Database);
			Column conversationIdColumn = ConversationItem.GetConversationIdColumn(mailbox);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber,
				documentId
			});
			byte[] result;
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, messageTable.Table, messageTable.Table.PrimaryKeyIndex, new Column[]
			{
				conversationIdColumn
			}, null, null, 0, 0, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					byte[] array = reader.Read() ? ((byte[])reader.GetValue(conversationIdColumn)) : null;
					result = array;
				}
			}
			return result;
		}

		public static ExchangeId EnableConversationsForMailbox(Context context, Mailbox mailbox)
		{
			mailbox.SetConversationEnabled(context);
			FolderTable folderTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(mailbox.Database);
			StartStopKey startStopKey = new StartStopKey(true, new object[]
			{
				mailbox.MailboxPartitionNumber
			});
			SearchCriteria restriction = Factory.CreateSearchCriteriaCompare(folderTable.SpecialFolderNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(20));
			Column[] columnsToFetch = new Column[]
			{
				folderTable.FolderId
			};
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, folderTable.Table, folderTable.Table.PrimaryKeyIndex, columnsToFetch, restriction, null, 0, 1, new KeyRange(startStopKey, startStopKey), false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					if (reader.Read())
					{
						return ExchangeId.CreateFrom26ByteArray(context, mailbox.ReplidGuidMap, reader.GetBinary(folderTable.FolderId));
					}
				}
			}
			Folder folder = Folder.CreateFolder(context, mailbox);
			folder.SetName(context, "Conversations");
			folder.SetSpecialFolderNumber(context, SpecialFolders.Conversations);
			folder.SetProperty(context, PropTag.Folder.DisablePerUserRead, true);
			if (mailbox.SharedState.UnifiedState != null)
			{
				folder.SetColumn(context, folderTable.MailboxNumber, -1);
			}
			folder.Save(context);
			return folder.GetId(context);
		}

		private static void TrackConversationUpdateForMessageInsert(Context context, Mailbox mailbox, TopMessage message, ModifiedSearchFolders modifiedSearchFolders)
		{
			ConversationItem conversationItem = message.ConversationItem;
			if (conversationItem == null)
			{
				return;
			}
			if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Adding message to Conversation");
			}
			conversationItem.AddMessage(context, new FidMid(message.GetFolderId(context), message.GetId(context)));
			conversationItem.ModifiedMessage = message;
			conversationItem.ModifiedSearchFolders = modifiedSearchFolders;
			if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Saving Conversation");
			}
			conversationItem.SaveChanges(context, SaveMessageChangesFlags.SkipQuotaCheck);
		}

		private static void TrackConversationUpdateForMessageDelete(Context context, Mailbox mailbox, TopMessage message, ModifiedSearchFolders modifiedSearchFolders)
		{
			if (message.GetOriginalConversationDocumentId(context) == null)
			{
				return;
			}
			using (ConversationItem conversationItem = ConversationItem.OpenConversationItem(context, mailbox, message.GetOriginalConversationDocumentId(context).Value))
			{
				if (conversationItem == null)
				{
					throw new StoreException((LID)55324U, ErrorCodeValue.CorruptStore);
				}
				if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Removing message from Conversation");
				}
				conversationItem.ModifiedMessage = message;
				conversationItem.ModifiedSearchFolders = modifiedSearchFolders;
				conversationItem.RemoveMessage(context, new FidMid(message.GetOriginalFolderId(context), message.OriginalMessageID));
				if (conversationItem.IsEmpty)
				{
					if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Deleting empty Conversation");
					}
					conversationItem.Delete(context);
				}
				else
				{
					if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Saving Conversation");
					}
					conversationItem.SaveChanges(context, SaveMessageChangesFlags.SkipQuotaCheck);
				}
			}
		}

		private static void TrackConversationUpdateForMessageReplace(Context context, Mailbox mailbox, TopMessage message, ModifiedSearchFolders modifiedSearchFolders)
		{
			ConversationItem conversationItem = message.ConversationItem;
			bool flag = message.GetOriginalConversationDocumentId(context) != null;
			bool flag2 = conversationItem != null;
			if (!flag2 && !flag)
			{
				return;
			}
			if (flag2 != flag || !ValueHelper.ValuesEqual(message.GetOriginalPropertyValue(context, PropTag.Message.ConversationId), message.GetPropertyValue(context, PropTag.Message.ConversationId)))
			{
				if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					if (!flag2)
					{
						ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Message was in a conversation but now is not");
					}
					else if (!flag)
					{
						ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Message was not in a conversation but now is");
					}
					else
					{
						ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Message conversation id has changed");
					}
				}
				if (flag)
				{
					ModifiedSearchFolders modifiedSearchFolders2 = new ModifiedSearchFolders();
					modifiedSearchFolders2.DeletedFrom.UnionWith(modifiedSearchFolders.DeletedFrom);
					modifiedSearchFolders2.DeletedFrom.UnionWith(modifiedSearchFolders.Updated);
					Conversations.TrackConversationUpdateForMessageDelete(context, mailbox, message, modifiedSearchFolders2);
				}
				if (flag2)
				{
					ModifiedSearchFolders modifiedSearchFolders3 = new ModifiedSearchFolders();
					modifiedSearchFolders3.InsertedInto.UnionWith(modifiedSearchFolders.InsertedInto);
					modifiedSearchFolders3.InsertedInto.UnionWith(modifiedSearchFolders.Updated);
					Conversations.TrackConversationUpdateForMessageInsert(context, mailbox, message, modifiedSearchFolders3);
					return;
				}
			}
			else
			{
				ExchangeId originalFolderId = message.GetOriginalFolderId(context);
				ExchangeId folderId = message.GetFolderId(context);
				ExchangeId originalMessageID = message.OriginalMessageID;
				ExchangeId id = message.GetId(context);
				if (originalFolderId != folderId || originalMessageID != id)
				{
					if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Changing moved message FIDMID in a Conversation");
					}
					conversationItem.RemoveMessage(context, new FidMid(originalFolderId, originalMessageID));
					conversationItem.AddMessage(context, new FidMid(folderId, id));
				}
				conversationItem.ModifiedMessage = message;
				conversationItem.ModifiedSearchFolders = modifiedSearchFolders;
				if (ExTraceGlobals.ConversationsSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ConversationsSummaryTracer.TraceDebug(0L, "Saving Conversation");
				}
				conversationItem.SaveChanges(context, SaveMessageChangesFlags.ForceSave | SaveMessageChangesFlags.SkipMailboxQuotaCheck | SaveMessageChangesFlags.SkipFolderQuotaCheck);
			}
		}

		private static void TrackConversationIndexUpdateForConversationCreation(Context context, Mailbox mailbox, ConversationItem conversationItem)
		{
			ConversationMembers conversationMembers = conversationItem.GetConversationMembers(context, conversationItem.ModifiedMessage, null);
			Conversations.TrackConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, conversationMembers.FolderIds, LogicalIndex.LogicalOperation.Insert);
			Conversations.TrackSearchFolderConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, conversationItem.ModifiedSearchFolders.InsertedInto, LogicalIndex.LogicalOperation.Insert);
			Conversations.TrackSearchFolderConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, conversationItem.ModifiedSearchFolders.Updated, LogicalIndex.LogicalOperation.Insert);
		}

		private static void TrackConversationIndexUpdateForConversationDeletion(Context context, Mailbox mailbox, ConversationItem conversationItem)
		{
			ConversationMembers conversationMembers = conversationItem.GetConversationMembers(context, conversationItem.ModifiedMessage, null);
			Conversations.TrackConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, conversationMembers.OriginalFolderIds, LogicalIndex.LogicalOperation.Delete);
			Conversations.TrackSearchFolderConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, conversationItem.ModifiedSearchFolders.DeletedFrom, LogicalIndex.LogicalOperation.Delete);
			Conversations.TrackSearchFolderConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, conversationItem.ModifiedSearchFolders.Updated, LogicalIndex.LogicalOperation.Delete);
		}

		private static void TrackConversationIndexUpdateForConversationUpdate(Context context, Mailbox mailbox, ConversationItem conversationItem)
		{
			ConversationMembers conversationMembers = conversationItem.GetConversationMembers(context, conversationItem.ModifiedMessage, null);
			IEnumerable<ExchangeId> originalFolderIds = conversationMembers.OriginalFolderIds;
			IEnumerable<ExchangeId> folderIds = conversationMembers.FolderIds;
			IEnumerable<ExchangeId> enumerable = originalFolderIds.Intersect(folderIds);
			IEnumerable<ExchangeId> folderIds2 = originalFolderIds.Except(enumerable);
			IEnumerable<ExchangeId> folderIds3 = folderIds.Except(enumerable);
			Conversations.TrackConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, folderIds3, LogicalIndex.LogicalOperation.Insert);
			Conversations.TrackConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, folderIds2, LogicalIndex.LogicalOperation.Delete);
			Conversations.TrackConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, enumerable, LogicalIndex.LogicalOperation.Update);
			Conversations.TrackSearchFolderConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, conversationItem.ModifiedSearchFolders.InsertedInto, LogicalIndex.LogicalOperation.Insert);
			Conversations.TrackSearchFolderConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, conversationItem.ModifiedSearchFolders.DeletedFrom, LogicalIndex.LogicalOperation.Delete);
			Conversations.TrackSearchFolderConversationIndexUpdate(context, mailbox, conversationItem, conversationMembers, conversationItem.ModifiedSearchFolders.Updated, LogicalIndex.LogicalOperation.Update);
			Conversations.UpdateAffectedSearchFolderConversationIndexes(context, mailbox, conversationItem, conversationMembers);
		}

		private static void TrackConversationIndexUpdate(Context context, Mailbox mailbox, ConversationItem conversationItem, ConversationMembers conversationMembers, IEnumerable<ExchangeId> folderIds, LogicalIndex.LogicalOperation logicalOperation)
		{
			foreach (ExchangeId exchangeId in folderIds)
			{
				IColumnValueBag updatedPropBag = new ConversationValueBag(conversationItem, exchangeId, conversationMembers);
				LogicalIndexCache.TrackIndexUpdate(context, mailbox, exchangeId, LogicalIndexType.Conversations, logicalOperation, updatedPropBag);
				Folder folder = Folder.OpenFolder(context, mailbox, exchangeId);
				if (logicalOperation == LogicalIndex.LogicalOperation.Insert || logicalOperation == LogicalIndex.LogicalOperation.Delete)
				{
					folder.SetConversationCount(context, folder.GetConversationCount(context) + ((logicalOperation == LogicalIndex.LogicalOperation.Insert) ? 1L : -1L));
				}
				ObjectNotificationEvent nev = null;
				switch (logicalOperation)
				{
				case LogicalIndex.LogicalOperation.Insert:
					nev = NotificationEvents.CreateConversationCreatedEvent(context, folder, conversationItem);
					break;
				case LogicalIndex.LogicalOperation.Update:
					nev = NotificationEvents.CreateConversationModifiedEvent(context, folder, conversationItem);
					break;
				case LogicalIndex.LogicalOperation.Delete:
					nev = NotificationEvents.CreateConversationDeletedEvent(context, folder, conversationItem);
					break;
				}
				context.RiseNotificationEvent(nev);
				if (logicalOperation == LogicalIndex.LogicalOperation.Delete)
				{
					LogicalIndex logicalIndex = LogicalIndexCache.FindIndex(context, mailbox, exchangeId, LogicalIndexType.ConversationDeleteHistory, 1);
					if (logicalIndex == null)
					{
						MessageTable messageTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(mailbox.Database);
						Column column = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Internal9ByteChangeNumber);
						Column column2 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.Mid);
						Column column3 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.LastModificationTime);
						Column column4 = PropertySchema.MapToColumn(context.Database, ObjectType.Message, PropTag.Message.ConversationId);
						SortOrder sortOrder = (SortOrder)new SortOrderBuilder
						{
							column
						};
						Column[] nonKeyColumns = new Column[]
						{
							column2,
							column3,
							column4
						};
						logicalIndex = LogicalIndexCache.CreateIndex(context, mailbox, exchangeId, LogicalIndexType.ConversationDeleteHistory, 1, null, false, sortOrder, nonKeyColumns, null, messageTable.Table, true);
					}
					logicalIndex.LogUpdate(context, updatedPropBag, LogicalIndex.LogicalOperation.Insert);
				}
			}
		}

		public static void TrackSearchFolderConversationIndexUpdate(Context context, Mailbox mailbox, ConversationItem conversationItem, ConversationMembers conversationMembers, IEnumerable<ExchangeId> searchFolderIds, LogicalIndex.LogicalOperation originalLogicalOperation)
		{
			FidMid? fidMid = null;
			FidMid? fidMid2 = null;
			List<FidMid> list = null;
			List<FidMid> list2 = null;
			foreach (ExchangeId exchangeId in searchFolderIds)
			{
				if (LogicalIndexCache.FolderHasConversationIndex(context, mailbox, exchangeId))
				{
					if (fidMid == null)
					{
						TopMessage modifiedMessage = conversationItem.ModifiedMessage;
						fidMid = new FidMid?(new FidMid(modifiedMessage.GetFolderId(context), modifiedMessage.GetId(context)));
						fidMid2 = new FidMid?(new FidMid(modifiedMessage.GetOriginalFolderId(context), modifiedMessage.OriginalMessageID));
						List<FidMid> list3 = new List<FidMid>(conversationMembers.OriginalConversationMessages);
						list3.Remove(fidMid2.Value);
						list = list3;
						list3 = new List<FidMid>(conversationMembers.ConversationMessages);
						list3.Remove(fidMid.Value);
						list2 = list3;
					}
					SearchFolder searchFolder = (SearchFolder)Folder.OpenFolder(context, mailbox, exchangeId);
					ICollection<FidMid> collection = null;
					ICollection<FidMid> collection2 = null;
					if (list2 != null)
					{
						collection = searchFolder.FilterMessages(context, list2, new bool?(false));
						if (list != null)
						{
							collection2 = list.Intersect(collection).ToList<FidMid>();
						}
					}
					else
					{
						collection2 = searchFolder.FilterMessages(context, list, new bool?(false));
					}
					LogicalIndex.LogicalOperation operation = originalLogicalOperation;
					switch (originalLogicalOperation)
					{
					case LogicalIndex.LogicalOperation.Insert:
						if (collection.Count > 0)
						{
							operation = LogicalIndex.LogicalOperation.Update;
						}
						collection.Add(fidMid.Value);
						break;
					case LogicalIndex.LogicalOperation.Update:
						collection2.Add(fidMid2.Value);
						collection.Add(fidMid.Value);
						break;
					case LogicalIndex.LogicalOperation.Delete:
						if (collection != null && collection.Count > 0)
						{
							operation = LogicalIndex.LogicalOperation.Update;
						}
						collection2.Add(fidMid2.Value);
						break;
					default:
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Unknown logicalOperation");
						break;
					}
					IColumnValueBag updatedPropBag = new ConversationValueBag(conversationItem, collection2, collection, conversationMembers);
					LogicalIndexCache.TrackIndexUpdate(context, mailbox, exchangeId, LogicalIndexType.Conversations, operation, updatedPropBag);
					ObjectNotificationEvent nev = null;
					switch (operation)
					{
					case LogicalIndex.LogicalOperation.Insert:
						nev = NotificationEvents.CreateConversationCreatedEvent(context, searchFolder, conversationItem);
						break;
					case LogicalIndex.LogicalOperation.Update:
						nev = NotificationEvents.CreateConversationModifiedEvent(context, searchFolder, conversationItem);
						break;
					case LogicalIndex.LogicalOperation.Delete:
						nev = NotificationEvents.CreateConversationDeletedEvent(context, searchFolder, conversationItem);
						break;
					}
					context.RiseNotificationEvent(nev);
				}
			}
		}

		private static void UpdateAffectedSearchFolderConversationIndexes(Context context, Mailbox mailbox, ConversationItem conversationItem, ConversationMembers conversationMembers)
		{
			TopMessage modifiedMessage = conversationItem.ModifiedMessage;
			FidMid item = new FidMid(modifiedMessage.GetFolderId(context), modifiedMessage.GetId(context));
			List<FidMid> list = conversationMembers.ConversationMessages.ToList<FidMid>();
			list.Remove(item);
			IEnumerable<ExchangeId> folders = new HashSet<ExchangeId>(from fidMid in list
			select fidMid.FolderId);
			IEnumerable<ExchangeId> searchBacklinks = Conversations.GetSearchBacklinks(context, mailbox, folders);
			ISet<ExchangeId> set = new HashSet<ExchangeId>(searchBacklinks);
			set.UnionWith(Conversations.GetSearchBacklinks(context, mailbox, searchBacklinks));
			set.ExceptWith(conversationItem.ModifiedSearchFolders.InsertedInto);
			set.ExceptWith(conversationItem.ModifiedSearchFolders.DeletedFrom);
			set.ExceptWith(conversationItem.ModifiedSearchFolders.Updated);
			foreach (ExchangeId exchangeId in set)
			{
				if (LogicalIndexCache.FolderHasConversationIndex(context, mailbox, exchangeId))
				{
					SearchFolder searchFolder = (SearchFolder)Folder.OpenFolder(context, mailbox, exchangeId);
					ICollection<FidMid> collection = searchFolder.FilterMessages(context, list, new bool?(false));
					if (collection.Count > 0)
					{
						IColumnValueBag updatedPropBag = new ConversationValueBag(conversationItem, collection, collection, conversationMembers);
						LogicalIndexCache.TrackIndexUpdate(context, mailbox, exchangeId, LogicalIndexType.Conversations, LogicalIndex.LogicalOperation.Update, updatedPropBag);
						ObjectNotificationEvent nev = NotificationEvents.CreateConversationModifiedEvent(context, searchFolder, conversationItem);
						context.RiseNotificationEvent(nev);
					}
				}
			}
		}

		private static IEnumerable<ExchangeId> GetSearchBacklinks(Context context, Mailbox mailbox, IEnumerable<ExchangeId> folders)
		{
			ISet<ExchangeId> set = new HashSet<ExchangeId>();
			foreach (ExchangeId id in folders)
			{
				Folder folder = Folder.OpenFolder(context, mailbox, id);
				if (folder != null)
				{
					set.UnionWith(folder.GetSearchBacklinks(context, false));
					set.UnionWith(folder.GetSearchBacklinks(context, true));
				}
			}
			return set;
		}

		public const int ConversationDeleteHistoryIndexSignatureVersion = 1;
	}
}
