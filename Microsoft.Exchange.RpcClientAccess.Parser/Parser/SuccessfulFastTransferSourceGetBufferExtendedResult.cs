using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulFastTransferSourceGetBufferExtendedResult : FastTransferSourceGetBufferExtendedResult
	{
		internal SuccessfulFastTransferSourceGetBufferExtendedResult(FastTransferState state, uint progress, uint steps, bool isMoveUser, ArraySegment<byte> data) : base(ErrorCode.None, new FastTransferSourceGetBufferData(state, progress, steps, isMoveUser, data, true))
		{
		}

		internal SuccessfulFastTransferSourceGetBufferExtendedResult(Reader reader) : base(reader, false)
		{
		}
	}
}
