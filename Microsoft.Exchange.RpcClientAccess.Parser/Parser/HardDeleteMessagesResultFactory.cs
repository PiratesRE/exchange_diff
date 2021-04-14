using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class HardDeleteMessagesResultFactory : ResultFactory, IProgressResultFactory
	{
		internal HardDeleteMessagesResultFactory(byte logonId)
		{
			this.logonId = logonId;
		}

		internal HardDeleteMessagesResultFactory(object progressToken)
		{
			if (progressToken == null)
			{
				throw new ArgumentNullException("progressToken");
			}
			LogonProgressToken logonProgressToken = (LogonProgressToken)progressToken;
			if (logonProgressToken.RopId != RopId.HardDeleteMessages)
			{
				throw new ArgumentException("Incorrect progress token, token's RopId: " + logonProgressToken.RopId, "progressToken");
			}
			this.logonId = logonProgressToken.LogonId;
		}

		public static RopResult Parse(Reader reader)
		{
			return ResultFactory.ParseResultOrProgress(RopId.HardDeleteMessages, reader, (Reader resultReader) => new HardDeleteMessagesResult(resultReader));
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, false);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, bool isPartiallyCompleted)
		{
			return new HardDeleteMessagesResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulResult(bool isPartiallyCompleted)
		{
			return new HardDeleteMessagesResult(ErrorCode.None, isPartiallyCompleted);
		}

		public object CreateProgressToken()
		{
			return new LogonProgressToken(RopId.HardDeleteMessages, this.logonId);
		}

		public RopResult CreateProgressResult(uint completedTaskCount, uint totalTaskCount)
		{
			return new SuccessfulProgressResult(this.logonId, completedTaskCount, totalTaskCount);
		}

		private readonly byte logonId;
	}
}
