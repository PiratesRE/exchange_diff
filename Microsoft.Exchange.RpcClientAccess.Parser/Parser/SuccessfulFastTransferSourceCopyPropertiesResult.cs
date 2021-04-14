using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulFastTransferSourceCopyPropertiesResult : RopResult
	{
		internal SuccessfulFastTransferSourceCopyPropertiesResult(IServerObject fastTransferDownloadObject) : base(RopId.FastTransferSourceCopyProperties, ErrorCode.None, fastTransferDownloadObject)
		{
		}

		internal SuccessfulFastTransferSourceCopyPropertiesResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulFastTransferSourceCopyPropertiesResult Parse(Reader reader)
		{
			return new SuccessfulFastTransferSourceCopyPropertiesResult(reader);
		}
	}
}
