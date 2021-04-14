using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal enum AzureDeviceRegistrationChannelState
	{
		Init,
		ReadRegistration,
		CreateRegistrationId,
		Sending,
		Discarding
	}
}
