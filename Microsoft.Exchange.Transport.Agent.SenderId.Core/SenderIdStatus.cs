using System;

namespace Microsoft.Exchange.SenderId
{
	internal enum SenderIdStatus
	{
		Pass = 1,
		Neutral,
		SoftFail,
		Fail,
		None,
		TempError,
		PermError
	}
}
