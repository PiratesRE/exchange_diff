using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MoveCopyMessagesExtendedResultFactory : ResultFactory, IProgressResultFactory
	{
		internal MoveCopyMessagesExtendedResultFactory(byte logonId, uint destinationObjectHandleIndex)
		{
			this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			this.logonId = logonId;
		}

		internal MoveCopyMessagesExtendedResultFactory(object progressToken)
		{
			if (progressToken == null)
			{
				throw new ArgumentNullException("progressToken");
			}
			LogonDestinationHandleProgressToken logonDestinationHandleProgressToken = (LogonDestinationHandleProgressToken)progressToken;
			if (logonDestinationHandleProgressToken.RopId != RopId.MoveCopyMessagesExtended)
			{
				throw new ArgumentException("Wrong RopId in progress token:" + logonDestinationHandleProgressToken.RopId, "progressToken");
			}
			this.destinationObjectHandleIndex = logonDestinationHandleProgressToken.DestinationObjectHandleIndex;
			this.logonId = logonDestinationHandleProgressToken.LogonId;
		}

		public static RopResult Parse(Reader reader)
		{
			return ResultFactory.ParseResultOrProgress(RopId.MoveCopyMessagesExtended, reader, (Reader resultReader) => new MoveCopyMessagesExtendedResult(resultReader));
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, false);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, bool isPartiallyCompleted)
		{
			return new MoveCopyMessagesExtendedResult(errorCode, isPartiallyCompleted, this.destinationObjectHandleIndex);
		}

		public RopResult CreateSuccessfulResult(bool isPartiallyCompleted)
		{
			return new MoveCopyMessagesExtendedResult(ErrorCode.None, isPartiallyCompleted, this.destinationObjectHandleIndex);
		}

		public object CreateProgressToken()
		{
			return new LogonDestinationHandleProgressToken(RopId.MoveCopyMessagesExtended, this.destinationObjectHandleIndex, this.logonId);
		}

		public RopResult CreateProgressResult(uint completedTaskCount, uint totalTaskCount)
		{
			return new SuccessfulProgressResult(this.logonId, completedTaskCount, totalTaskCount);
		}

		private readonly byte logonId;

		private readonly uint destinationObjectHandleIndex;
	}
}
