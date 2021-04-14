using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum ServerCapabilityFlag
	{
		PackedFastTransferUploadBuffers = 1,
		PackedWriteStreamExtendedUploadBuffers = 2
	}
}
