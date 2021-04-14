using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MoveCopyMessagesExtendedWithEntryIdsResultFactory : ResultFactory, IProgressResultFactory
	{
		internal MoveCopyMessagesExtendedWithEntryIdsResultFactory(byte logonId, uint destinationObjectHandleIndex)
		{
			this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			this.logonId = logonId;
		}

		internal MoveCopyMessagesExtendedWithEntryIdsResultFactory(object progressToken)
		{
			if (progressToken == null)
			{
				throw new ArgumentNullException("progressToken");
			}
			LogonDestinationHandleProgressToken logonDestinationHandleProgressToken = (LogonDestinationHandleProgressToken)progressToken;
			if (logonDestinationHandleProgressToken.RopId != RopId.MoveCopyMessagesExtendedWithEntryIds)
			{
				throw new ArgumentException("Wrong RopId in progress token:" + logonDestinationHandleProgressToken.RopId, "progressToken");
			}
			this.destinationObjectHandleIndex = logonDestinationHandleProgressToken.DestinationObjectHandleIndex;
			this.logonId = logonDestinationHandleProgressToken.LogonId;
		}

		public static RopResult Parse(Reader reader)
		{
			return ResultFactory.ParseResultOrProgress(RopId.MoveCopyMessagesExtendedWithEntryIds, reader, (Reader resultReader) => new MoveCopyMessagesExtendedWithEntryIdsResult(resultReader));
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, false);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, bool isPartiallyCompleted)
		{
			return new MoveCopyMessagesExtendedWithEntryIdsResult(errorCode, isPartiallyCompleted, null, null, this.destinationObjectHandleIndex);
		}

		public RopResult CreateSuccessfulResult(bool isPartiallyCompleted, StoreId[] messageIds, ulong[] changeNumbers)
		{
			return new MoveCopyMessagesExtendedWithEntryIdsResult(ErrorCode.None, isPartiallyCompleted, messageIds, changeNumbers, this.destinationObjectHandleIndex);
		}

		public object CreateProgressToken()
		{
			return new LogonDestinationHandleProgressToken(RopId.MoveCopyMessagesExtendedWithEntryIds, this.destinationObjectHandleIndex, this.logonId);
		}

		public RopResult CreateProgressResult(uint completedTaskCount, uint totalTaskCount)
		{
			return new SuccessfulProgressResult(this.logonId, completedTaskCount, totalTaskCount);
		}

		private readonly byte logonId;

		private readonly uint destinationObjectHandleIndex;
	}
}
