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
	internal sealed class MoveCopyMessagesSegmentedOperation : SegmentedRopOperation
	{
		internal MoveCopyMessagesSegmentedOperation(ReferenceCount<CoreFolder> sourceFolder, ReferenceCount<CoreFolder> destinationFolder, bool isCopy, StoreObjectId[] storeObjectIds, int segmentSize, TeamMailboxClientOperations teamMailboxClientOperations) : base(RopId.MoveCopyMessages)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.sourceFolder = sourceFolder;
				this.sourceFolder.AddRef();
				this.destinationFolder = destinationFolder;
				this.destinationFolder.AddRef();
				this.isCopy = isCopy;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3903204669U, ref segmentSize);
				this.moveCopyMessagesEnumerator = new IdsSegmentEnumerator(storeObjectIds, segmentSize);
				base.TotalWork = this.moveCopyMessagesEnumerator.Count;
				this.teamMailboxClientOperations = teamMailboxClientOperations;
				disposeGuard.Success();
			}
		}

		protected override SegmentOperationResult InternalDoNextBatchOperation()
		{
			StoreObjectId[] storeObjectIds = this.moveCopyMessagesEnumerator.GetNextBatchIds();
			if (storeObjectIds.Length > 0)
			{
				GroupOperationResult groupOperationResult;
				if (this.teamMailboxClientOperations != null)
				{
					groupOperationResult = TeamMailboxExecutionHelper.RunGroupOperationsWithExecutionLimitHandler(() => this.teamMailboxClientOperations.OnMoveCopyMessages(this.sourceFolder.ReferencedObject, this.destinationFolder.ReferencedObject, storeObjectIds, this.isCopy), "TeamMailboxClientOperations.OnMoveCopyMessages");
				}
				else if (this.isCopy)
				{
					groupOperationResult = this.sourceFolder.ReferencedObject.CopyItems(this.destinationFolder.ReferencedObject, storeObjectIds, null, null, null);
					this.PostCopyMessages(ref groupOperationResult);
				}
				else
				{
					groupOperationResult = this.sourceFolder.ReferencedObject.MoveItems(this.destinationFolder.ReferencedObject, storeObjectIds, null, null, null);
				}
				return new SegmentOperationResult
				{
					CompletedWork = storeObjectIds.Length,
					OperationResult = groupOperationResult.OperationResult,
					Exception = groupOperationResult.Exception,
					IsCompleted = (storeObjectIds.Length == 0)
				};
			}
			if (this.teamMailboxClientOperations != null)
			{
				((MailboxSession)this.destinationFolder.ReferencedObject.Session).TryToSyncSiteMailboxNow();
			}
			return SegmentedRopOperation.FinalResult;
		}

		internal override RopResult CreateCompleteResult(object progressToken, IProgressResultFactory resultFactory)
		{
			if (base.ErrorCode == ErrorCode.None)
			{
				return ((MoveCopyMessagesResultFactory)resultFactory).CreateSuccessfulResult(base.IsPartiallyCompleted);
			}
			return ((MoveCopyMessagesResultFactory)resultFactory).CreateFailedResult(base.ErrorCode, base.IsPartiallyCompleted);
		}

		internal override RopResult CreateCompleteResultForProgress(object progressToken, ProgressResultFactory progressResultFactory)
		{
			if (base.ErrorCode == ErrorCode.None)
			{
				return progressResultFactory.CreateSuccessfulMoveCopyMessagesResult(progressToken, base.IsPartiallyCompleted);
			}
			return progressResultFactory.CreateFailedMoveCopyMessagesResult(progressToken, base.ErrorCode, base.IsPartiallyCompleted);
		}

		private void PostCopyMessages(ref GroupOperationResult result)
		{
			if (this.sourceFolder.ReferencedObject.Id.Equals(this.destinationFolder.ReferencedObject.Id) && result.OperationResult == OperationResult.PartiallySucceeded && result.Exception is PartialCompletionException && result.Exception.InnerException is MapiExceptionPartialCompletion)
			{
				StoreSession session = this.sourceFolder.ReferencedObject.Session;
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
						storeObjectId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId(this.sourceFolder.ReferencedObject);
					}
				}
				if (storeObjectId != null && !this.sourceFolder.ReferencedObject.Id.ObjectId.Equals(storeObjectId))
				{
					StoreObjectId[] sourceItemIds = SegmentedRopOperation.ConvertMessageIds(session.IdConverter, storeObjectId, result.ObjectIds);
					using (CoreFolder coreFolder = CoreFolder.Bind(session, storeObjectId))
					{
						ExTraceGlobals.FailedRopTracer.TraceDebug<int>((long)this.GetHashCode(), "Retry CopyItems from Dumpster with {0} items", result.ObjectIds.Count);
						GroupOperationResult groupOperationResult = coreFolder.CopyItems(this.destinationFolder.ReferencedObject, sourceItemIds, null, null, null);
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
			if (this.sourceFolder != null)
			{
				this.sourceFolder.Release();
			}
			if (this.destinationFolder != null)
			{
				this.destinationFolder.Release();
			}
			Util.DisposeIfPresent(this.moveCopyMessagesEnumerator);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MoveCopyMessagesSegmentedOperation>(this);
		}

		private readonly TeamMailboxClientOperations teamMailboxClientOperations;

		private readonly ReferenceCount<CoreFolder> sourceFolder;

		private readonly ReferenceCount<CoreFolder> destinationFolder;

		private readonly bool isCopy;

		private readonly IdsSegmentEnumerator moveCopyMessagesEnumerator;
	}
}
