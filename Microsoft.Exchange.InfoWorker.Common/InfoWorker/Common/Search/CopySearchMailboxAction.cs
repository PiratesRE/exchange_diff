using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class CopySearchMailboxAction : SearchMailboxAction
	{
		public override SearchMailboxAction Clone()
		{
			return new CopySearchMailboxAction();
		}

		public override void PerformBatchOperation(object[][] batchedItemBuffer, int fetchedItemCount, StoreId currentFolderId, MailboxSession sourceMailbox, MailboxSession targetMailbox, Dictionary<StoreId, FolderNode> folderNodeMap, SearchResultProcessor processor)
		{
			processor.CheckTargetMailboxAvailableSpace();
			StoreId[] array = new StoreId[fetchedItemCount];
			for (int i = 0; i < fetchedItemCount; i++)
			{
				array[i] = (StoreId)batchedItemBuffer[i][0];
			}
			FolderNode folderNode = folderNodeMap[currentFolderId];
			using (Folder folder = Folder.Bind(sourceMailbox, currentFolderId))
			{
				processor.BackOffFromSourceStore();
				processor.BackOffFromTargetStore();
				try
				{
					GroupOperationResult groupOperationResult = null;
					lock (targetMailbox)
					{
						if (processor.IsAborted())
						{
							return;
						}
						if (folderNode.TargetFolderId == null)
						{
							this.CreateTargetFolder(targetMailbox, folderNode);
						}
						groupOperationResult = folder.CopyItems(targetMailbox, folderNode.TargetFolderId, false, array);
					}
					if (groupOperationResult.OperationResult != OperationResult.Succeeded)
					{
						SearchMailboxAction.Tracer.TraceError<OperationResult, LocalizedException>((long)this.GetHashCode(), "DeleteItems operation failed with operation result: {0} and exception: {1}", groupOperationResult.OperationResult, groupOperationResult.Exception);
						if (groupOperationResult.Exception.GetType() == typeof(MapiExceptionPartialCompletion) || groupOperationResult.Exception.GetType() == typeof(PartialCompletionException))
						{
							StoragePermanentException e = new StoragePermanentException(Strings.CopyItemsFailed, groupOperationResult.Exception);
							processor.ReportActionException(e);
						}
						else
						{
							SearchMailboxException e2 = new SearchMailboxException(Strings.CopyItemsFailed, groupOperationResult.Exception);
							processor.ReportActionException(e2);
						}
					}
					else
					{
						SearchMailboxAction.Tracer.TraceDebug<int, string>((long)this.GetHashCode(), "SearchMailboxWoker progressed with {0} message deletes on mailbox {1}", array.Length, processor.GetSourceUserName());
					}
				}
				catch (ArgumentException ex)
				{
					SearchMailboxAction.Tracer.TraceError<ArgumentException>((long)this.GetHashCode(), "CopyItems operation failed due to item validation failed, exception: {0}", ex);
					SearchMailboxException e3 = new SearchMailboxException(Strings.CopyItemsFailed, ex);
					processor.ReportActionException(e3);
				}
				catch (MapiExceptionPartialCompletion mapiExceptionPartialCompletion)
				{
					SearchMailboxAction.Tracer.TraceError<MapiExceptionPartialCompletion>((long)this.GetHashCode(), "CopyItems operation failed due to insufficient space in target mailbox, exception: {0}", mapiExceptionPartialCompletion);
					StoragePermanentException e4 = new StoragePermanentException(Strings.CopyItemsFailed, mapiExceptionPartialCompletion);
					processor.ReportActionException(e4);
				}
				catch (PartialCompletionException ex2)
				{
					SearchMailboxAction.Tracer.TraceError<PartialCompletionException>((long)this.GetHashCode(), "CopyItems operation failed due to insufficient space in target mailbox, exception: {0}", ex2);
					StoragePermanentException e5 = new StoragePermanentException(Strings.CopyItemsFailed, ex2);
					processor.ReportActionException(e5);
				}
			}
		}

		private StoreId CreateTargetFolder(MailboxSession targetMailbox, FolderNode folderNode)
		{
			Stack<FolderNode> stack = new Stack<FolderNode>();
			FolderNode folderNode2 = folderNode;
			while (folderNode2.TargetFolderId == null)
			{
				stack.Push(folderNode2);
				folderNode2 = folderNode2.Parent;
			}
			FolderNode folderNode3 = folderNode2;
			while (stack.Count > 0)
			{
				bool sourceBasedFolder = folderNode.SourceFolderId != null;
				folderNode2 = stack.Pop();
				folderNode2.TargetFolderId = this.CreateFolder(targetMailbox, folderNode3.TargetFolderId, folderNode2.DisplayName, sourceBasedFolder);
				folderNode3 = folderNode2;
			}
			return folderNode.TargetFolderId;
		}

		private StoreId CreateFolder(MailboxSession mailboxSession, StoreId parentId, string displayName, bool sourceBasedFolder)
		{
			StoreId result = null;
			using (Folder folder = Folder.Create(mailboxSession, parentId, StoreObjectType.Folder, displayName, CreateMode.OpenIfExists))
			{
				if (sourceBasedFolder)
				{
					folder[FolderSchema.OwaViewStateSortColumn] = "DeliveryTime";
				}
				FolderSaveResult folderSaveResult = folder.Save();
				if (folderSaveResult.OperationResult != OperationResult.Succeeded)
				{
					SearchMailboxAction.Tracer.TraceError<string, FolderSaveResult>((long)this.GetHashCode(), "Folder.Save operation failed on mailbox {0} with operation result {1} ", mailboxSession.MailboxOwner.MailboxInfo.DisplayName, folderSaveResult);
					throw folderSaveResult.ToException(Strings.CreateFolderFailed(displayName));
				}
				folder.Load();
				result = folder.Id;
			}
			return result;
		}
	}
}
