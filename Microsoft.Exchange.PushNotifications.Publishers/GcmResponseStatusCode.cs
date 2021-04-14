using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum GcmResponseStatusCode
	{
		Undefined,
		Success,
		MissingRegistration,
		InvalidRegistration,
		MismatchSenderId,
		NotRegistered,
		MessageTooBig,
		InvalidDataKey,
		InvalidTtl,
		InvalidPackageName,
		InvalidResponse
	}
}
