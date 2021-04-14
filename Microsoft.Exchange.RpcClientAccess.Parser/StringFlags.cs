using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	[Flags]
	internal enum StringFlags : uint
	{
		None = 0U,
		IncludeNull = 1U,
		Sized = 2U,
		Sized16 = 4U,
		SevenBitAscii = 8U,
		Sized32 = 16U,
		FailOnError = 32U,
		SevenBitAsciiOrFail = 40U
	}
}
