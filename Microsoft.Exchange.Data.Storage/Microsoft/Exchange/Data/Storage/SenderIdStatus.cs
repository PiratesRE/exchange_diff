using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum SenderIdStatus : uint
	{
		Neutral = 1U,
		Pass,
		Fail,
		SoftFail,
		None,
		TempError = 2147483654U,
		PermError
	}
}
