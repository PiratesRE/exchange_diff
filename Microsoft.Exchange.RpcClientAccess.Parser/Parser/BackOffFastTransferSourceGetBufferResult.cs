using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class BackOffFastTransferSourceGetBufferResult : FastTransferSourceGetBufferResult
	{
		internal BackOffFastTransferSourceGetBufferResult(uint backOffTime) : base(ErrorCode.ServerBusy, new FastTransferSourceGetBufferData(backOffTime, false))
		{
		}

		internal BackOffFastTransferSourceGetBufferResult(Reader reader) : base(reader, true)
		{
		}
	}
}
