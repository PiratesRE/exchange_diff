using System;

namespace Microsoft.Exchange.Nspi
{
	public enum NspiStatus
	{
		Success,
		UnbindSuccess,
		UnbindFailure,
		ErrorsReturned = 263040,
		GeneralFailure = -2147467259,
		NotSupported = -2147221246,
		NotFound = -2147221233,
		LogonFailed = -2147221231,
		TooComplex = -2147221225,
		InvalidCodePage = -2147221218,
		InvalidLocale,
		TooBig = -2147220731,
		TableTooBig = -2147220477,
		InvalidBookmark = -2147220475,
		AccessDenied = -2147024891,
		InvalidParameter = -2147024809
	}
}
