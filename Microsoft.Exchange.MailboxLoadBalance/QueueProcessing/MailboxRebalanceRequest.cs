using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxRebalanceRequest : BaseRequest
	{
		public MailboxRebalanceRequest(BandMailboxRebalanceData rebalanceData, LoadBalanceAnchorContext serviceContext, IList<Guid> nonMovableOrgs)
		{
			this.rebalanceData = rebalanceData;
			this.serviceContext = serviceContext;
			this.clientFactory = serviceContext.ClientFactory;
			this.logger = serviceContext.Logger;
			this.directoryProvider = serviceContext.Directory;
			this.nonMovableOrgs = nonMovableOrgs;
		}

		protected override void ProcessRequest()
		{
			using (OperationTracker.Create(this.logger, "Rebalancing {0} from {1} to {2}.", new object[]
			{
				this.rebalanceData.RebalanceInformation,
				this.rebalanceData.SourceDatabase,
				this.rebalanceData.TargetDatabase
			}))
			{
				this.logger.Log(MigrationEventType.Information, "MOVE: Moving {0} from {1} to {2}.", new object[]
				{
					this.rebalanceData.RebalanceInformation,
					this.rebalanceData.SourceDatabase,
					this.rebalanceData.TargetDatabase
				});
				Band[] bands = this.rebalanceData.RebalanceInformation.Metrics.OfType<Band>().ToArray<Band>();
				TopologyExtractorFactory entitySelectorFactory = this.serviceContext.TopologyExtractorFactoryContextPool.GetContext(this.clientFactory, bands, this.nonMovableOrgs, this.logger).GetEntitySelectorFactory();
				DirectoryDatabase database = this.directoryProvider.GetDatabase(this.rebalanceData.SourceDatabase.Guid);
				LoadContainer container = entitySelectorFactory.GetExtractor(database).ExtractTopology();
				IOperationRetryManager operationRetryManager = LoadBalanceOperationRetryManager.Create(this.logger);
				foreach (LoadMetric loadMetric in this.rebalanceData.RebalanceInformation.Metrics)
				{
					EntitySelector selector = loadMetric.GetSelector(container, this.rebalanceData.ConstraintSetIdentity, this.rebalanceData.RebalanceInformation[loadMetric]);
					if (selector.IsEmpty)
					{
						this.logger.Log(MigrationEventType.Information, "Could not find any mailbox for metric {0} in database {1}.", new object[]
						{
							loadMetric,
							this.rebalanceData.SourceDatabase
						});
					}
					else
					{
						this.logger.Log(MigrationEventType.Information, "Found mailboxes matching the metric {0} in database {1}. Requesting the injections.", new object[]
						{
							loadMetric,
							this.rebalanceData.SourceDatabase
						});
						operationRetryManager.TryRun(delegate
						{
							DirectoryDatabase database2 = (DirectoryDatabase)this.directoryProvider.GetDirectoryObject(this.rebalanceData.TargetDatabase.DirectoryObjectIdentity);
							using (IInjectorService injectorClientForDatabase = this.clientFactory.GetInjectorClientForDatabase(database2))
							{
								injectorClientForDatabase.InjectMoves(this.rebalanceData.TargetDatabase.Guid, this.rebalanceData.RebalanceBatchName, selector.GetEntities(this.rebalanceData.TargetDatabase));
							}
						});
					}
				}
			}
		}

		private readonly BandMailboxRebalanceData rebalanceData;

		private readonly LoadBalanceAnchorContext serviceContext;

		private readonly IClientFactory clientFactory;

		private readonly ILogger logger;

		private readonly IList<Guid> nonMovableOrgs;

		private readonly IDirectoryProvider directoryProvider;
	}
}
