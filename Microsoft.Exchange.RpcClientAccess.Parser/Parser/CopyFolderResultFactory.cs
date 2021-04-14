using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CopyFolderResultFactory : ResultFactory, IProgressResultFactory
	{
		internal CopyFolderResultFactory(byte logonId, uint destinationObjectHandleIndex)
		{
			this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			this.logonId = logonId;
		}

		internal CopyFolderResultFactory(object progressToken)
		{
			if (progressToken == null)
			{
				throw new ArgumentNullException("progressToken");
			}
			LogonDestinationHandleProgressToken logonDestinationHandleProgressToken = (LogonDestinationHandleProgressToken)progressToken;
			if (logonDestinationHandleProgressToken.RopId != RopId.CopyFolder)
			{
				throw new ArgumentException("Incorrect progress token, token's RopId: " + logonDestinationHandleProgressToken.RopId, "progressToken");
			}
			this.destinationObjectHandleIndex = logonDestinationHandleProgressToken.DestinationObjectHandleIndex;
			this.logonId = logonDestinationHandleProgressToken.LogonId;
		}

		public static RopResult Parse(Reader reader)
		{
			return ResultFactory.ParseResultOrProgress(RopId.CopyFolder, reader, (Reader resultReader) => new CopyFolderResult(resultReader));
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, false);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, bool isPartiallyCompleted)
		{
			return new CopyFolderResult(errorCode, isPartiallyCompleted, this.destinationObjectHandleIndex);
		}

		public RopResult CreateSuccessfulResult(bool isPartiallyCompleted)
		{
			return new CopyFolderResult(ErrorCode.None, isPartiallyCompleted, this.destinationObjectHandleIndex);
		}

		public object CreateProgressToken()
		{
			return new LogonDestinationHandleProgressToken(RopId.CopyFolder, this.destinationObjectHandleIndex, this.logonId);
		}

		public RopResult CreateProgressResult(uint completedTaskCount, uint totalTaskCount)
		{
			return new SuccessfulProgressResult(this.logonId, completedTaskCount, totalTaskCount);
		}

		private readonly uint destinationObjectHandleIndex;

		private readonly byte logonId;
	}
}
