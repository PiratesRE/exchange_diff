using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MRSImapSyncRequestAccessor : MRSSyncRequestAccessorBase
	{
		public MRSImapSyncRequestAccessor(IMigrationDataProvider dataProvider, string batchName) : base(dataProvider, batchName)
		{
		}

		public override bool IsSnapshotCompatible(SubscriptionSnapshot subscriptionSnapshot, MigrationJobItem migrationJobItem)
		{
			if (!base.IsSnapshotCompatible(subscriptionSnapshot, migrationJobItem))
			{
				return false;
			}
			SyncSubscriptionSnapshot syncSubscriptionSnapshot = subscriptionSnapshot as SyncSubscriptionSnapshot;
			IMAPJobItemSubscriptionSettings imapjobItemSubscriptionSettings = migrationJobItem.SubscriptionSettings as IMAPJobItemSubscriptionSettings;
			if (syncSubscriptionSnapshot == null)
			{
				return false;
			}
			if (syncSubscriptionSnapshot.Protocol != SyncProtocol.Imap)
			{
				return false;
			}
			if (imapjobItemSubscriptionSettings != null)
			{
				if (!string.Equals(imapjobItemSubscriptionSettings.Username, syncSubscriptionSnapshot.UserName))
				{
					return false;
				}
				if (!string.Equals(syncSubscriptionSnapshot.EmailAddress.ToString(), migrationJobItem.Identifier))
				{
					return false;
				}
			}
			return true;
		}

		protected override void ApplySubscriptionSettings(NewSyncRequestCommandBase command, string identifier, IMailboxData localMailbox, ISubscriptionSettings endpointSettings, ISubscriptionSettings jobSettings, ISubscriptionSettings jobItemSettings)
		{
			ImapEndpoint imapEndpoint = endpointSettings as ImapEndpoint;
			IMAPPAWJobSubscriptionSettings imappawjobSubscriptionSettings = jobSettings as IMAPPAWJobSubscriptionSettings;
			IMAPJobItemSubscriptionSettings imapjobItemSubscriptionSettings = jobItemSettings as IMAPJobItemSubscriptionSettings;
			MigrationUtil.AssertOrThrow(endpointSettings == null || imapEndpoint != null, "endpoint if passed in, needs to be an Imap endpoint", new object[0]);
			MigrationUtil.AssertOrThrow(imapjobItemSubscriptionSettings != null, "Must provide IMAP job-item settings", new object[0]);
			if (imapEndpoint != null)
			{
				command.RemoteServerName = imapEndpoint.RemoteServer;
				command.RemoteServerPort = imapEndpoint.Port;
				command.Security = imapEndpoint.Security;
				command.Authentication = ((imapEndpoint.Authentication == IMAPAuthenticationMechanism.Ntlm) ? AuthenticationMethod.Ntlm : AuthenticationMethod.Basic);
			}
			if (imappawjobSubscriptionSettings != null)
			{
				DateTime? startAfter = (DateTime?)imappawjobSubscriptionSettings.StartAfter;
				if (startAfter != null && startAfter.Value == DateTime.MinValue)
				{
					startAfter = null;
				}
				if (startAfter != null)
				{
					command.StartAfter = startAfter;
				}
				DateTime? completeAfter = (DateTime?)imappawjobSubscriptionSettings.CompleteAfter;
				if (completeAfter != null)
				{
					command.CompleteAfter = completeAfter;
				}
				else
				{
					command.CompleteAfter = new DateTime?(DateTime.MinValue);
				}
			}
			if (imapjobItemSubscriptionSettings != null)
			{
				command.Password = MigrationServiceFactory.Instance.GetCryptoAdapter().EncryptedStringToSecureString(imapjobItemSubscriptionSettings.EncryptedPassword);
			}
			NewSyncRequestCommand newSyncRequestCommand = command as NewSyncRequestCommand;
			if (newSyncRequestCommand != null)
			{
				newSyncRequestCommand.Mailbox = localMailbox.GetIdParameter<MailboxIdParameter>();
				newSyncRequestCommand.Name = base.BatchName + ":" + identifier;
				newSyncRequestCommand.RemoteEmailAddress = SmtpAddress.Parse(identifier);
				newSyncRequestCommand.Imap = true;
				newSyncRequestCommand.IncrementalSyncInterval = MRSImapSyncRequestAccessor.IncrementalSyncInterval;
				newSyncRequestCommand.UserName = imapjobItemSubscriptionSettings.Username;
				newSyncRequestCommand.WorkloadType = RequestWorkloadType.Onboarding;
			}
		}

		public static readonly TimeSpan IncrementalSyncInterval = TimeSpan.FromDays(1.0);
	}
}
