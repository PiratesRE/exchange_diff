using System;
using Microsoft.Exchange.Data.PushNotifications;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PushNotificationSubscriptionTableEntry
	{
		public static bool IsSubscriptionDisabled(byte[] subscriptionOption)
		{
			return subscriptionOption == null || subscriptionOption.Length == 0 || PushNotificationSubscriptionTableEntry.IsSubscriptionDisabled((PushNotificationSubscriptionOption)subscriptionOption[0]);
		}

		public static bool IsSubscriptionDisabled(PushNotificationSubscriptionOption subscriptionOption)
		{
			return subscriptionOption == PushNotificationSubscriptionOption.NoSubscription;
		}

		public static bool IsEmailSubscriptionEnabled(PushNotificationSubscriptionOption subscriptionOption)
		{
			return (subscriptionOption & PushNotificationSubscriptionOption.Email) == PushNotificationSubscriptionOption.Email;
		}

		public static bool IsSuppressNotificationsWhenOofEnabled(PushNotificationSubscriptionOption subscriptionOption)
		{
			return (subscriptionOption & PushNotificationSubscriptionOption.SuppressNotificationsWhenOof) == PushNotificationSubscriptionOption.SuppressNotificationsWhenOof;
		}

		public static bool IsBackgroundSyncEnabled(PushNotificationSubscriptionOption subscriptionOption)
		{
			return (subscriptionOption & PushNotificationSubscriptionOption.BackgroundSync) == PushNotificationSubscriptionOption.BackgroundSync;
		}

		public void DisableSubscriptionOnMailboxTable(IMailboxSession session)
		{
			this.UpdateSubscriptionOnMailboxTable(session, PushNotificationSubscriptionOption.NoSubscription);
		}

		public void UpdateSubscriptionOnMailboxTable(IMailboxSession session, PushNotificationSubscription subscription)
		{
			this.UpdateSubscriptionOnMailboxTable(session, subscription.GetSubscriptionOption());
		}

		public virtual PushNotificationSubscriptionOption ReadSubscriptionOnMailboxTable(IMailboxSession session)
		{
			session.Mailbox.Load(new PropertyDefinition[]
			{
				MailboxSchema.PushNotificationSubscriptionType
			});
			object obj = session.Mailbox.TryGetProperty(MailboxSchema.PushNotificationSubscriptionType);
			byte[] array = obj as byte[];
			if (!(obj is PropertyError) && !PushNotificationSubscriptionTableEntry.IsSubscriptionDisabled(array))
			{
				return (PushNotificationSubscriptionOption)array[0];
			}
			return PushNotificationSubscriptionOption.NoSubscription;
		}

		public virtual void UpdateSubscriptionOnMailboxTable(IMailboxSession session, PushNotificationSubscriptionOption subscriptionOption)
		{
			session.Mailbox.Load(new PropertyDefinition[]
			{
				MailboxSchema.PushNotificationSubscriptionType
			});
			session.Mailbox[MailboxSchema.PushNotificationSubscriptionType] = new byte[]
			{
				(byte)subscriptionOption
			};
			session.Mailbox.Save();
			session.Mailbox.Load();
		}

		public const int SubscriptionTableEntryArrayLength = 1;
	}
}
