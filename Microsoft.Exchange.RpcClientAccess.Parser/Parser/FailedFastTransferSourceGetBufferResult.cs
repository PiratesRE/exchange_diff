using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class FailedFastTransferSourceGetBufferResult : FastTransferSourceGetBufferResult
	{
		internal FailedFastTransferSourceGetBufferResult(ErrorCode errorCode) : base(errorCode, new FastTransferSourceGetBufferData(FastTransferState.Error, false))
		{
		}

		internal FailedFastTransferSourceGetBufferResult(Reader reader) : base(reader, false)
		{
		}
	}
}
