using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal enum FastTransferCopyOperation : byte
	{
		CopyTo = 1,
		CopyProperties,
		CopyMessages,
		CopyFolder
	}
}
