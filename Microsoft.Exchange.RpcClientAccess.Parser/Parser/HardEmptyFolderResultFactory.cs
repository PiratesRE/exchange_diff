using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class HardEmptyFolderResultFactory : ResultFactory, IProgressResultFactory
	{
		internal HardEmptyFolderResultFactory(byte logonId)
		{
			this.logonId = logonId;
		}

		internal HardEmptyFolderResultFactory(object progressToken)
		{
			if (progressToken == null)
			{
				throw new ArgumentNullException("progressToken");
			}
			LogonProgressToken logonProgressToken = (LogonProgressToken)progressToken;
			if (logonProgressToken.RopId != RopId.HardEmptyFolder)
			{
				throw new ArgumentException("Incorrect progress token, token's RopId: " + logonProgressToken.RopId, "progressToken");
			}
			this.logonId = logonProgressToken.LogonId;
		}

		public static RopResult Parse(Reader reader)
		{
			return ResultFactory.ParseResultOrProgress(RopId.HardEmptyFolder, reader, (Reader resultReader) => new HardEmptyFolderResult(resultReader));
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, false);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, bool isPartiallyCompleted)
		{
			return new HardEmptyFolderResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulResult(bool isPartiallyCompleted)
		{
			return new HardEmptyFolderResult(ErrorCode.None, isPartiallyCompleted);
		}

		public object CreateProgressToken()
		{
			return new LogonProgressToken(RopId.HardEmptyFolder, this.logonId);
		}

		public RopResult CreateProgressResult(uint completedTaskCount, uint totalTaskCount)
		{
			return new SuccessfulProgressResult(this.logonId, completedTaskCount, totalTaskCount);
		}

		private readonly byte logonId;
	}
}
