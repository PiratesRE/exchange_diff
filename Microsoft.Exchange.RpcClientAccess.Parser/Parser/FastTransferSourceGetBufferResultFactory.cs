using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FastTransferSourceGetBufferResultFactory : ResultFactory
	{
		internal FastTransferSourceGetBufferResultFactory(ArraySegment<byte> outputBuffer)
		{
			this.outputBuffer = outputBuffer;
		}

		public ArraySegment<byte> GetOutputBuffer()
		{
			return this.outputBuffer;
		}

		public RopResult CreateBackoffResult(uint backOffTime)
		{
			return new BackOffFastTransferSourceGetBufferResult(backOffTime);
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode)
		{
			return new FailedFastTransferSourceGetBufferResult(errorCode);
		}

		public RopResult CreateSuccessfulResult(FastTransferState state, ushort progressCount, ushort totalStepCount, bool moveUserOperation, int outputBufferSize)
		{
			return new SuccessfulFastTransferSourceGetBufferResult(state, progressCount, totalStepCount, moveUserOperation, this.outputBuffer.SubSegment(0, outputBufferSize));
		}

		private readonly ArraySegment<byte> outputBuffer;

		internal static readonly FastTransferSourceGetBufferResultFactory Empty = new FastTransferSourceGetBufferResultFactory(default(ArraySegment<byte>));
	}
}
