using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Constraints;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.Provisioning;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MoveInjector
	{
		public MoveInjector(LoadBalanceAnchorContext serviceContext)
		{
			this.serviceContext = serviceContext;
			this.queueManager = serviceContext.QueueManager;
			this.settings = serviceContext.Settings;
			this.directoryProvider = serviceContext.Directory;
			this.logger = serviceContext.Logger;
			this.clientFactory = serviceContext.ClientFactory;
			this.databaseSelector = serviceContext.DatabaseSelector;
		}

		public virtual void InjectMoveForMailbox(DirectoryMailbox mailbox, BatchName batchName)
		{
			if (LoadBalanceADSettings.Instance.Value.UseDatabaseSelectorForMoveInjection)
			{
				LoadEntity loadEntity = this.serviceContext.GetTopologyExtractorFactoryContext().GetEntitySelectorFactory().GetExtractor(mailbox).ExtractEntity();
				MailboxProvisioningResult database = this.databaseSelector.GetDatabase(new MailboxProvisioningData(mailbox.PhysicalSize, mailbox.MailboxProvisioningConstraints, loadEntity.ConsumedLoad));
				database.ValidateSelection();
				DirectoryIdentity database2 = database.Database;
				DirectoryDatabase database3 = (DirectoryDatabase)this.directoryProvider.GetDirectoryObject(database2);
				using (IInjectorService injectorClientForDatabase = this.clientFactory.GetInjectorClientForDatabase(database3))
				{
					injectorClientForDatabase.InjectSingleMove(database2.Guid, batchName.ToString(), new LoadEntity(mailbox));
					return;
				}
			}
			IRequest request = mailbox.CreateRequestToMove(null, batchName.ToString(), this.logger);
			this.queueManager.MainProcessingQueue.EnqueueRequest(request);
		}

		public void InjectMoves(Guid targetDatabase, BatchName batchName, IList<LoadEntity> loadEntityList, bool throwIfNotValid = false)
		{
			this.logger.Log(MigrationEventType.Information, "Injecting {0} moves into database '{1}' with batch name '{2}'.", new object[]
			{
				loadEntityList.Count,
				targetDatabase,
				batchName
			});
			TopologyExtractorFactoryContextPool topologyExtractorFactoryContextPool = this.serviceContext.TopologyExtractorFactoryContextPool;
			IList<Guid> nonMovableOrgsList = LoadBalanceUtils.GetNonMovableOrgsList(this.settings);
			TopologyExtractorFactoryContext context = topologyExtractorFactoryContextPool.GetContext(this.clientFactory, null, nonMovableOrgsList, this.logger);
			TopologyExtractorFactory entitySelectorFactory = context.GetEntitySelectorFactory();
			LoadContainer database = entitySelectorFactory.GetExtractor(this.directoryProvider.GetDatabase(targetDatabase)).ExtractTopology();
			this.InjectMoves(database, batchName, loadEntityList, throwIfNotValid);
		}

		public void InjectMovesOnCompatibilityMode(LoadContainer targetDatabase, BatchName batchName, IEnumerable<LoadEntity> mailboxes, bool throwIfNotValid)
		{
			IList<LoadEntity> list = (mailboxes as IList<LoadEntity>) ?? mailboxes.ToList<LoadEntity>();
			this.logger.Log(MigrationEventType.Information, "Injecting {0} moves into database '{1}' with batch name '{2}' in backwards compatibility mode.", new object[]
			{
				list.Count,
				targetDatabase.Guid,
				batchName
			});
			this.InjectMoves(targetDatabase, batchName, list, throwIfNotValid);
		}

		protected virtual void InjectMoves(LoadContainer database, BatchName batchName, IEnumerable<LoadEntity> mailboxes, bool throwIfNotValid)
		{
			IRequestQueue injectorQueue = this.queueManager.GetInjectionQueue((DirectoryDatabase)database.DirectoryObject);
			IOperationRetryManager operationRetryManager = LoadBalanceOperationRetryManager.Create(this.logger);
			using (IEnumerator<LoadEntity> enumerator = mailboxes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LoadEntity mailbox = enumerator.Current;
					if (mailbox.DirectoryObject == null)
					{
						this.logger.Log(MigrationEventType.Warning, "Not injecting move for {0} because its DirectoryObject is null", new object[]
						{
							mailbox
						});
					}
					else
					{
						OperationRetryManagerResult operationRetryManagerResult = operationRetryManager.TryRun(delegate
						{
							DirectoryObject directoryObject = mailbox.DirectoryObject;
							ConstraintValidationResult constraintValidationResult = database.Constraint.Accept(mailbox);
							if (constraintValidationResult.Accepted)
							{
								database.CommittedLoad += mailbox.ConsumedLoad;
								if (directoryObject.SupportsMoving)
								{
									DirectoryObject directoryObject2 = database.DirectoryObject;
									IRequest request = directoryObject.CreateRequestToMove(directoryObject2.Identity, batchName.ToString(), this.logger);
									injectorQueue.EnqueueRequest(request);
									return;
								}
								if (throwIfNotValid)
								{
									throw new ObjectCannotBeMovedException(mailbox.DirectoryObjectIdentity.ObjectType.ToString(), mailbox.DirectoryObjectIdentity.ToString());
								}
							}
							else
							{
								this.logger.Log(MigrationEventType.Warning, "Not injecting move for {0} because it violates the target database constraints: {1}", new object[]
								{
									mailbox,
									constraintValidationResult
								});
								if (throwIfNotValid)
								{
									constraintValidationResult.Constraint.ValidateAccepted(mailbox);
								}
							}
						});
						if (!operationRetryManagerResult.Succeeded && throwIfNotValid)
						{
							throw operationRetryManagerResult.Exception;
						}
					}
				}
			}
		}

		private readonly IClientFactory clientFactory;

		private readonly DatabaseSelector databaseSelector;

		private readonly IDirectoryProvider directoryProvider;

		private readonly ILogger logger;

		private readonly IRequestQueueManager queueManager;

		private readonly LoadBalanceAnchorContext serviceContext;

		private readonly ILoadBalanceSettings settings;
	}
}
