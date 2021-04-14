using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors.Policies
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxProvisioningConstraintPolicy : IMailboxPolicy
	{
		public MailboxProvisioningConstraintPolicy(IEventNotificationSender eventNotificationSender)
		{
			this.eventNotificationSender = eventNotificationSender;
		}

		public BatchName GetBatchName()
		{
			return BatchName.CreateProvisioningConstraintFixBatch();
		}

		public bool IsMailboxOutOfPolicy(DirectoryMailbox mailbox, DirectoryDatabase currentDatabase)
		{
			return mailbox.MailboxProvisioningConstraints != null && mailbox.MailboxProvisioningConstraints.HardConstraint != null && !mailbox.MailboxProvisioningConstraints.IsMatch(currentDatabase.MailboxProvisioningAttributes);
		}

		public void HandleExistingButNotInProgressMove(DirectoryMailbox mailbox, DirectoryDatabase database)
		{
			this.eventNotificationSender.CreateAndPublishMailboxEventNotification(MigrationNotifications.CannotMoveMailboxDueToExistingMoveNotInProgress, mailbox, database);
		}

		private readonly IEventNotificationSender eventNotificationSender;
	}
}
