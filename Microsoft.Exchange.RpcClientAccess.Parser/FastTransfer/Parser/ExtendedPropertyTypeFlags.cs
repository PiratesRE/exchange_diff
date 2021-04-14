using System;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[Flags]
	internal enum ExtendedPropertyTypeFlags : ushort
	{
		CodePageStringFlag = 32768,
		CodePageMask = 4095
	}
}
