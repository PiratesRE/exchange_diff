using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	[Flags]
	internal enum PropertyTypeFlags : ushort
	{
		MultiValueFlag = 4096,
		MultiValueInstanceFlag = 8192
	}
}
