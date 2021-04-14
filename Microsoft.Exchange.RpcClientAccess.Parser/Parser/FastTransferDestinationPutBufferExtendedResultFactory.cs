using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferDestinationPutBufferExtendedResultFactory : StandardResultFactory
	{
		internal FastTransferDestinationPutBufferExtendedResultFactory() : base(RopId.FastTransferDestinationPutBufferExtended)
		{
		}

		public RopResult CreateSuccessfulResult(uint progressCount, uint totalStepCount, bool moveUserOperation, ushort usedBufferSize)
		{
			return new FastTransferDestinationPutBufferExtendedResult(ErrorCode.None, progressCount, totalStepCount, moveUserOperation, usedBufferSize);
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, 0U, 0U, false, 0);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, uint progressCount, uint totalStepCount, bool moveUserOperation, ushort usedBufferSize)
		{
			return new FastTransferDestinationPutBufferExtendedResult(errorCode, progressCount, totalStepCount, moveUserOperation, usedBufferSize);
		}
	}
}
