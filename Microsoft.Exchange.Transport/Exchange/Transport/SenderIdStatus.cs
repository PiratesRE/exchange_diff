using System;

namespace Microsoft.Exchange.Transport
{
	internal enum SenderIdStatus : uint
	{
		NEUTRAL = 1U,
		PASS,
		FAIL,
		SOFTFAIL,
		NONE,
		TEMPERROR = 2147483654U,
		PERMERROR
	}
}
