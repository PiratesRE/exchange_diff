using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class SuccessfulFastTransferSourceCopyFolderResult : RopResult
	{
		internal SuccessfulFastTransferSourceCopyFolderResult(IServerObject fastTransferDownloadObject) : base(RopId.FastTransferSourceCopyFolder, ErrorCode.None, fastTransferDownloadObject)
		{
		}

		internal SuccessfulFastTransferSourceCopyFolderResult(Reader reader) : base(reader)
		{
		}

		internal static SuccessfulFastTransferSourceCopyFolderResult Parse(Reader reader)
		{
			return new SuccessfulFastTransferSourceCopyFolderResult(reader);
		}
	}
}
