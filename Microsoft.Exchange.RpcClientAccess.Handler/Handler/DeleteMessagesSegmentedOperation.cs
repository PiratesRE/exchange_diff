using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Mapi;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class DeleteMessagesSegmentedOperation : SegmentedRopOperation
	{
		protected DeleteMessagesSegmentedOperation(ReferenceCount<CoreFolder> folder, DeleteItemFlags deleteItemFlags, StoreObjectId[] storeObjectIds, int segmentSize, TeamMailboxClientOperations teamMailboxClientOperations) : base(((deleteItemFlags & DeleteItemFlags.HardDelete) == DeleteItemFlags.HardDelete) ? RopId.HardDeleteMessages : RopId.DeleteMessages)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.folder = folder;
				this.folder.AddRef();
				this.deleteItemFlags = deleteItemFlags;
				this.deleteMessagesEnumerator = new IdsSegmentEnumerator(storeObjectIds, segmentSize);
				base.TotalWork = this.deleteMessagesEnumerator.Count;
				this.teamMailboxClientOperations = teamMailboxClientOperations;
				disposeGuard.Success();
			}
		}

		protected override SegmentOperationResult InternalDoNextBatchOperation()
		{
			StoreObjectId[] ids = this.deleteMessagesEnumerator.GetNextBatchIds();
			if (ids.Length > 0)
			{
				GroupOperationResult groupOperationResult;
				if (this.teamMailboxClientOperations != null)
				{
					groupOperationResult = TeamMailboxExecutionHelper.RunGroupOperationsWithExecutionLimitHandler(() => this.teamMailboxClientOperations.OnDeleteMessages(this.folder.ReferencedObject, ids), "TeamMailboxClientOperations.OnDeleteMessages");
				}
				else
				{
					groupOperationResult = this.folder.ReferencedObject.Session.Delete(this.deleteItemFlags, ids).GroupOperationResults[0];
					this.PostDeleteMessages(ref groupOperationResult);
				}
				TestInterceptor.InterceptValue<GroupOperationResult>(TestInterceptorLocation.DeleteMessagesSegmentedOperation_InternalDoNextBatchOperation, ref groupOperationResult);
				return new SegmentOperationResult
				{
					CompletedWork = ids.Length,
					OperationResult = groupOperationResult.OperationResult,
					Exception = groupOperationResult.Exception,
					IsCompleted = false
				};
			}
			if (this.teamMailboxClientOperations != null)
			{
				((MailboxSession)this.folder.ReferencedObject.Session).TryToSyncSiteMailboxNow();
			}
			return SegmentedRopOperation.FinalResult;
		}

		private void PostDeleteMessages(ref GroupOperationResult result)
		{
			if ((this.deleteItemFlags & DeleteItemFlags.HardDelete) == DeleteItemFlags.HardDelete && result.OperationResult == OperationResult.PartiallySucceeded && result.Exception is PartialCompletionException && result.Exception.InnerException is MapiExceptionPartialCompletion)
			{
				StoreSession session = this.folder.ReferencedObject.Session;
				StoreObjectId storeObjectId = null;
				MailboxSession mailboxSession = session as MailboxSession;
				if (mailboxSession != null)
				{
					storeObjectId = mailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions);
				}
				else
				{
					PublicFolderSession publicFolderSession = session as PublicFolderSession;
					if (publicFolderSession != null)
					{
						storeObjectId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId(this.folder.ReferencedObject);
					}
				}
				if (storeObjectId != null && !this.folder.ReferencedObject.Id.ObjectId.Equals(storeObjectId))
				{
					ExTraceGlobals.FailedRopTracer.TraceDebug<int>((long)this.GetHashCode(), "Retry HardDelete from Dumpster with {0} items", result.ObjectIds.Count);
					StoreObjectId[] ids = SegmentedRopOperation.ConvertMessageIds(session.IdConverter, storeObjectId, result.ObjectIds);
					using (CoreFolder coreFolder = CoreFolder.Bind(session, storeObjectId))
					{
						GroupOperationResult groupOperationResult = coreFolder.Session.Delete(this.deleteItemFlags, ids).GroupOperationResults[0];
						if (groupOperationResult.OperationResult != OperationResult.Failed)
						{
							result = groupOperationResult;
						}
					}
				}
			}
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.deleteMessagesEnumerator);
			if (this.folder != null)
			{
				this.folder.Release();
			}
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DeleteMessagesSegmentedOperation>(this);
		}

		private readonly ReferenceCount<CoreFolder> folder;

		private readonly DeleteItemFlags deleteItemFlags;

		private readonly IdsSegmentEnumerator deleteMessagesEnumerator;

		private readonly TeamMailboxClientOperations teamMailboxClientOperations;
	}
}
