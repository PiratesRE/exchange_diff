using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.Drain
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DatabaseDrainRequest : BaseRequest
	{
		public DatabaseDrainRequest(DirectoryDatabase directoryDatabase, MoveInjector moveInjector, LoadBalanceAnchorContext serviceContext, BatchName batchName)
		{
			AnchorUtil.ThrowOnNullArgument(directoryDatabase, "directoryDatabase");
			AnchorUtil.ThrowOnNullArgument(moveInjector, "moveInjector");
			AnchorUtil.ThrowOnNullArgument(serviceContext, "serviceContext");
			AnchorUtil.ThrowOnNullArgument(batchName, "batchName");
			this.directoryDatabase = directoryDatabase;
			this.moveInjector = moveInjector;
			this.serviceContext = serviceContext;
			this.logger = serviceContext.Logger;
			this.batchName = batchName;
		}

		public event Action<DirectoryDatabase> OnDrainFinished;

		protected override void ProcessRequest()
		{
			this.logger.LogInformation("Starting to drain database {0} with batch name {1}.", new object[]
			{
				this.directoryDatabase.Identity,
				this.batchName
			});
			IOperationRetryManager operationRetryManager = LoadBalanceOperationRetryManager.Create(this.logger);
			using (OperationTracker.Create(this.logger, "Moving mailboxes out of {0}.", new object[]
			{
				this.directoryDatabase.Identity
			}))
			{
				foreach (DirectoryMailbox mailboxToMove2 in this.directoryDatabase.GetMailboxes())
				{
					DirectoryMailbox mailboxToMove = mailboxToMove2;
					operationRetryManager.TryRun(delegate
					{
						this.moveInjector.InjectMoveForMailbox(mailboxToMove, this.batchName);
					});
				}
			}
			this.logger.LogInformation("Draining database {0}: Cleaning soft deleted mailboxes.", new object[]
			{
				this.directoryDatabase.Identity
			});
			using (OperationTracker.Create(this.logger, "Starting soft deleted cleanup for {0}.", new object[]
			{
				this.directoryDatabase.Identity
			}))
			{
				this.serviceContext.CleanupSoftDeletedMailboxesOnDatabase(this.directoryDatabase.Identity, ByteQuantifiedSize.Zero);
			}
			this.logger.LogInformation("Finished processing the draining of database {0}.", new object[]
			{
				this.directoryDatabase.Identity
			});
			if (this.OnDrainFinished != null)
			{
				this.OnDrainFinished(this.directoryDatabase);
			}
		}

		private readonly BatchName batchName;

		private readonly DirectoryDatabase directoryDatabase;

		private readonly ILogger logger;

		private readonly MoveInjector moveInjector;

		private readonly LoadBalanceAnchorContext serviceContext;
	}
}
