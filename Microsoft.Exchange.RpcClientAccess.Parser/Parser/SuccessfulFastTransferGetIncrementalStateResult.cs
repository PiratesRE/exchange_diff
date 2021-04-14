using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulFastTransferGetIncrementalStateResult : RopResult
	{
		internal SuccessfulFastTransferGetIncrementalStateResult(IServerObject fastTransferDownloadObject) : base(RopId.FastTransferGetIncrementalState, ErrorCode.None, fastTransferDownloadObject)
		{
		}

		internal SuccessfulFastTransferGetIncrementalStateResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulFastTransferGetIncrementalStateResult Parse(Reader reader)
		{
			return new SuccessfulFastTransferGetIncrementalStateResult(reader);
		}
	}
}
