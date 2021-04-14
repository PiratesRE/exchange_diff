using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal enum RecoErrorType
	{
		Success,
		AudioQualityPoor,
		LanguageNotSupported,
		Rejected,
		BadRequest,
		SystemError,
		Timeout,
		MessageTooLong,
		ProtectedVoiceMail,
		Throttled,
		Other,
		ErrorReadingSettings
	}
}
