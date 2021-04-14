using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedMailboxRemovalRequest : CmdletExecutionRequest<object>
	{
		public SoftDeletedMailboxRemovalRequest(Guid mailboxGuid, Guid databaseGuid, ILogger logger, CmdletExecutionPool cmdletPool, ILoadBalanceSettings settings) : base("Remove-StoreMailbox", cmdletPool, logger)
		{
			this.mailboxGuid = mailboxGuid;
			this.databaseGuid = databaseGuid;
			this.settings = settings;
			base.Command.AddParameter("Identity", mailboxGuid);
			base.Command.AddParameter("Database", databaseGuid.ToString());
			base.Command.AddParameter("MailboxState", "SoftDeleted");
		}

		protected override RequestDiagnosticData CreateDiagnosticData()
		{
			return new SoftDeletedMailboxRemovalDiagnosticData(this.databaseGuid, this.mailboxGuid);
		}

		protected override void ProcessRequest()
		{
			if (this.settings.DontRemoveSoftDeletedMailboxes)
			{
				base.Command.AddParameter("WhatIf");
			}
			base.ProcessRequest();
		}

		private const string RemoveStoreMailboxCmdletName = "Remove-StoreMailbox";

		private const string SoftDeletedMailboxStateName = "SoftDeleted";

		private readonly Guid databaseGuid;

		private readonly ILoadBalanceSettings settings;

		private readonly Guid mailboxGuid;
	}
}
