using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MigrationJobSyncStartingProcessor : JobProcessor
	{
		internal static MigrationJobSyncStartingProcessor CreateProcessor(MigrationType type)
		{
			if (type <= MigrationType.BulkProvisioning)
			{
				if (type != MigrationType.IMAP && type != MigrationType.ExchangeOutlookAnywhere)
				{
					if (type != MigrationType.BulkProvisioning)
					{
						goto IL_37;
					}
					throw new NotSupportedException("Not a valid state for Bulk Provisioning");
				}
			}
			else if (type != MigrationType.ExchangeRemoteMove && type != MigrationType.ExchangeLocalMove && type != MigrationType.PublicFolder)
			{
				goto IL_37;
			}
			return new MigrationJobSyncStartingProcessor();
			IL_37:
			throw new ArgumentException("Invalid MigrationType " + type);
		}

		internal override bool Validate()
		{
			return true;
		}

		internal override MigrationJobStatus GetNextStageStatus()
		{
			if (base.Job.IsCancelled)
			{
				return MigrationJobStatus.SyncCompleted;
			}
			if (!base.Job.AutoComplete)
			{
				return MigrationJobStatus.SyncCompleting;
			}
			if (base.Job.CompleteAfterMoveSyncNotCompleted)
			{
				return MigrationJobStatus.SyncCompleted;
			}
			return MigrationJobStatus.CompletionStarting;
		}

		protected sealed override LegacyMigrationJobProcessorResponse Process(bool scheduleNewWork)
		{
			int jobItemCount = base.GetJobItemCount(MigrationJobSyncStartingProcessor.SyncingJobItemStatusArray);
			int jobItemCount2 = base.GetJobItemCount(MigrationJobSyncStartingProcessor.QueuedJobItemStatusArray);
			LegacyMigrationJobProcessorResponse response = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			if (jobItemCount > 0 || (scheduleNewWork && jobItemCount2 > 0))
			{
				response.NumItemsOutstanding = new int?(0);
				response.NumItemsProcessed = new int?(0);
				response.NumItemsTransitioned = new int?(0);
				bool flag = jobItemCount > 0;
				if (scheduleNewWork)
				{
					base.SlotProvider.UpdateAllocationCounts(base.DataProvider);
					int num = Math.Min(base.SlotProvider.AvailableInitialSeedingSlots.IsUnlimited ? int.MaxValue : base.SlotProvider.AvailableInitialSeedingSlots.Value, ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationPollingBatchSize"));
					MigrationLogger.Log(MigrationEventType.Verbose, "Creating subscriptions. Slots available: {0}", new object[]
					{
						num
					});
					foreach (MigrationJobItem migrationJobItem in base.Job.GetItemsByStatus(base.DataProvider, MigrationUserStatus.Queued, new int?(num)))
					{
						flag = true;
						MigrationJobItem item = migrationJobItem;
						MigrationProcessorResult migrationProcessorResult = ItemStateTransitionHelper.RunJobItemOperation(base.Job, migrationJobItem, base.DataProvider, MigrationUserStatus.Failed, delegate
						{
							using (BasicMigrationSlotProvider.SlotAcquisitionGuard slotAcquisitionGuard = this.SlotProvider.AcquireSlot(item, MigrationSlotType.InitialSeeding, this.DataProvider))
							{
								MigrationLogger.Log(MigrationEventType.Verbose, "Acquired 1 slot to job item, {0} available.", new object[]
								{
									this.SlotProvider.AvailableInitialSeedingSlots
								});
								if (this.SubscriptionHandler.CreateUnderlyingSubscriptions(item))
								{
									response.NumItemsProcessed++;
									response.NumItemsTransitioned++;
									slotAcquisitionGuard.Success();
								}
							}
						});
						if (migrationProcessorResult == MigrationProcessorResult.Failed)
						{
							response.NumItemsTransitioned++;
						}
					}
				}
				if (flag)
				{
					ExDateTime cutoffTime = ExDateTime.UtcNow - ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationInitialSyncStartPollingTimeout");
					int count = Math.Min(base.SlotProvider.MaximumConcurrentMigrations.IsUnlimited ? base.Job.TotalItemCount : base.SlotProvider.MaximumConcurrentMigrations.Value, ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationPollingBatchSize"));
					JobItemOperationResult value = base.SyncJobItems(MigrationJobSyncStartingProcessor.SyncingJobItemStatusArray, MigrationUserStatus.Failed, cutoffTime, count);
					MigrationLogger.Log(MigrationEventType.Verbose, "Polled {0} initial active sync subscriptions", new object[]
					{
						value.NumItemsProcessed
					});
					int count2 = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationPollingBatchSize") - value.NumItemsProcessed;
					cutoffTime = ExDateTime.UtcNow - ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationPollingTimeout");
					value += base.SyncJobItems(MigrationJobSyncStartingProcessor.IncrementalSyncJobtemStatusArray, MigrationUserStatus.Failed, cutoffTime, count2);
					MigrationLogger.Log(MigrationEventType.Verbose, "Polled a total of {0} subscriptions", new object[]
					{
						value.NumItemsProcessed
					});
					response.NumItemsTransitioned += value.NumItemsTransitioned;
					jobItemCount = base.GetJobItemCount(MigrationJobSyncStartingProcessor.SyncingJobItemStatusArray);
				}
				response.NumItemsOutstanding = new int?(base.GetJobItemCount(MigrationJobSyncStartingProcessor.QueuedJobItemStatusArray) + jobItemCount);
				if (jobItemCount >= base.SlotProvider.MaximumConcurrentMigrations || base.SlotProvider.AvailableInitialSeedingSlots <= 0)
				{
					response.Result = MigrationProcessorResult.Waiting;
				}
				else if (response.NumItemsOutstanding.Value > 0)
				{
					response.Result = MigrationProcessorResult.Working;
				}
			}
			if (response.Result == MigrationProcessorResult.Completed && !base.Job.AutoComplete)
			{
				base.Job.ReportData.Append(Strings.MigrationReportJobInitialSyncComplete);
			}
			return response;
		}

		protected override LegacyMigrationJobProcessorResponse StopProcessing()
		{
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			legacyMigrationJobProcessorResponse.NumItemsOutstanding = new int?(0);
			JobItemOperationResult jobItemOperationResult = base.FindAndRunJobItemOperation(MigrationJobSyncStartingProcessor.ProcessedJobItemStatusArray, MigrationUserStatus.Failed, ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationPollingBatchSize"), (MigrationUserStatus status, int itemCount) => base.Job.GetItemsByStatus(base.DataProvider, status, new int?(itemCount)), delegate(MigrationJobItem item)
			{
				base.SubscriptionHandler.CancelUnderlyingSubscriptions(item);
				return true;
			});
			legacyMigrationJobProcessorResponse.NumItemsProcessed = new int?(jobItemOperationResult.NumItemsProcessed);
			legacyMigrationJobProcessorResponse.NumItemsTransitioned = new int?(jobItemOperationResult.NumItemsTransitioned);
			legacyMigrationJobProcessorResponse.NumItemsOutstanding = new int?(base.GetJobItemCount(MigrationJobSyncStartingProcessor.InitialSyncingJobItemStatusArray));
			if (legacyMigrationJobProcessorResponse.NumItemsProcessed > 0 || legacyMigrationJobProcessorResponse.NumItemsOutstanding > 0)
			{
				legacyMigrationJobProcessorResponse.Result = MigrationProcessorResult.Working;
			}
			return legacyMigrationJobProcessorResponse;
		}

		protected IEnumerable<MigrationJobItem> GetJobItemsForSubscriptionCreation(int maxCount)
		{
			if (maxCount > 0)
			{
				return base.Job.GetItemsByStatus(base.DataProvider, MigrationUserStatus.Queued, new int?(maxCount));
			}
			return new List<MigrationJobItem>(0);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationJobSyncStartingProcessor>(this);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MigrationJobSyncStartingProcessor()
		{
			MigrationUserStatus[] queuedJobItemStatusArray = new MigrationUserStatus[1];
			MigrationJobSyncStartingProcessor.QueuedJobItemStatusArray = queuedJobItemStatusArray;
			MigrationJobSyncStartingProcessor.IncrementalSyncJobtemStatusArray = new MigrationUserStatus[]
			{
				MigrationUserStatus.Synced,
				MigrationUserStatus.IncrementalFailed
			};
		}

		private static readonly MigrationUserStatus[] SyncingJobItemStatusArray = new MigrationUserStatus[]
		{
			MigrationUserStatus.Syncing,
			MigrationUserStatus.IncrementalSyncing
		};

		private static readonly MigrationUserStatus[] InitialSyncingJobItemStatusArray = new MigrationUserStatus[]
		{
			MigrationUserStatus.Syncing
		};

		private static readonly MigrationUserStatus[] ProcessedJobItemStatusArray = new MigrationUserStatus[]
		{
			MigrationUserStatus.Queued,
			MigrationUserStatus.Syncing,
			MigrationUserStatus.IncrementalSyncing
		};

		private static readonly MigrationUserStatus[] QueuedJobItemStatusArray;

		private static readonly MigrationUserStatus[] IncrementalSyncJobtemStatusArray;
	}
}
