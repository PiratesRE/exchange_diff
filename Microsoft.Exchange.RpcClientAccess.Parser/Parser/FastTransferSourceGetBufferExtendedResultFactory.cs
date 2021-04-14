using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferSourceGetBufferExtendedResultFactory : ResultFactory
	{
		internal FastTransferSourceGetBufferExtendedResultFactory(ArraySegment<byte> outputBuffer)
		{
			this.outputBuffer = outputBuffer;
		}

		public ArraySegment<byte> GetOutputBuffer()
		{
			return this.outputBuffer;
		}

		public RopResult CreateBackoffResult(uint backOffTime)
		{
			return new BackOffFastTransferSourceGetBufferExtendedResult(backOffTime);
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode)
		{
			return new FailedFastTransferSourceGetBufferExtendedResult(errorCode);
		}

		public RopResult CreateSuccessfulResult(FastTransferState state, uint progressCount, uint totalStepCount, bool moveUserOperation, int outputBufferSize)
		{
			return new SuccessfulFastTransferSourceGetBufferExtendedResult(state, progressCount, totalStepCount, moveUserOperation, this.outputBuffer.SubSegment(0, outputBufferSize));
		}

		private readonly ArraySegment<byte> outputBuffer;

		internal static readonly FastTransferSourceGetBufferExtendedResultFactory Empty = new FastTransferSourceGetBufferExtendedResultFactory(default(ArraySegment<byte>));
	}
}
