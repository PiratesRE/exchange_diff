using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging
{
	internal class ProvisioningConstraintFixStateLogEntry
	{
		public Guid MailboxGuid { get; set; }

		public Guid ExistingMoveRequestGuid { get; set; }

		public MoveStatus ExistingMoveStatus { get; set; }

		public Guid SourceDatabaseGuid { get; set; }

		public MailboxProvisioningAttributes SourceDatabaseProvisioningAttributes { get; set; }

		public IMailboxProvisioningConstraint MailboxProvisioningHardConstraint { get; set; }
	}
}
