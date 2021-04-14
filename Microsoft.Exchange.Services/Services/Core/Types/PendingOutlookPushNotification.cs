using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class PendingOutlookPushNotification
	{
		internal bool CalendarChanged { get; set; }

		internal bool ConnectionLost { get; set; }

		internal uint NotificationWaterMark { get; set; }

		internal Guid MailboxGuid { get; set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"PendingOutlookPushNotification(CalendarChanged=",
				this.CalendarChanged,
				"; NotificationWaterMark=",
				this.NotificationWaterMark,
				"; MailboxGuid=",
				this.MailboxGuid,
				")"
			});
		}

		internal void Merge(PendingOutlookPushNotification other)
		{
			this.CalendarChanged |= other.CalendarChanged;
			this.ConnectionLost |= other.ConnectionLost;
			if (this.NotificationWaterMark < other.NotificationWaterMark)
			{
				this.NotificationWaterMark = other.NotificationWaterMark;
			}
		}
	}
}
