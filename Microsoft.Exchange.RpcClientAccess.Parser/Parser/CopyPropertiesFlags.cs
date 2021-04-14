using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum CopyPropertiesFlags : byte
	{
		None = 0,
		Move = 1,
		NoReplace = 2
	}
}
