using System;

namespace Microsoft.Exchange.Data.PushNotifications
{
	[Flags]
	public enum PushNotificationSubscriptionOption
	{
		NoSubscription = 0,
		Email = 1,
		Calendar = 2,
		VoiceMail = 4,
		MissedCall = 8,
		SuppressNotificationsWhenOof = 16,
		BackgroundSync = 32
	}
}
