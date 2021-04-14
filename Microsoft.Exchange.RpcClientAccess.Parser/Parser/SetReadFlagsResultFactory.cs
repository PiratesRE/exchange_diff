using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetReadFlagsResultFactory : ResultFactory, IProgressResultFactory
	{
		internal SetReadFlagsResultFactory(byte logonId)
		{
			this.logonId = logonId;
		}

		internal SetReadFlagsResultFactory(object progressToken)
		{
			if (progressToken == null)
			{
				throw new ArgumentNullException("progressToken");
			}
			LogonProgressToken logonProgressToken = (LogonProgressToken)progressToken;
			if (logonProgressToken.RopId != RopId.SetReadFlags)
			{
				throw new ArgumentException("Incorrect progress token, token's RopId: " + logonProgressToken.RopId, "progressToken");
			}
			this.logonId = logonProgressToken.LogonId;
		}

		public static RopResult Parse(Reader reader)
		{
			return ResultFactory.ParseResultOrProgress(RopId.SetReadFlags, reader, (Reader resultReader) => new SetReadFlagsResult(resultReader));
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, false);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, bool isPartiallyCompleted)
		{
			return new SetReadFlagsResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulResult(bool isPartiallyCompleted)
		{
			return new SetReadFlagsResult(ErrorCode.None, isPartiallyCompleted);
		}

		public object CreateProgressToken()
		{
			return new LogonProgressToken(RopId.SetReadFlags, this.logonId);
		}

		public RopResult CreateProgressResult(uint completedTaskCount, uint totalTaskCount)
		{
			return new SuccessfulProgressResult(this.logonId, completedTaskCount, totalTaskCount);
		}

		private readonly byte logonId;
	}
}
