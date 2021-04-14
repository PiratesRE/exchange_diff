using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors.Policies
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IMailboxPolicy
	{
		BatchName GetBatchName();

		bool IsMailboxOutOfPolicy(DirectoryMailbox mailbox, DirectoryDatabase currentDatabase);

		void HandleExistingButNotInProgressMove(DirectoryMailbox mailbox, DirectoryDatabase database);
	}
}
