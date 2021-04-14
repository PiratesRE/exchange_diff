using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum ImportDeleteFlags : byte
	{
		None = 0,
		Hierarchy = 1,
		HardDelete = 2
	}
}
