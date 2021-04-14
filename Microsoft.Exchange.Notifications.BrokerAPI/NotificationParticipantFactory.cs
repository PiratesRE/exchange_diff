using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal static class NotificationParticipantFactory
	{
		public static NotificationParticipant FromExchangePrincipal(ExchangePrincipal exchangePrincipal)
		{
			NotificationParticipant notificationParticipant = new NotificationParticipant();
			IMailboxInfo mailboxInfo = exchangePrincipal.MailboxInfo;
			notificationParticipant.OrganizationId = mailboxInfo.OrganizationId;
			notificationParticipant.DatabaseGuid = mailboxInfo.MailboxDatabase.ObjectGuid;
			notificationParticipant.MailboxGuid = mailboxInfo.MailboxGuid;
			notificationParticipant.MailboxSmtp = mailboxInfo.PrimarySmtpAddress.ToString();
			notificationParticipant.LocationKind = NotificationParticipantLocationKind.LocalResourceForest;
			return notificationParticipant;
		}

		public static NotificationParticipant FromADUser(ADUser user)
		{
			NotificationParticipant notificationParticipant = new NotificationParticipant();
			notificationParticipant.OrganizationId = user.OrganizationId;
			notificationParticipant.MailboxSmtp = user.PrimarySmtpAddress.ToString();
			if (user.Database != null)
			{
				if (PartitionId.IsLocalForestPartition(user.Database.PartitionFQDN))
				{
					notificationParticipant.LocationKind = NotificationParticipantLocationKind.LocalResourceForest;
				}
				else
				{
					notificationParticipant.LocationKind = NotificationParticipantLocationKind.RemoteResourceForest;
				}
				notificationParticipant.DatabaseGuid = user.Database.ObjectGuid;
				notificationParticipant.MailboxGuid = user.ExchangeGuid;
			}
			else
			{
				notificationParticipant.LocationKind = NotificationParticipantLocationKind.CrossPremise;
			}
			return notificationParticipant;
		}
	}
}
