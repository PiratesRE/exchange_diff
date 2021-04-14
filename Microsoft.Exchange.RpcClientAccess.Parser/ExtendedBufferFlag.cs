using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	[Flags]
	internal enum ExtendedBufferFlag : ushort
	{
		Compressed = 1,
		Obfuscated = 2,
		Last = 4
	}
}
