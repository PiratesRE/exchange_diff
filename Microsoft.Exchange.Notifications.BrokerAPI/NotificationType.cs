using System;

namespace Microsoft.Exchange.Notifications.Broker
{
	[Serializable]
	public enum NotificationType
	{
		Dropped,
		Missed,
		NewMail,
		Conversation,
		MessageItem,
		CalendarItem,
		PeopleIKnow,
		UnseenCount,
		ConnectionDropped
	}
}
