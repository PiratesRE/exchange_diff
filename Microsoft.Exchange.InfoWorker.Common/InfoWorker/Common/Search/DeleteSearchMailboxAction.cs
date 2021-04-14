using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal sealed class DeleteSearchMailboxAction : SearchMailboxAction
	{
		public override SearchMailboxAction Clone()
		{
			return new DeleteSearchMailboxAction();
		}

		public override void PerformBatchOperation(object[][] batchedItemBuffer, int fetchedItemCount, StoreId currentFolderId, MailboxSession sourceMailbox, MailboxSession targetMailbox, Dictionary<StoreId, FolderNode> folderNodeMap, SearchResultProcessor processor)
		{
			StoreId[] array = new StoreId[fetchedItemCount];
			for (int i = 0; i < fetchedItemCount; i++)
			{
				array[i] = (StoreId)batchedItemBuffer[i][0];
			}
			processor.BackOffFromSourceStore();
			AggregateOperationResult aggregateOperationResult = null;
			using (Folder folder = Folder.Bind(sourceMailbox, currentFolderId))
			{
				lock (sourceMailbox)
				{
					if (processor.IsAborted())
					{
						return;
					}
					aggregateOperationResult = folder.DeleteObjects(DeleteItemFlags.HardDelete, array);
				}
			}
			if (aggregateOperationResult.OperationResult == OperationResult.Succeeded)
			{
				SearchMailboxAction.Tracer.TraceDebug<int, string>((long)this.GetHashCode(), "SearchMailboxWoker progressed with {0} message deletes on mailbox {1}", array.Length, processor.GetSourceUserName());
				return;
			}
			string empty = string.Empty;
			if (!DeleteSearchMailboxAction.DeleteItemByItem(batchedItemBuffer, fetchedItemCount, currentFolderId, sourceMailbox, processor, out empty))
			{
				foreach (GroupOperationResult groupOperationResult in aggregateOperationResult.GroupOperationResults)
				{
					if (groupOperationResult.OperationResult != OperationResult.Succeeded && !(groupOperationResult.Exception is RecoverableItemsAccessDeniedException))
					{
						SearchMailboxAction.Tracer.TraceError<OperationResult, LocalizedException>((long)this.GetHashCode(), "DeleteItems operation failed with operation result: {0} and exception: {1}", groupOperationResult.OperationResult, groupOperationResult.Exception);
						SearchMailboxException e = new SearchMailboxException(new LocalizedString(empty), groupOperationResult.Exception);
						processor.ReportActionException(e);
					}
				}
				return;
			}
			SearchMailboxAction.Tracer.TraceDebug<int, string>((long)this.GetHashCode(), "Item by Item delete succeded with {0} message deletes on mailbox {1}", array.Length, processor.GetSourceUserName());
		}

		private static bool DeleteItemByItem(object[][] batchedItemBuffer, int fetchedItemCount, StoreId currentFolderId, MailboxSession sourceMailbox, SearchResultProcessor processor, out string message)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool result = true;
			using (Folder folder = Folder.Bind(sourceMailbox, currentFolderId))
			{
				for (int i = 0; i < fetchedItemCount; i++)
				{
					StoreId storeId = (StoreId)batchedItemBuffer[i][0];
					if (!DeleteSearchMailboxAction.ItemSuccessfullyDeleted(sourceMailbox, storeId))
					{
						lock (sourceMailbox)
						{
							if (processor.IsAborted())
							{
								message = stringBuilder.ToString();
								return result;
							}
							AggregateOperationResult aggregateOperationResult = folder.DeleteObjects(DeleteItemFlags.HardDelete, new StoreId[]
							{
								storeId
							});
							if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
							{
								foreach (GroupOperationResult groupOperationResult in aggregateOperationResult.GroupOperationResults)
								{
									if (groupOperationResult.OperationResult != OperationResult.Succeeded)
									{
										result = false;
										string text = SearchMailboxAction.PropertyExists(batchedItemBuffer[i][3]) ? ((string)batchedItemBuffer[i][3]) : string.Empty;
										bool? flag2 = null;
										if (SearchMailboxAction.PropertyExists(batchedItemBuffer[i][4]))
										{
											flag2 = new bool?((bool)batchedItemBuffer[i][4]);
										}
										ExDateTime? exDateTime = null;
										if (SearchMailboxAction.PropertyExists(batchedItemBuffer[i][5]))
										{
											exDateTime = new ExDateTime?((ExDateTime)batchedItemBuffer[i][5]);
										}
										ExDateTime? exDateTime2 = null;
										if (SearchMailboxAction.PropertyExists(batchedItemBuffer[i][6]))
										{
											exDateTime2 = new ExDateTime?((ExDateTime)batchedItemBuffer[i][6]);
										}
										Participant participant = null;
										if (SearchMailboxAction.PropertyExists(batchedItemBuffer[i][7]))
										{
											participant = (Participant)batchedItemBuffer[i][7];
										}
										string text2 = null;
										if (SearchMailboxAction.PropertyExists(batchedItemBuffer[i][8]))
										{
											text2 = (string)batchedItemBuffer[i][8];
										}
										string text3 = (participant != null) ? participant.DisplayName : string.Empty;
										string text4 = (participant != null) ? participant.EmailAddress : string.Empty;
										string text5 = (folder != null) ? folder.DisplayName : string.Empty;
										stringBuilder.Append(string.Format("\n{0}:\n {1},\"{2}\",\"{3}\",{4},{5},{6},{7},{8},{9}", new object[]
										{
											Strings.DeleteItemFailedForMessage(groupOperationResult.Exception.Message),
											sourceMailbox.MailboxOwner.MailboxInfo.DisplayName,
											text5,
											text,
											flag2,
											exDateTime,
											exDateTime2,
											text3,
											text2 ?? text4,
											((VersionedId)storeId).ObjectId
										}));
									}
								}
							}
						}
					}
				}
			}
			message = stringBuilder.ToString();
			return result;
		}

		private static bool ItemSuccessfullyDeleted(MailboxSession mailbox, StoreId itemId)
		{
			try
			{
				CoreItem coreItem = CoreItem.Bind(mailbox, itemId, new PropertyDefinition[]
				{
					StoreObjectSchema.ParentItemId
				});
				coreItem.Dispose();
			}
			catch (ObjectNotFoundException)
			{
				return true;
			}
			return false;
		}
	}
}
