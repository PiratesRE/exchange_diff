using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulFastTransferSourceCopyMessagesResult : RopResult
	{
		internal SuccessfulFastTransferSourceCopyMessagesResult(IServerObject fastTransferDownloadObject) : base(RopId.FastTransferSourceCopyMessages, ErrorCode.None, fastTransferDownloadObject)
		{
		}

		internal SuccessfulFastTransferSourceCopyMessagesResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulFastTransferSourceCopyMessagesResult Parse(Reader reader)
		{
			return new SuccessfulFastTransferSourceCopyMessagesResult(reader);
		}
	}
}
