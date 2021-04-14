using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulFastTransferSourceCopyToResult : RopResult
	{
		internal SuccessfulFastTransferSourceCopyToResult(IServerObject fastTransferDownloadObject) : base(RopId.FastTransferSourceCopyTo, ErrorCode.None, fastTransferDownloadObject)
		{
		}

		internal SuccessfulFastTransferSourceCopyToResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulFastTransferSourceCopyToResult Parse(Reader reader)
		{
			return new SuccessfulFastTransferSourceCopyToResult(reader);
		}
	}
}
