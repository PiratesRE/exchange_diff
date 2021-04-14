using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MoveFolderSegmentedOperation : SegmentedRopOperation
	{
		internal MoveFolderSegmentedOperation(ReferenceCount<CoreFolder> sourcefolderReferenceCount, ReferenceCount<CoreFolder> destinationFolderReferenceCount, Logon logon, StoreObjectId sourceFolderId, string newFolderName) : base(RopId.MoveFolder)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.sourceRootContainerFolder = sourcefolderReferenceCount;
				this.sourceRootContainerFolder.AddRef();
				this.destinationRootContainerFolder = destinationFolderReferenceCount;
				this.destinationRootContainerFolder.AddRef();
				this.logon = logon;
				this.newFolderName = newFolderName;
				this.sourceRootFolderId = sourceFolderId;
				List<StoreObjectId> list = new List<StoreObjectId>();
				using (CoreFolder coreFolder = CoreFolder.Bind(this.sourceRootContainerFolder.ReferencedObject.Session, sourceFolderId, new PropertyDefinition[]
				{
					FolderSchema.DisplayName
				}))
				{
					base.TotalWork = SegmentedRopOperation.EstimateWork(coreFolder, true, list);
				}
				base.TotalWork = 1;
				base.DetectCopyMoveLoop(destinationFolderReferenceCount, list);
				disposeGuard.Success();
			}
		}

		protected override SegmentOperationResult InternalDoNextBatchOperation()
		{
			GroupOperationResult result = null;
			if (base.SafeSegmentExecution(delegate()
			{
				string folderName = (this.newFolderName == string.Empty) ? null : this.newFolderName;
				PublicLogon publicLogon = this.logon as PublicLogon;
				if (publicLogon != null && !publicLogon.IsPrimaryHierarchyLogon)
				{
					result = PublicFolderOperations.MoveFolder(publicLogon, this.sourceRootContainerFolder.ReferencedObject.Id, this.destinationRootContainerFolder.ReferencedObject.Id, this.sourceRootFolderId, folderName);
					return;
				}
				result = this.sourceRootContainerFolder.ReferencedObject.MoveFolder(this.destinationRootContainerFolder.ReferencedObject, this.sourceRootFolderId, folderName);
			}))
			{
				return new SegmentOperationResult
				{
					CompletedWork = 1,
					Exception = result.Exception,
					IsCompleted = true,
					OperationResult = result.OperationResult
				};
			}
			return new SegmentOperationResult
			{
				CompletedWork = 0,
				Exception = null,
				IsCompleted = true,
				OperationResult = OperationResult.Failed
			};
		}

		internal override RopResult CreateCompleteResult(object progressToken, IProgressResultFactory resultFactory)
		{
			if (base.ErrorCode == ErrorCode.None)
			{
				return ((MoveFolderResultFactory)resultFactory).CreateSuccessfulResult(base.IsPartiallyCompleted);
			}
			return ((MoveFolderResultFactory)resultFactory).CreateFailedResult(base.ErrorCode, base.IsPartiallyCompleted);
		}

		internal override RopResult CreateCompleteResultForProgress(object progressToken, ProgressResultFactory progressResultFactory)
		{
			if (base.ErrorCode == ErrorCode.None)
			{
				return progressResultFactory.CreateSuccessfulMoveFolderResult(progressToken, base.IsPartiallyCompleted);
			}
			return progressResultFactory.CreateFailedMoveFolderResult(progressToken, base.ErrorCode, base.IsPartiallyCompleted);
		}

		protected override void InternalDispose()
		{
			this.sourceRootContainerFolder.Release();
			this.destinationRootContainerFolder.Release();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MoveFolderSegmentedOperation>(this);
		}

		private readonly ReferenceCount<CoreFolder> sourceRootContainerFolder;

		private readonly ReferenceCount<CoreFolder> destinationRootContainerFolder;

		private readonly Logon logon;

		private readonly StoreObjectId sourceRootFolderId;

		private string newFolderName;
	}
}
