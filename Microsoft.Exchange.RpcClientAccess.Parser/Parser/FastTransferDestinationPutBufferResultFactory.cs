using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferDestinationPutBufferResultFactory : StandardResultFactory
	{
		internal FastTransferDestinationPutBufferResultFactory() : base(RopId.FastTransferDestinationPutBuffer)
		{
		}

		public RopResult CreateSuccessfulResult(ushort progressCount, ushort totalStepCount, bool moveUserOperation, ushort usedBufferSize)
		{
			return new FastTransferDestinationPutBufferResult(ErrorCode.None, progressCount, totalStepCount, moveUserOperation, usedBufferSize);
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, 0, 0, false, 0);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, ushort progressCount, ushort totalStepCount, bool moveUserOperation, ushort usedBufferSize)
		{
			return new FastTransferDestinationPutBufferResult(errorCode, progressCount, totalStepCount, moveUserOperation, usedBufferSize);
		}
	}
}
