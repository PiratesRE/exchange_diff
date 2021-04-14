using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiNotification
	{
		public static void AbandonNotificationsDuringShutdown(bool abandon)
		{
			NativeMethods.AbandonNotificationsDuringShutdown(abandon);
		}

		public AdviseFlags NotificationType
		{
			get
			{
				return this.notificationType;
			}
		}

		internal unsafe MapiNotification(NOTIFICATION* notification)
		{
			this.notificationType = (AdviseFlags)notification->ulEventType;
		}

		internal unsafe static MapiNotification Create(NOTIFICATION* notification)
		{
			AdviseFlags ulEventType = (AdviseFlags)notification->ulEventType;
			if (ulEventType <= AdviseFlags.ObjectMoved)
			{
				if (ulEventType <= AdviseFlags.ObjectCreated)
				{
					if (ulEventType == AdviseFlags.Extended)
					{
						return new MapiExtendedNotification(notification);
					}
					switch (ulEventType)
					{
					case AdviseFlags.CriticalError:
						return new MapiErrorNotification(notification);
					case AdviseFlags.NewMail:
						return new MapiNewMailNotification(notification);
					case AdviseFlags.ObjectCreated:
						return new MapiObjectNotification(notification);
					}
				}
				else
				{
					if (ulEventType == AdviseFlags.ObjectDeleted)
					{
						return new MapiObjectNotification(notification);
					}
					if (ulEventType == AdviseFlags.ObjectModified)
					{
						return new MapiObjectNotification(notification);
					}
					if (ulEventType == AdviseFlags.ObjectMoved)
					{
						return new MapiObjectNotification(notification);
					}
				}
			}
			else if (ulEventType <= AdviseFlags.SearchComplete)
			{
				if (ulEventType == AdviseFlags.ObjectCopied)
				{
					return new MapiObjectNotification(notification);
				}
				if (ulEventType == AdviseFlags.SearchComplete)
				{
					return new MapiNotification(notification);
				}
			}
			else
			{
				if (ulEventType == AdviseFlags.TableModified)
				{
					return new MapiTableNotification(notification);
				}
				if (ulEventType == AdviseFlags.StatusObjectModified)
				{
					return new MapiStatusObjectNotification(notification);
				}
				if (ulEventType == AdviseFlags.ConnectionDropped)
				{
					return new MapiConnectionDroppedNotification(notification);
				}
			}
			return new MapiNotification(notification);
		}

		private readonly AdviseFlags notificationType;
	}
}
