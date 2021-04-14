using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CopyPropertiesResultFactory : ResultFactory, IProgressResultFactory
	{
		internal CopyPropertiesResultFactory(byte logonId, uint destinationObjectHandleIndex)
		{
			this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			this.logonId = logonId;
		}

		internal CopyPropertiesResultFactory(object progressToken)
		{
			if (progressToken == null)
			{
				throw new ArgumentNullException("progressToken");
			}
			LogonDestinationHandleProgressToken logonDestinationHandleProgressToken = (LogonDestinationHandleProgressToken)progressToken;
			if (logonDestinationHandleProgressToken.RopId != RopId.CopyProperties)
			{
				throw new ArgumentException("Incorrect progress token, token's RopId: " + logonDestinationHandleProgressToken.RopId, "progressToken");
			}
			this.destinationObjectHandleIndex = logonDestinationHandleProgressToken.DestinationObjectHandleIndex;
			this.logonId = logonDestinationHandleProgressToken.LogonId;
		}

		public static RopResult Parse(Reader reader)
		{
			return ResultFactory.ParseResultOrProgress(RopId.CopyProperties, reader, (Reader resultReader) => new SuccessfulCopyPropertiesResult(resultReader), (Reader resultReader) => new FailedCopyPropertiesResult(resultReader));
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode)
		{
			return new FailedCopyPropertiesResult(errorCode, this.destinationObjectHandleIndex);
		}

		public RopResult CreateSuccessfulResult(PropertyProblem[] propertyProblems)
		{
			return new SuccessfulCopyPropertiesResult(propertyProblems);
		}

		public object CreateProgressToken()
		{
			return new LogonDestinationHandleProgressToken(RopId.CopyProperties, this.destinationObjectHandleIndex, this.logonId);
		}

		public RopResult CreateProgressResult(uint completedTaskCount, uint totalTaskCount)
		{
			return new SuccessfulProgressResult(this.logonId, completedTaskCount, totalTaskCount);
		}

		private readonly byte logonId;

		private readonly uint destinationObjectHandleIndex;
	}
}
