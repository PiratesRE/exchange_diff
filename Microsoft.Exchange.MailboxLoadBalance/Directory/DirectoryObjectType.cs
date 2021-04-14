using System;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	internal enum DirectoryObjectType
	{
		Unknown,
		Forest,
		DatabaseAvailabilityGroup,
		Server,
		Database,
		Mailbox,
		CloudArchive,
		NonConnectedMailbox,
		ConstraintSet,
		ConsumerMailbox
	}
}
