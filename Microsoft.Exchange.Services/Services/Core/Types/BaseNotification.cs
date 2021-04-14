using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class BaseNotification
	{
		public BaseNotification(NotificationKindType notificationKind)
		{
			this.NotificationKind = notificationKind;
		}

		public BaseNotification()
		{
		}

		public NotificationKindType NotificationKind { get; private set; }

		public NotificationTypeType NotificationType { get; set; }
	}
}
