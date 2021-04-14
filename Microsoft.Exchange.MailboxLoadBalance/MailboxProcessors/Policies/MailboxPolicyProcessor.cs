using System;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Logging;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors.Policies
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxPolicyProcessor : MailboxProcessor
	{
		public MailboxPolicyProcessor(ILogger logger, IGetMoveInfo getMoveInfo, MoveInjector moveInjector, IMailboxPolicy[] policy) : base(logger)
		{
			this.getMoveInfo = getMoveInfo;
			this.moveInjector = moveInjector;
			this.policy = policy;
		}

		public override bool RequiresRunspace
		{
			get
			{
				return true;
			}
		}

		public override void ProcessMailbox(DirectoryMailbox mailbox, IAnchorRunspaceProxy runspace)
		{
			DirectoryDatabase directoryDatabase = (DirectoryDatabase)mailbox.Parent;
			IMailboxPolicy violatedPolicy = this.GetViolatedPolicy(mailbox, directoryDatabase);
			if (violatedPolicy == null)
			{
				return;
			}
			MoveInfo info = this.getMoveInfo.GetInfo(mailbox, runspace);
			ProvisioningConstraintFixStateLog.Write(mailbox, directoryDatabase, info);
			switch (info.Status)
			{
			case MoveStatus.MoveExistsNotInProgress:
				violatedPolicy.HandleExistingButNotInProgressMove(mailbox, directoryDatabase);
				return;
			case MoveStatus.MoveExistsInProgress:
				break;
			case MoveStatus.MoveDoesNotExist:
				this.InjectMoveRequest(mailbox, violatedPolicy);
				break;
			default:
				return;
			}
		}

		public override bool ShouldProcess(DirectoryMailbox mailbox)
		{
			DirectoryDatabase database = (DirectoryDatabase)mailbox.Parent;
			return database != null && this.policy.Any((IMailboxPolicy p) => p.IsMailboxOutOfPolicy(mailbox, database));
		}

		private IMailboxPolicy GetViolatedPolicy(DirectoryMailbox mailbox, DirectoryDatabase database)
		{
			return this.policy.FirstOrDefault((IMailboxPolicy p) => p.IsMailboxOutOfPolicy(mailbox, database));
		}

		private void InjectMoveRequest(DirectoryMailbox mailbox, IMailboxPolicy violatedPolicy)
		{
			BatchName batchName = violatedPolicy.GetBatchName();
			if (batchName == null)
			{
				throw new InvalidOperationException("Batch names should never be null.");
			}
			base.Logger.Log(MigrationEventType.Information, "Starting a new load balancing procedure to fix provisioning constraint, batch name will be '{0}'", new object[]
			{
				batchName
			});
			this.moveInjector.InjectMoveForMailbox(mailbox, batchName);
		}

		private readonly IGetMoveInfo getMoveInfo;

		private readonly MoveInjector moveInjector;

		private readonly IMailboxPolicy[] policy;
	}
}
