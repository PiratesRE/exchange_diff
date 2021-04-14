using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class InvalidNotificationEventArgs : EventArgs
	{
		public InvalidNotificationEventArgs(PushNotification notification, Exception ex)
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			ArgumentValidator.ThrowIfNull("ex", ex);
			this.Notification = notification;
			this.Exception = ex;
		}

		public PushNotification Notification { get; private set; }

		public Exception Exception { get; private set; }
	}
}
