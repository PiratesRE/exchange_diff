using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CopyFolderSegmentedOperation : SegmentedRopOperation
	{
		internal CopyFolderSegmentedOperation(ReferenceCount<CoreFolder> sourcefolderReferenceCount, ReferenceCount<CoreFolder> destinationFolderReferenceCount, StoreObjectId sourceFolderId, string newFolderName, bool isRecursive) : base(RopId.CopyFolder)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.sourceRootContainerFolder = sourcefolderReferenceCount;
				this.sourceRootContainerFolder.AddRef();
				this.destinationRootContainerFolder = destinationFolderReferenceCount;
				this.destinationRootContainerFolder.AddRef();
				this.newFolderName = newFolderName;
				this.isRecursive = isRecursive;
				this.sourceRootFolder = CoreFolder.Bind(this.sourceRootContainerFolder.ReferencedObject.Session, sourceFolderId, new PropertyDefinition[]
				{
					FolderSchema.DisplayName
				});
				List<StoreObjectId> list = new List<StoreObjectId>();
				base.TotalWork = SegmentedRopOperation.EstimateWork(this.sourceRootFolder, true, list);
				if (base.RopId == RopId.CopyFolder)
				{
					base.TotalWork -= list.Count;
				}
				base.DetectCopyMoveLoop(destinationFolderReferenceCount, list);
				this.copyFolderEnumerator = this.InternalCopyFolder(this.sourceRootContainerFolder.ReferencedObject, this.destinationRootContainerFolder.ReferencedObject, sourceFolderId, true).GetEnumerator();
				disposeGuard.Success();
			}
		}

		private static CoreFolder CreateDuplicateFolder(CoreFolder sourceFolder, CoreFolder destinationContainerFolder, string folderName)
		{
			CoreFolder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				TestInterceptor.Intercept(TestInterceptorLocation.CopyFolderSegmentedOperation_CoreFolderDeletedCreatingNewSubFolder, new object[]
				{
					destinationContainerFolder,
					destinationContainerFolder.Id
				});
				if (folderName == string.Empty)
				{
					folderName = (string)sourceFolder.PropertyBag[FolderSchema.DisplayName];
				}
				CoreFolder coreFolder = CoreFolder.Create(destinationContainerFolder.Session, destinationContainerFolder.Id, false, folderName, CreateMode.CreateNew);
				disposeGuard.Add<CoreFolder>(coreFolder);
				FolderSaveResult folderSaveResult = coreFolder.Save(SaveMode.FailOnAnyConflict);
				if (folderSaveResult.OperationResult != OperationResult.Succeeded)
				{
					throw folderSaveResult.ToException(new LocalizedString("Cannot save the folder of displayName " + folderName));
				}
				coreFolder.PropertyBag.Load(null);
				PropertyError[] array = sourceFolder.CopyFolder(coreFolder, CopyPropertiesFlags.None, CopySubObjects.DoNotCopy, new NativeStorePropertyDefinition[]
				{
					(NativeStorePropertyDefinition)FolderSchema.DisplayName
				});
				if (array.Length > 0)
				{
					throw new RopExecutionException(new LocalizedString(string.Format("Cannot copy folder properties. Errors = {0:!E}", array)), (ErrorCode)2147500037U);
				}
				coreFolder.PropertyBag.Load(null);
				disposeGuard.Success();
				result = coreFolder;
			}
			return result;
		}

		[Conditional("DEBUG")]
		private static void DebugCheckFolderType(CoreFolder folder, bool expectSearchFolder)
		{
			VersionedId versionedId = (VersionedId)folder.PropertyBag[FolderSchema.Id];
		}

		protected override SegmentOperationResult InternalDoNextBatchOperation()
		{
			if (this.copyFolderEnumerator.MoveNext())
			{
				return this.copyFolderEnumerator.Current;
			}
			return SegmentedRopOperation.FinalResult;
		}

		private IEnumerable<SegmentOperationResult> InternalCopyFolder(CoreFolder sourceContainerFolder, CoreFolder destinationContainerFolder, StoreObjectId sourceFolderId, bool isTopLevel)
		{
			CoreFolder sourceFolder = null;
			bool processedSearchFolder = false;
			GroupOperationResult result = null;
			TestInterceptor.Intercept(TestInterceptorLocation.CopyFolderSegmentedOperation_CoreFolderDeletedAboutToCreateNewSubFolder, new object[]
			{
				sourceContainerFolder,
				sourceFolderId
			});
			if (!base.SafeSegmentExecution(delegate()
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					if (isTopLevel)
					{
						sourceFolder = this.sourceRootFolder;
						this.sourceRootFolder = null;
					}
					else
					{
						sourceFolder = CoreFolder.Bind(sourceContainerFolder.Session, sourceFolderId);
					}
					disposeGuard.Add<CoreFolder>(sourceFolder);
					if (sourceFolder.Id.ObjectId.ObjectType == StoreObjectType.SearchFolder || sourceFolder.Id.ObjectId.ObjectType == StoreObjectType.OutlookSearchFolder)
					{
						processedSearchFolder = true;
						result = this.ProcessSearchFolder(sourceFolder, destinationContainerFolder, this.isRecursive ? CopySubObjects.Copy : CopySubObjects.DoNotCopy, sourceFolderId);
					}
					disposeGuard.Success();
				}
			}))
			{
				yield return SegmentedRopOperation.FailedSegmentResult;
			}
			else
			{
				using (sourceFolder)
				{
					if (processedSearchFolder)
					{
						yield return new SegmentOperationResult
						{
							IsCompleted = false,
							CompletedWork = 1,
							OperationResult = result.OperationResult,
							Exception = result.Exception
						};
						yield break;
					}
					CoreFolder destinationFolder = null;
					if (!base.SafeSegmentExecution(delegate()
					{
						destinationFolder = CopyFolderSegmentedOperation.CreateDuplicateFolder(sourceFolder, destinationContainerFolder, isTopLevel ? this.newFolderName : string.Empty);
					}))
					{
						yield return SegmentedRopOperation.FailedSegmentResult;
						yield break;
					}
					using (destinationFolder)
					{
						foreach (SegmentOperationResult copyMessages in this.CopyContents(sourceFolder, destinationFolder))
						{
							yield return copyMessages;
						}
						if (!this.isRecursive)
						{
							yield break;
						}
						TestInterceptor.Intercept(TestInterceptorLocation.CopyFolderSegmentedOperation_CoreFolderDeletedAboutToQueryingSubFolders, new object[]
						{
							sourceFolder,
							sourceFolder.Id
						});
						foreach (SegmentOperationResult copySubfolders in this.CopySubfolders(sourceFolder, destinationFolder))
						{
							yield return copySubfolders;
						}
					}
				}
			}
			yield break;
		}

		private IEnumerable<SegmentOperationResult> CopyContents(CoreFolder sourceFolder, CoreFolder destinationFolder)
		{
			foreach (SegmentOperationResult copyMessages in this.CopyMessages(sourceFolder, destinationFolder, ItemQueryType.None))
			{
				yield return copyMessages;
			}
			foreach (SegmentOperationResult copyMessages2 in this.CopyMessages(sourceFolder, destinationFolder, ItemQueryType.Associated))
			{
				yield return copyMessages2;
			}
			yield break;
		}

		private IEnumerable<SegmentOperationResult> CopySubfolders(CoreFolder sourceFolder, CoreFolder destinationFolder)
		{
			CopyFolderSegmentedOperation.<>c__DisplayClass23 CS$<>8__locals1 = new CopyFolderSegmentedOperation.<>c__DisplayClass23();
			CS$<>8__locals1.sourceFolder = sourceFolder;
			CS$<>8__locals1.sourcefolderQuery = null;
			if (!base.SafeSegmentExecution(delegate()
			{
				CS$<>8__locals1.sourcefolderQuery = new QuerySegmentEnumerator(CS$<>8__locals1.sourceFolder, FolderQueryFlags.None, 10);
			}))
			{
				yield return SegmentedRopOperation.FailedSegmentResult;
				yield break;
			}
			QuerySegmentEnumerator sourcefolderQuery = CS$<>8__locals1.sourcefolderQuery;
			StoreObjectId[] sourceSubfolderIds = null;
			for (;;)
			{
				TestInterceptor.Intercept(TestInterceptorLocation.CopyFolderSegmentedOperation_CoreFolderDeletedAboutToPeruseSubFolders, new object[]
				{
					CS$<>8__locals1.sourceFolder,
					CS$<>8__locals1.sourceFolder.Id
				});
				if (!base.SafeSegmentExecution(delegate()
				{
					sourceSubfolderIds = CS$<>8__locals1.sourcefolderQuery.GetNextBatchIds();
				}))
				{
					break;
				}
				if (sourceSubfolderIds.Length == 0)
				{
					goto Block_4;
				}
				foreach (StoreObjectId sourceSubfolderId in sourceSubfolderIds)
				{
					TestInterceptor.Intercept(TestInterceptorLocation.CopyFolderSegmentedOperation_CoreFolderDeletedAboutToCopySubFolder, new object[]
					{
						CS$<>8__locals1.sourceFolder,
						CS$<>8__locals1.sourceFolder.Id
					});
					foreach (SegmentOperationResult copySubfolderResult in this.InternalCopyFolder(CS$<>8__locals1.sourceFolder, destinationFolder, sourceSubfolderId, false))
					{
						yield return copySubfolderResult;
					}
				}
			}
			yield return SegmentedRopOperation.FailedSegmentResult;
			yield break;
			Block_4:
			yield break;
		}

		private IEnumerable<SegmentOperationResult> CopyMessages(CoreFolder sourceFolder, CoreFolder destinationFolder, ItemQueryType itemQueryType)
		{
			using (QuerySegmentEnumerator copyMessages = new QuerySegmentEnumerator(sourceFolder, itemQueryType, SegmentEnumerator.MessageSegmentSize))
			{
				TestInterceptor.Intercept(TestInterceptorLocation.CopyFolderSegmentedOperation_CoreFolderDeletedWhenAboutToCopyMessages, new object[]
				{
					sourceFolder,
					sourceFolder.Id
				});
				for (;;)
				{
					StoreObjectId[] messageIds = null;
					if (!base.SafeSegmentExecution(delegate()
					{
						messageIds = copyMessages.GetNextBatchIds();
					}))
					{
						yield return SegmentedRopOperation.FailedSegmentResult;
					}
					if (messageIds == null || messageIds.Length <= 0)
					{
						break;
					}
					GroupOperationResult groupOperationResult = null;
					if (!base.SafeSegmentExecution(delegate()
					{
						TestInterceptor.Intercept(TestInterceptorLocation.CopyFolderSegmentedOperation_CoreFolderDeletedWhenDoingCopyMessages, new object[]
						{
							destinationFolder,
							destinationFolder.Id
						});
						groupOperationResult = sourceFolder.CopyItems(destinationFolder, messageIds, null, null, null);
					}))
					{
						yield return SegmentedRopOperation.FailedSegmentResult;
					}
					else
					{
						yield return new SegmentOperationResult
						{
							OperationResult = groupOperationResult.OperationResult,
							Exception = groupOperationResult.Exception,
							CompletedWork = messageIds.Length,
							IsCompleted = false
						};
					}
					TestInterceptor.Intercept(TestInterceptorLocation.CopyFolderSegmentedOperation_CoreFolderDeletedWhenDoingNextCopyMessages, new object[]
					{
						sourceFolder,
						sourceFolder.Id
					});
				}
			}
			yield break;
		}

		private GroupOperationResult ProcessSearchFolder(CoreFolder sourceFolder, CoreFolder destinationFolder, CopySubObjects copySubObjects, StoreObjectId sourceFolderId)
		{
			return sourceFolder.CopyFolder(destinationFolder, copySubObjects, sourceFolderId);
		}

		internal override RopResult CreateCompleteResult(object progressToken, IProgressResultFactory resultFactory)
		{
			if (base.ErrorCode == ErrorCode.None)
			{
				return ((CopyFolderResultFactory)resultFactory).CreateSuccessfulResult(base.IsPartiallyCompleted);
			}
			return ((CopyFolderResultFactory)resultFactory).CreateFailedResult(base.ErrorCode, base.IsPartiallyCompleted);
		}

		internal override RopResult CreateCompleteResultForProgress(object progressToken, ProgressResultFactory progressResultFactory)
		{
			if (base.ErrorCode == ErrorCode.None)
			{
				return progressResultFactory.CreateSuccessfulCopyFolderResult(progressToken, base.IsPartiallyCompleted);
			}
			return progressResultFactory.CreateFailedCopyFolderResult(progressToken, base.ErrorCode, base.IsPartiallyCompleted);
		}

		protected override void InternalDispose()
		{
			if (this.sourceRootContainerFolder != null)
			{
				this.sourceRootContainerFolder.Release();
			}
			if (this.destinationRootContainerFolder != null)
			{
				this.destinationRootContainerFolder.Release();
			}
			Util.DisposeIfPresent(this.sourceRootFolder);
			Util.DisposeIfPresent(this.copyFolderEnumerator);
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CopyFolderSegmentedOperation>(this);
		}

		private readonly ReferenceCount<CoreFolder> sourceRootContainerFolder;

		private readonly ReferenceCount<CoreFolder> destinationRootContainerFolder;

		private readonly bool isRecursive;

		private readonly string newFolderName;

		private readonly IEnumerator<SegmentOperationResult> copyFolderEnumerator;

		private CoreFolder sourceRootFolder;
	}
}
