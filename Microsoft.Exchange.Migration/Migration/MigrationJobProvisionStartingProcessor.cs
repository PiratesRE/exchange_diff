using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class MigrationJobProvisionStartingProcessor : JobProcessor
	{
		protected virtual bool IsProvisioningSupported
		{
			get
			{
				return base.Job.IsProvisioningSupported;
			}
		}

		internal static MigrationJobProvisionStartingProcessor CreateProcessor(MigrationType type)
		{
			if (type <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (type != MigrationType.IMAP)
				{
					if (type != MigrationType.ExchangeOutlookAnywhere)
					{
						goto IL_38;
					}
					return new ExchangeJobProvisionStartingProcessor();
				}
			}
			else if (type != MigrationType.ExchangeRemoteMove && type != MigrationType.ExchangeLocalMove)
			{
				goto IL_38;
			}
			throw new ArgumentException("MigrationType does not support Provisioning!: " + type);
			IL_38:
			throw new ArgumentException("Invalid MigrationType " + type);
		}

		internal override bool Validate()
		{
			return true;
		}

		internal override MigrationJobStatus GetNextStageStatus()
		{
			return MigrationJobStatus.SyncStarting;
		}

		protected sealed override LegacyMigrationJobProcessorResponse Process(bool scheduleNewWork)
		{
			if (scheduleNewWork)
			{
				this.AutoCancelIfTooManyErrors();
			}
			return base.ProcessActions(scheduleNewWork, new Func<bool, LegacyMigrationJobProcessorResponse>[]
			{
				new Func<bool, LegacyMigrationJobProcessorResponse>(this.ProcessPendingJobItems)
			});
		}

		protected virtual IProvisioningHandler GetProvisioningHandler()
		{
			if (this.IsProvisioningSupported)
			{
				return MigrationApplication.ProvisioningHandler;
			}
			return null;
		}

		protected abstract IProvisioningData GetProvisioningData(MigrationJobItem jobItem);

		protected virtual void HandleProvisioningCompletedEvent(ProvisionedObject provisionedObj, MigrationJobItem jobItem)
		{
			if (provisionedObj.Succeeded)
			{
				MigrationUserStatus value = MigrationUserStatus.Queued;
				jobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(value), (MailboxData)provisionedObj.MailboxData, null, new ExDateTime?(ExDateTime.UtcNow));
				return;
			}
			jobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.Failed), (MailboxData)provisionedObj.MailboxData, new ProvisioningFailedException(new LocalizedString(provisionedObj.Error)), null);
		}

		protected virtual void HandleNullIProvisioningDataEvent(MigrationJobItem jobItem)
		{
			jobItem.SetStatusAndSubscriptionLastChecked(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.Failed), new UserProvisioningInternalException(), null, null);
		}

		private LegacyMigrationJobProcessorResponse ProcessPendingJobItems(bool scheduleNewWork)
		{
			if (!this.IsProvisioningSupported)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} no provisioning supported, so all process pending is finished", new object[]
				{
					base.Job
				});
				return LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			}
			int jobItemCount = base.GetJobItemCount(new MigrationUserStatus[]
			{
				MigrationUserStatus.Provisioning
			});
			MigrationUserStatus status = (jobItemCount <= 0) ? MigrationUserStatus.ProvisionUpdating : MigrationUserStatus.Provisioning;
			return this.ProcessProvisioningItems(status, scheduleNewWork);
		}

		private MigrationProcessorResult ProcessProvisioningItem(IProvisioningHandler scheduler, MigrationJobItem jobItem, bool scheduleNewWork, ref int scheduledCount)
		{
			if (scheduler.IsItemQueued(jobItem.StoreObjectId))
			{
				if (this.ProcessQueuedItem(scheduler, jobItem))
				{
					return MigrationProcessorResult.Completed;
				}
				return MigrationProcessorResult.Waiting;
			}
			else
			{
				if (!scheduleNewWork)
				{
					return MigrationProcessorResult.Failed;
				}
				if (!scheduler.HasCapacity(base.Job.JobId))
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} doesn't have capacity anymore, will process on next cycle", new object[]
					{
						base.Job
					});
					return MigrationProcessorResult.Failed;
				}
				if (this.QueueItem(scheduler, jobItem))
				{
					scheduledCount++;
					return MigrationProcessorResult.Waiting;
				}
				return MigrationProcessorResult.Completed;
			}
		}

		private LegacyMigrationJobProcessorResponse ProcessProvisioningItems(MigrationUserStatus status, bool scheduleNewWork)
		{
			LegacyMigrationJobProcessorResponse response = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Working, null);
			response.NumItemsProcessed = new int?(0);
			response.NumItemsTransitioned = new int?(0);
			IProvisioningHandler scheduler = this.GetProvisioningHandler();
			this.RegisterJob(scheduler);
			bool foundWork = false;
			int scheduledCount = 0;
			int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MaxItemsToProvisionInOnePass");
			IEnumerable<MigrationJobItem> itemsByStatus = base.Job.GetItemsByStatus(base.DataProvider, status, null);
			using (IEnumerator<MigrationJobItem> enumerator = itemsByStatus.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MigrationJobItem jobItem = enumerator.Current;
					MigrationProcessorResult migrationProcessorResult = ItemStateTransitionHelper.RunJobItemOperation(base.Job, jobItem, base.DataProvider, MigrationUserStatus.Failed, delegate
					{
						MigrationProcessorResult migrationProcessorResult2 = this.ProcessProvisioningItem(scheduler, jobItem, scheduleNewWork, ref scheduledCount);
						if (migrationProcessorResult2 != MigrationProcessorResult.Failed)
						{
							response.NumItemsProcessed++;
							foundWork = true;
							if (migrationProcessorResult2 == MigrationProcessorResult.Completed)
							{
								response.NumItemsTransitioned++;
							}
						}
					});
					if (migrationProcessorResult == MigrationProcessorResult.Waiting)
					{
						foundWork = true;
					}
					if (migrationProcessorResult == MigrationProcessorResult.Failed)
					{
						response.NumItemsTransitioned++;
					}
					if (scheduledCount > config)
					{
						MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} provisioned too many in one pass {1}, max {2}", new object[]
						{
							base.Job,
							scheduledCount,
							config
						});
						break;
					}
				}
			}
			if (!foundWork)
			{
				scheduler.UnregisterJob(base.Job.JobId);
				response.Result = MigrationProcessorResult.Completed;
			}
			else
			{
				response.NumItemsOutstanding = new int?(base.GetJobItemCount(new MigrationUserStatus[]
				{
					status
				}));
			}
			return response;
		}

		private void RegisterJob(IProvisioningHandler scheduler)
		{
			if (scheduler.IsJobRegistered(base.Job.JobId))
			{
				return;
			}
			Guid jobId = base.Job.JobId;
			CultureInfo adminCulture = base.Job.AdminCulture;
			Guid ownerExchangeObjectId = base.Job.OwnerExchangeObjectId;
			ADObjectId ownerId = base.Job.OwnerId;
			DelegatedPrincipal delegatedAdminOwner = base.Job.DelegatedAdminOwner;
			if (ownerId == null && delegatedAdminOwner == null)
			{
				throw MigrationHelperBase.CreatePermanentExceptionWithInternalData<MigrationUnknownException>("Cannot do provisioning since both owner id and delegated admin are null");
			}
			scheduler.RegisterJob(jobId, adminCulture, ownerExchangeObjectId, ownerId, delegatedAdminOwner, base.Job.SubmittedByUserAdminType, base.DataProvider.ADProvider.TenantOrganizationName, base.DataProvider.OrganizationId);
		}

		private bool ProcessQueuedItem(IProvisioningHandler scheduler, MigrationJobItem jobItem)
		{
			MigrationUserStatus status = jobItem.Status;
			if (scheduler.IsItemCompleted(jobItem.StoreObjectId))
			{
				ProvisionedObject provisionedObj = scheduler.DequeueItem(jobItem.StoreObjectId);
				this.HandleProvisioningCompletedEvent(provisionedObj, jobItem);
			}
			else if (base.Job.IsCancelled)
			{
				scheduler.CancelItem(jobItem.StoreObjectId);
			}
			return status != jobItem.Status;
		}

		private bool QueueItem(IProvisioningHandler scheduler, MigrationJobItem jobItem)
		{
			IProvisioningData provisioningData = this.GetProvisioningData(jobItem);
			if (provisioningData == null)
			{
				this.HandleNullIProvisioningDataEvent(jobItem);
				return false;
			}
			scheduler.QueueItem(base.Job.JobId, jobItem.StoreObjectId, provisioningData);
			return true;
		}
	}
}
