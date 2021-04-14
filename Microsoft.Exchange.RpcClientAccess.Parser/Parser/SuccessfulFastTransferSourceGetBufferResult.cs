using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulFastTransferSourceGetBufferResult : FastTransferSourceGetBufferResult
	{
		internal SuccessfulFastTransferSourceGetBufferResult(FastTransferState state, ushort progress, ushort steps, bool isMoveUser, ArraySegment<byte> data) : base(ErrorCode.None, new FastTransferSourceGetBufferData(state, (uint)progress, (uint)steps, isMoveUser, data, false))
		{
		}

		internal SuccessfulFastTransferSourceGetBufferResult(Reader reader) : base(reader, false)
		{
		}
	}
}
