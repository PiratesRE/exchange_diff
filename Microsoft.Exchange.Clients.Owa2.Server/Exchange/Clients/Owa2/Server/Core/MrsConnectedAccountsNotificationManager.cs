using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MrsConnectedAccountsNotificationManager : ConnectedAccountsNotificationManagerBase
	{
		private MrsConnectedAccountsNotificationManager(Guid userMailboxGuid, Guid userMdbGuid, string userMailboxServerFQDN, ISyncNowNotificationClient notificationClient) : base(userMailboxGuid, userMdbGuid, userMailboxServerFQDN, ConnectedAccountsConfiguration.Instance, notificationClient)
		{
		}

		public static MrsConnectedAccountsNotificationManager Create(MailboxSession mailboxSession, UserContext userContext)
		{
			if (ConnectedAccountsNotificationManagerBase.ShouldSetupNotificationManagerForUser(mailboxSession, userContext) && MrsConnectedAccountsNotificationManager.ShouldSetupNotificationManagerForUser(mailboxSession))
			{
				IExchangePrincipal mailboxOwner = mailboxSession.MailboxOwner;
				ExTraceGlobals.ConnectedAccountsTracer.TraceDebug<Guid, Guid, string>((long)userContext.GetHashCode(), "MrsConnectedAccountsNotificationManager.Create::Setting up ConnectedAccountsNotificationManager for User (MailboxGuid:{0}, MdbGuid:{1}, ServerFullyQualifiedDomainName:{2}).", mailboxOwner.MailboxInfo.MailboxGuid, mailboxOwner.MailboxInfo.GetDatabaseGuid(), mailboxOwner.MailboxInfo.Location.ServerFqdn);
				ISyncNowNotificationClient notificationClient = new MrsSyncNowNotificationClient();
				return new MrsConnectedAccountsNotificationManager(mailboxOwner.MailboxInfo.MailboxGuid, mailboxOwner.MailboxInfo.GetDatabaseGuid(), mailboxOwner.MailboxInfo.Location.ServerFqdn, notificationClient);
			}
			return null;
		}

		internal static MrsConnectedAccountsNotificationManager CreateFromTest(Guid userMailboxGuid, Guid userMdbGuid, string userMailboxServerFQDN)
		{
			ISyncNowNotificationClient notificationClient = new MrsSyncNowNotificationClient();
			return new MrsConnectedAccountsNotificationManager(userMailboxGuid, userMdbGuid, userMailboxServerFQDN, notificationClient);
		}

		private static bool ShouldSetupNotificationManagerForUser(MailboxSession userMailboxSession)
		{
			SyncUtilities.ThrowIfArgumentNull("userMailboxSession", userMailboxSession);
			try
			{
				IExchangePrincipal mailboxOwner = userMailboxSession.MailboxOwner;
				ADUser aduser = DirectoryHelper.ReadADRecipient(mailboxOwner.MailboxInfo.MailboxGuid, mailboxOwner.MailboxInfo.IsArchive, userMailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid)) as ADUser;
				if (aduser != null)
				{
					AggregatedAccountHelper aggregatedAccountHelper = new AggregatedAccountHelper(userMailboxSession, aduser);
					List<AggregatedAccountInfo> listOfAccounts = aggregatedAccountHelper.GetListOfAccounts();
					if (listOfAccounts != null)
					{
						return listOfAccounts.Count > 0;
					}
				}
			}
			catch (LocalizedException arg)
			{
				ExTraceGlobals.ConnectedAccountsTracer.TraceError<Guid, SmtpAddress, LocalizedException>((long)userMailboxSession.GetHashCode(), "MrsConnectedAccountsNotificationManager.ShouldSetupNotificationManagerForUser failed for User (MailboxGuid:{0}, PrimarySmtpAddress:{1}), with error:{2}. We will assume that user has active connected accounts.", userMailboxSession.MailboxGuid, userMailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress, arg);
			}
			return false;
		}
	}
}
