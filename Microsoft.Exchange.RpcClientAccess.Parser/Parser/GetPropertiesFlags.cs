using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum GetPropertiesFlags : ulong
	{
		None = 0UL,
		Unicode = 2147483648UL
	}
}
