using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Drain
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DatabaseDrainControl
	{
		public DatabaseDrainControl(LoadBalanceAnchorContext serviceContext)
		{
			this.serviceContext = serviceContext;
			this.inProgressDrainRequests = new ConcurrentDictionary<DirectoryIdentity, BatchName>(12, 12);
		}

		public virtual BatchName BeginDrainDatabase(DirectoryDatabase database)
		{
			BatchName batchName = BatchName.CreateDrainBatch(database.Identity);
			DatabaseDrainRequest databaseDrainRequest = new DatabaseDrainRequest(database, this.serviceContext.MoveInjector, this.serviceContext, batchName);
			if (!this.inProgressDrainRequests.TryAdd(database.Identity, batchName))
			{
				return this.inProgressDrainRequests[database.Identity];
			}
			databaseDrainRequest.OnDrainFinished += this.DatabaseDrainFinished;
			this.serviceContext.QueueManager.GetProcessingQueue(database).EnqueueRequest(databaseDrainRequest);
			return batchName;
		}

		protected void DatabaseDrainFinished(DirectoryDatabase database)
		{
			BatchName batchName;
			if (this.inProgressDrainRequests.TryRemove(database.Identity, out batchName))
			{
				this.serviceContext.Logger.LogVerbose("Draining processing for database '{0}' using batch name '{1}' has completed.", new object[]
				{
					database.Identity,
					batchName
				});
				return;
			}
			this.serviceContext.Logger.LogWarning("Received a signal that database {0} finished draining, but the database wasn't tracked.", new object[]
			{
				database.Identity
			});
		}

		private readonly LoadBalanceAnchorContext serviceContext;

		private readonly ConcurrentDictionary<DirectoryIdentity, BatchName> inProgressDrainRequests;
	}
}
