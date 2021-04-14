using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal enum TelephoneNumberProcessStatus
	{
		Success,
		DialPlanNotSupported,
		PhoneNumberAlreadyRegistered,
		PhoneNumberReachQuota,
		PhoneNumberUsedByOthers,
		PhoneNumberInvalidFormat,
		PhoneNumberInvalidCountryCode,
		PhoneNumberInvalidLength,
		Failure
	}
}
