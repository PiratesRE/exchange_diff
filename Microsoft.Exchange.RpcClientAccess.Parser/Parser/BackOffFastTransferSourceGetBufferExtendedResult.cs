using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class BackOffFastTransferSourceGetBufferExtendedResult : FastTransferSourceGetBufferExtendedResult
	{
		internal BackOffFastTransferSourceGetBufferExtendedResult(uint backOffTime) : base(ErrorCode.ServerBusy, new FastTransferSourceGetBufferData(backOffTime, true))
		{
		}

		internal BackOffFastTransferSourceGetBufferExtendedResult(Reader reader) : base(reader, true)
		{
		}
	}
}
