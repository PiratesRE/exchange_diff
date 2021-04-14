using System;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SetReadFlagsSegmentedOperation : SegmentedRopOperation
	{
		internal SetReadFlagsSegmentedOperation(ReferenceCount<CoreFolder> folder, SetReadFlagFlags setReadFlagFlags, StoreObjectId[] messageIds, int segmentSize) : base(RopId.SetReadFlags)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.folder = folder;
				this.folder.AddRef();
				this.setReadFlagFlags = setReadFlagFlags;
				if (messageIds != null)
				{
					this.setReadFlagsEnumerator = new IdsSegmentEnumerator(messageIds, segmentSize);
					base.TotalWork = messageIds.Length;
				}
				else
				{
					this.setReadFlagsEnumerator = new QuerySegmentEnumerator(this.folder.ReferencedObject, ItemQueryType.None, segmentSize);
					base.TotalWork = (int)this.folder.ReferencedObject.PropertyBag[FolderSchema.ItemCount];
				}
				disposeGuard.Success();
			}
		}

		protected override SegmentOperationResult InternalDoNextBatchOperation()
		{
			TestInterceptor.Intercept(TestInterceptorLocation.SetReadFlagsSegmentedOperation_InternalDoNextBatchOperation, new object[0]);
			StoreObjectId[] nextBatchIds = this.setReadFlagsEnumerator.GetNextBatchIds();
			if (nextBatchIds.Length > 0)
			{
				bool flag;
				this.folder.ReferencedObject.SetReadFlags((int)this.setReadFlagFlags, (from id in nextBatchIds
				select id).ToArray<StoreId>(), out flag);
				return new SegmentOperationResult
				{
					CompletedWork = nextBatchIds.Length,
					OperationResult = (flag ? OperationResult.PartiallySucceeded : OperationResult.Succeeded),
					IsCompleted = false,
					Exception = null
				};
			}
			return SegmentedRopOperation.FinalResult;
		}

		internal override RopResult CreateCompleteResult(object progressToken, IProgressResultFactory resultFactory)
		{
			if (base.ErrorCode == ErrorCode.None)
			{
				return ((SetReadFlagsResultFactory)resultFactory).CreateSuccessfulResult(base.IsPartiallyCompleted);
			}
			return ((SetReadFlagsResultFactory)resultFactory).CreateFailedResult(base.ErrorCode, base.IsPartiallyCompleted);
		}

		internal override RopResult CreateCompleteResultForProgress(object progressToken, ProgressResultFactory progressResultFactory)
		{
			if (base.ErrorCode == ErrorCode.None)
			{
				return progressResultFactory.CreateSuccessfulSetReadFlagsResult(progressToken, base.IsPartiallyCompleted);
			}
			return progressResultFactory.CreateFailedSetReadFlagsResult(progressToken, base.ErrorCode, base.IsPartiallyCompleted);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.setReadFlagsEnumerator);
			if (this.folder != null)
			{
				this.folder.Release();
			}
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SetReadFlagsSegmentedOperation>(this);
		}

		private readonly ReferenceCount<CoreFolder> folder;

		private readonly SetReadFlagFlags setReadFlagFlags;

		private readonly SegmentEnumerator setReadFlagsEnumerator;
	}
}
