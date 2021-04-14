using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class HardDeleteMessagesSegmentedOperation : DeleteMessagesSegmentedOperation
	{
		internal HardDeleteMessagesSegmentedOperation(ReferenceCount<CoreFolder> folder, DeleteItemFlags deleteItemFlags, StoreObjectId[] storeObjectIds, int segmentSize, TeamMailboxClientOperations teamMailboxClientOperations) : base(folder, deleteItemFlags, storeObjectIds, segmentSize, teamMailboxClientOperations)
		{
		}

		internal override RopResult CreateCompleteResult(object progressToken, IProgressResultFactory resultFactory)
		{
			if (base.ErrorCode != ErrorCode.None)
			{
				return ((HardDeleteMessagesResultFactory)resultFactory).CreateSuccessfulResult(base.IsPartiallyCompleted);
			}
			return ((HardDeleteMessagesResultFactory)resultFactory).CreateFailedResult(base.ErrorCode, base.IsPartiallyCompleted);
		}

		internal override RopResult CreateCompleteResultForProgress(object progressToken, ProgressResultFactory progressResultFactory)
		{
			if (base.ErrorCode == ErrorCode.None)
			{
				return progressResultFactory.CreateSuccessfulHardDeleteMessagesResult(progressToken, base.IsPartiallyCompleted);
			}
			return progressResultFactory.CreateFailedHardDeleteMessagesResult(progressToken, base.ErrorCode, base.IsPartiallyCompleted);
		}
	}
}
