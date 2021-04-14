using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	internal class GetMoveRequestStatistics : CmdletExecutionRequest<MoveRequestStatistics>
	{
		public GetMoveRequestStatistics(DirectoryMailbox mailbox, ILogger logger, CmdletExecutionPool cmdletPool) : base("Get-MoveRequestStatistics", cmdletPool, logger)
		{
			this.Mailbox = mailbox;
			AnchorUtil.ThrowOnNullArgument(mailbox, "mailbox");
			string value;
			if (mailbox.OrganizationId == TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg)
			{
				value = string.Format("{0}", mailbox.Guid);
			}
			else
			{
				value = string.Format("{0}\\{1}", mailbox.OrganizationId, mailbox.Guid);
			}
			base.Command.AddParameter("Identity", value);
		}

		public DirectoryMailbox Mailbox { get; private set; }
	}
}
