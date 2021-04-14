using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class EmptyFolderSegmentedOperation : SegmentedRopOperation
	{
		protected EmptyFolderSegmentedOperation(ReferenceCount<CoreFolder> folder, EmptyFolderFlags emptyFolderFlags) : base(((emptyFolderFlags & EmptyFolderFlags.HardDelete) == EmptyFolderFlags.HardDelete) ? RopId.HardEmptyFolder : RopId.EmptyFolder)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.folder = folder;
				this.folder.AddRef();
				this.emptyFolderFlags = emptyFolderFlags;
				this.deleteAssociated = ((emptyFolderFlags & EmptyFolderFlags.DeleteAssociatedMessages) == EmptyFolderFlags.DeleteAssociatedMessages);
				this.subFolderIds = new List<StoreObjectId>();
				this.subFolderIds.Add(this.folder.ReferencedObject.Id.ObjectId);
				base.TotalWork = SegmentedRopOperation.EstimateWork(this.folder.ReferencedObject, this.deleteAssociated, this.subFolderIds);
				this.subFolderIds.Reverse();
				this.emptyFolderEnumerator = this.InternalEmptyFolder().GetEnumerator();
				disposeGuard.Success();
			}
		}

		protected override SegmentOperationResult InternalDoNextBatchOperation()
		{
			if (this.emptyFolderEnumerator.MoveNext())
			{
				return this.emptyFolderEnumerator.Current;
			}
			return SegmentedRopOperation.FinalResult;
		}

		private IEnumerable<SegmentOperationResult> InternalEmptyFolder()
		{
			using (List<StoreObjectId>.Enumerator enumerator = this.subFolderIds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EmptyFolderSegmentedOperation.<>c__DisplayClass5 CS$<>8__locals2 = new EmptyFolderSegmentedOperation.<>c__DisplayClass5();
					CS$<>8__locals2.subFolderId = enumerator.Current;
					TestInterceptor.Intercept(TestInterceptorLocation.EmptyFolderSegmentedOperation_CoreFolderBind, new object[]
					{
						this.folder.ReferencedObject.Session,
						CS$<>8__locals2.subFolderId
					});
					CoreFolder subFolder = null;
					if (base.SafeSegmentExecution(delegate()
					{
						subFolder = CoreFolder.Bind(this.folder.ReferencedObject.Session, CS$<>8__locals2.subFolderId);
					}))
					{
						using (subFolder)
						{
							if (!RopHandler.IsSearchFolder(subFolder.Id))
							{
								goto IL_168;
							}
						}
						continue;
						IL_168:
						foreach (SegmentOperationResult segmentOperationResult in this.DeleteContents(subFolder))
						{
							yield return segmentOperationResult;
						}
					}
					else
					{
						yield return SegmentedRopOperation.FailedSegmentResult;
					}
				}
			}
			foreach (SegmentOperationResult segmentOperationResult2 in this.DeleteContents(this.folder.ReferencedObject))
			{
				yield return segmentOperationResult2;
			}
			GroupOperationResult emptyFolderOperationResult = null;
			if (base.SafeSegmentExecution(delegate()
			{
				TestInterceptor.Intercept(TestInterceptorLocation.EmptyFolderSegmentedOperation_EmptyFolderHierarchy, new object[0]);
				emptyFolderOperationResult = this.folder.ReferencedObject.EmptyFolder(false, this.emptyFolderFlags);
			}))
			{
				yield return new SegmentOperationResult
				{
					CompletedWork = this.subFolderIds.Count,
					OperationResult = emptyFolderOperationResult.OperationResult,
					Exception = emptyFolderOperationResult.Exception,
					IsCompleted = true
				};
			}
			else
			{
				yield return SegmentedRopOperation.FailedSegmentResult;
			}
			yield break;
		}

		private IEnumerable<SegmentOperationResult> DeleteContents(CoreFolder subFolder)
		{
			foreach (SegmentOperationResult deletedMessages in this.DeleteMessages(subFolder, ItemQueryType.None))
			{
				yield return deletedMessages;
			}
			if (this.deleteAssociated)
			{
				foreach (SegmentOperationResult deletedMessages2 in this.DeleteMessages(subFolder, ItemQueryType.Associated))
				{
					yield return deletedMessages2;
				}
			}
			yield break;
		}

		private IEnumerable<SegmentOperationResult> DeleteMessages(CoreFolder coreFolder, ItemQueryType itemQueryType)
		{
			DeleteItemFlags deleteItemFlags = ((this.emptyFolderFlags & EmptyFolderFlags.HardDelete) == EmptyFolderFlags.HardDelete) ? DeleteItemFlags.HardDelete : DeleteItemFlags.SoftDelete;
			using (QuerySegmentEnumerator deleteMessages = new QuerySegmentEnumerator(coreFolder, itemQueryType, SegmentEnumerator.MessageSegmentSize))
			{
				TestInterceptor.Intercept(TestInterceptorLocation.EmptyFolderSegmentedOperation_CoreFolderQuery, new object[]
				{
					coreFolder
				});
				StoreObjectId[] messageIds = null;
				if (!base.SafeSegmentExecution(delegate()
				{
					messageIds = deleteMessages.GetNextBatchIds();
				}))
				{
					yield return SegmentedRopOperation.FailedSegmentResult;
					yield break;
				}
				while (messageIds.Length > 0)
				{
					TestInterceptor.Intercept(TestInterceptorLocation.EmptyFolderSegmentedOperation_CoreFolderDeleteMessages, new object[]
					{
						coreFolder
					});
					GroupOperationResult groupOperationResult = null;
					if (!base.SafeSegmentExecution(delegate()
					{
						groupOperationResult = coreFolder.DeleteItems(deleteItemFlags, messageIds);
					}))
					{
						yield return SegmentedRopOperation.FailedSegmentResult;
						break;
					}
					yield return new SegmentOperationResult
					{
						OperationResult = groupOperationResult.OperationResult,
						CompletedWork = messageIds.Length,
						Exception = groupOperationResult.Exception,
						IsCompleted = false
					};
					if (!base.SafeSegmentExecution(delegate()
					{
						messageIds = deleteMessages.GetNextBatchIds();
					}))
					{
						yield return SegmentedRopOperation.FailedSegmentResult;
						break;
					}
				}
			}
			yield break;
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.emptyFolderEnumerator);
			if (this.folder != null)
			{
				this.folder.Release();
			}
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EmptyFolderSegmentedOperation>(this);
		}

		private readonly ReferenceCount<CoreFolder> folder;

		private readonly EmptyFolderFlags emptyFolderFlags;

		private readonly bool deleteAssociated;

		private readonly IEnumerator<SegmentOperationResult> emptyFolderEnumerator;

		private readonly List<StoreObjectId> subFolderIds;
	}
}
