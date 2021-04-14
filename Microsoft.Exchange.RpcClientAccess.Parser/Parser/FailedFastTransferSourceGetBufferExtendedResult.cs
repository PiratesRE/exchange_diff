using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class FailedFastTransferSourceGetBufferExtendedResult : FastTransferSourceGetBufferExtendedResult
	{
		internal FailedFastTransferSourceGetBufferExtendedResult(ErrorCode errorCode) : base(errorCode, new FastTransferSourceGetBufferData(FastTransferState.Error, true))
		{
		}

		internal FailedFastTransferSourceGetBufferExtendedResult(Reader reader) : base(reader, false)
		{
		}
	}
}
