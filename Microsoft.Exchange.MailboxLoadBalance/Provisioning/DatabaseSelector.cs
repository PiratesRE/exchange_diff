using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DatabaseSelector
	{
		public DatabaseSelector(ILogger logger)
		{
			this.logger = logger;
		}

		public MailboxProvisioningResult GetDatabase(MailboxProvisioningData provisioningData)
		{
			MailboxProvisioningResult result;
			using (OperationTracker.Create(this.logger, "Selecting database for '{0}' using {1}.", new object[]
			{
				provisioningData,
				base.GetType().Name
			}))
			{
				MailboxProvisioningResult mailboxProvisioningResult = new MailboxProvisioningResult
				{
					Status = MailboxProvisioningResultStatus.ConstraintCouldNotBeSatisfied,
					MailboxProvisioningConstraints = provisioningData.MailboxProvisioningConstraints
				};
				IMailboxProvisioningConstraints provisioningConstraint = provisioningData.MailboxProvisioningConstraints ?? new MailboxProvisioningConstraints();
				IEnumerable<LoadContainer> databasesMatchingConstraint = this.GetDatabasesMatchingConstraint(provisioningConstraint);
				List<LoadContainer> list = new List<LoadContainer>(300);
				foreach (LoadContainer loadContainer in databasesMatchingConstraint)
				{
					mailboxProvisioningResult.Status = MailboxProvisioningResultStatus.InsufficientCapacity;
					LoadMetric loadMetric;
					long num;
					long num2;
					if (!loadContainer.CanAcceptRegularLoad)
					{
						this.logger.Log(MigrationEventType.Instrumentation, "Database '{0}' cannot accept regular load, skipping.", new object[]
						{
							loadContainer.DirectoryObjectIdentity
						});
					}
					else if (!loadContainer.AvailableCapacity.SupportsAdditional(provisioningData.ConsumedLoad, out loadMetric, out num, out num2))
					{
						this.logger.Log(MigrationEventType.Instrumentation, "Database '{0}' does not have sufficient capacity for the provisioning request. The {1} requested units of {2} would exceed the {3} available. Skipped.", new object[]
						{
							loadContainer.DirectoryObjectIdentity,
							num,
							loadMetric,
							num2
						});
					}
					else
					{
						list.Add(loadContainer);
					}
				}
				if (list.Any<LoadContainer>())
				{
					LoadContainer loadContainer2 = list[new Random().Next(0, list.Count)];
					mailboxProvisioningResult.Status = MailboxProvisioningResultStatus.Valid;
					mailboxProvisioningResult.Database = loadContainer2.DirectoryObjectIdentity;
					loadContainer2.CommittedLoad += provisioningData.ConsumedLoad;
					this.logger.Log(MigrationEventType.Instrumentation, "Selected database {0} with {1} max, {2} consumed and {3} available.", new object[]
					{
						loadContainer2.DirectoryObjectIdentity,
						loadContainer2.MaximumLoad,
						loadContainer2.ConsumedLoad,
						loadContainer2.AvailableCapacity
					});
				}
				result = mailboxProvisioningResult;
			}
			return result;
		}

		protected virtual IEnumerable<LoadContainer> GetAvailableDatabases()
		{
			yield break;
		}

		protected IEnumerable<LoadContainer> GetDatabasesMatchingConstraint(IMailboxProvisioningConstraints provisioningConstraint)
		{
			foreach (LoadContainer container in this.GetAvailableDatabases())
			{
				DirectoryDatabase database = container.DirectoryObject as DirectoryDatabase;
				if (database != null && provisioningConstraint.IsMatch(database.MailboxProvisioningAttributes))
				{
					yield return container;
				}
			}
			yield break;
		}

		private readonly ILogger logger;
	}
}
