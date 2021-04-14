using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum MrsCmdlet
	{
		UpdateMovedMailbox = 1,
		SetOrganization,
		SetConsumerMailbox,
		GetMailbox,
		GetPublicFolderMoveRequest,
		GetMoveRequest
	}
}
