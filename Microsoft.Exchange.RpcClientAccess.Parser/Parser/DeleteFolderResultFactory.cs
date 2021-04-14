using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DeleteFolderResultFactory : ResultFactory
	{
		internal DeleteFolderResultFactory()
		{
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode)
		{
			return new DeleteFolderResult(errorCode, false);
		}

		public RopResult CreateSuccessfulResult(bool isPartiallyCompleted)
		{
			return new DeleteFolderResult(ErrorCode.None, isPartiallyCompleted);
		}
	}
}
