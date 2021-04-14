using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CopyToStreamResultFactory : ResultFactory
	{
		internal CopyToStreamResultFactory(uint destinationObjectHandleIndex)
		{
			this.destinationObjectHandleIndex = destinationObjectHandleIndex;
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, 0UL, 0UL);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, ulong bytesRead, ulong bytesWritten)
		{
			return new CopyToStreamResult(errorCode, bytesRead, bytesWritten, this.destinationObjectHandleIndex);
		}

		public RopResult CreateSuccessfulResult(ulong bytesRead, ulong bytesWritten)
		{
			return new CopyToStreamResult(ErrorCode.None, bytesRead, bytesWritten, 0U);
		}

		private readonly uint destinationObjectHandleIndex;
	}
}
