using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal abstract class JobProcessor : DisposeTrackableBase
	{
		protected JobProcessor()
		{
			this.slotProvider = new Lazy<BasicMigrationSlotProvider>(new Func<BasicMigrationSlotProvider>(this.InitializeSlotProvider), LazyThreadSafetyMode.ExecutionAndPublication);
		}

		internal virtual bool SupportsInterrupting
		{
			get
			{
				return false;
			}
		}

		private protected IMigrationDataProvider DataProvider { protected get; private set; }

		private protected MigrationSession Session { protected get; private set; }

		private protected MigrationJob Job { protected get; private set; }

		protected ILegacySubscriptionHandler SubscriptionHandler
		{
			get
			{
				if (this.subscriptionHandler == null)
				{
					this.subscriptionHandler = LegacySubscriptionHandlerBase.CreateSubscriptionHandler(this.DataProvider, this.Job);
				}
				return this.subscriptionHandler;
			}
		}

		protected BasicMigrationSlotProvider SlotProvider
		{
			get
			{
				return this.slotProvider.Value;
			}
		}

		internal static JobProcessor CreateJobProcessor(MigrationJob job)
		{
			MigrationUtil.ThrowOnNullArgument(job, "job");
			switch (job.Status)
			{
			case MigrationJobStatus.SyncInitializing:
				return JobSyncInitializingProcessor.CreateProcessor(job.MigrationType);
			case MigrationJobStatus.SyncStarting:
				return MigrationJobSyncStartingProcessor.CreateProcessor(job.MigrationType);
			case MigrationJobStatus.SyncCompleting:
				return MigrationJobSyncCompletingProcessor.CreateProcessor(job.MigrationType);
			case MigrationJobStatus.SyncCompleted:
				return MigrationJobSyncCompletedProcessor.CreateProcessor(job.MigrationType);
			case MigrationJobStatus.CompletionInitializing:
				return MigrationJobCompletionInitializingProcessor.CreateProcessor(job.MigrationType);
			case MigrationJobStatus.CompletionStarting:
				return MigrationJobCompletionStartingProcessor.CreateProcessor(job.MigrationType);
			case MigrationJobStatus.Completing:
				return MigrationJobCompletingProcessor.CreateProcessor(job.MigrationType, job.IsStaged);
			case MigrationJobStatus.Completed:
				return MigrationJobCompletedProcessor.CreateProcessor(job.MigrationType, job.SupportsMultiBatchFinalization);
			case MigrationJobStatus.Removing:
				return MigrationJobRemovingProcessor.CreateProcessor(job.MigrationType);
			case MigrationJobStatus.ProvisionStarting:
				return MigrationJobProvisionStartingProcessor.CreateProcessor(job.MigrationType);
			case MigrationJobStatus.Validating:
				return MigrationJobValidatingProcessor.CreateProcessor(job.MigrationType);
			case MigrationJobStatus.Stopped:
				return MigrationJobStoppedProcessor.CreateProcessor(job.MigrationType);
			}
			return null;
		}

		internal void Initialize(IMigrationDataProvider dataProvider, MigrationSession session, MigrationJob job)
		{
			this.DataProvider = dataProvider;
			this.Session = session;
			this.Job = job;
		}

		internal abstract bool Validate();

		internal abstract MigrationJobStatus GetNextStageStatus();

		internal virtual void OnComplete()
		{
		}

		internal LegacyMigrationJobProcessorResponse Process()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse;
			if (this.ShouldStopProcessing())
			{
				legacyMigrationJobProcessorResponse = this.StopProcessing();
			}
			else
			{
				legacyMigrationJobProcessorResponse = this.Process(true);
			}
			this.RunFullScan(legacyMigrationJobProcessorResponse);
			if (this.slotProvider.IsValueCreated && this.SlotProvider != BasicMigrationSlotProvider.Unlimited)
			{
				MigrationEndpointBase migrationEndpointBase = this.Job.SourceEndpoint ?? this.Job.TargetEndpoint;
				MigrationUtil.AssertOrThrow(migrationEndpointBase != null, "Endpoint should be non-null or unlimited after processing.", new object[0]);
				using (IMigrationDataProvider providerForFolder = this.DataProvider.GetProviderForFolder(MigrationFolderName.Settings))
				{
					using (IMigrationMessageItem migrationMessageItem = providerForFolder.FindMessage(migrationEndpointBase.StoreObjectId, BasicMigrationSlotProvider.PropertyDefinition))
					{
						migrationMessageItem.OpenAsReadWrite();
						this.SlotProvider.WriteCachedCountsToMessageItem(migrationMessageItem);
						migrationMessageItem.Save(SaveMode.ResolveConflicts);
					}
				}
			}
			MigrationLogger.Log(MigrationEventType.Information, "Job type {0}, status {1}, result {2}, length {3}, job {4}", new object[]
			{
				this.Job.MigrationType,
				this.Job.Status,
				legacyMigrationJobProcessorResponse.Result,
				stopwatch.Elapsed.TotalSeconds,
				this.Job
			});
			return legacyMigrationJobProcessorResponse;
		}

		protected abstract LegacyMigrationJobProcessorResponse Process(bool scheduleNewWork);

		protected virtual void AutoCancelIfTooManyErrors()
		{
			if (this.Job.IsCancelled)
			{
				return;
			}
			int jobItemCount = this.GetJobItemCount(new MigrationUserStatus[]
			{
				MigrationUserStatus.Failed
			});
			if (jobItemCount < ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationMinimumFailureCountForAutoCancel"))
			{
				return;
			}
			if (jobItemCount >= ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationAbsoluteFailureCountForAutoCancel"))
			{
				this.Job.StopJob(this.DataProvider, this.Session.Config, JobCancellationStatus.CancelledDueToHighFailureCount);
				return;
			}
			int totalItemCount = this.Job.TotalItemCount;
			if (totalItemCount > 0)
			{
				double num = (double)(ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationFailureRatioForAutoCancel") / 100);
				double num2 = (double)jobItemCount / (double)totalItemCount;
				if (num2 > num)
				{
					this.Job.StopJob(this.DataProvider, this.Session.Config, JobCancellationStatus.CancelledDueToHighFailureCount);
				}
			}
		}

		protected virtual bool ShouldStopProcessing()
		{
			return this.Job.IsCancelled;
		}

		protected virtual LegacyMigrationJobProcessorResponse StopProcessing()
		{
			return this.Process(false);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.subscriptionHandler != null)
				{
					this.subscriptionHandler.Dispose();
				}
				this.subscriptionHandler = null;
			}
		}

		protected void CheckIfJobExceededThreshold(ExDateTime? beginTime, TimeSpan threshold)
		{
			if (beginTime == null)
			{
				beginTime = this.Job.StateLastUpdated;
				if (beginTime == null)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} has no beginning time to check if threshold reached", new object[]
					{
						this.Job
					});
					return;
				}
			}
			TimeSpan timeSpan = new TimeSpan(ExDateTime.UtcNow.UtcTicks - beginTime.Value.UtcTicks);
			if (timeSpan > threshold)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "Job {0} has exceed threshold of {1} with {2}", new object[]
				{
					this.Job,
					threshold,
					timeSpan
				});
				return;
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} has NOT exceed threshold of {1} with {2}", new object[]
			{
				this.Job,
				threshold,
				timeSpan
			});
		}

		protected JobItemOperationResult ResumeJobItems(MigrationUserStatus[] sourceStatuses, MigrationUserStatus targetStatus, MigrationUserStatus targetFailedStatus, ExDateTime? cutoffTime, int count, MigrationSlotType slotType = MigrationSlotType.IncrementalSync)
		{
			MigrationUtil.ThrowOnNullArgument(sourceStatuses, "sourceStatuses");
			return this.FindAndRunJobItemOperation(sourceStatuses, targetFailedStatus, count, (MigrationUserStatus status, int itemCount) => MigrationJobItem.GetMigratableByStateLastUpdated(this.DataProvider, this.Job, cutoffTime, status, itemCount), delegate(MigrationJobItem item)
			{
				bool result;
				using (BasicMigrationSlotProvider.SlotAcquisitionGuard slotAcquisitionGuard = this.SlotProvider.AcquireSlot(item, slotType, this.DataProvider))
				{
					this.SubscriptionHandler.SyncSubscriptionSettings(item);
					this.SubscriptionHandler.ResumeUnderlyingSubscriptions(targetStatus, item);
					slotAcquisitionGuard.Success();
					result = true;
				}
				return result;
			});
		}

		protected JobItemOperationResult SyncJobItems(MigrationUserStatus[] findStatuses, MigrationUserStatus targetFailedStatus, ExDateTime cutoffTime, int count)
		{
			MigrationUtil.ThrowOnNullArgument(findStatuses, "findStatuses");
			return this.FindAndRunJobItemOperation(findStatuses, targetFailedStatus, count, (MigrationUserStatus jobItemStatus, int itemCount) => this.SubscriptionHandler.GetJobItemsForSubscriptionCheck(new ExDateTime?(cutoffTime), jobItemStatus, itemCount), new Func<MigrationJobItem, bool>(this.SyncJobItemToSubscription));
		}

		protected int GetJobItemCount(params MigrationUserStatus[] statuses)
		{
			return MigrationJobItem.GetCount(this.DataProvider, this.Job.JobId, statuses);
		}

		protected int GetJobItemCount(ExDateTime? lastRestartTime, params MigrationUserStatus[] statuses)
		{
			return MigrationJobItem.GetCount(this.DataProvider, this.Job.JobId, lastRestartTime, statuses);
		}

		protected void RunFullScan(LegacyMigrationJobProcessorResponse response)
		{
			if (response.Result == MigrationProcessorResult.Completed || (response.NumItemsTransitioned != null && response.NumItemsTransitioned > 0 && this.Job.ShouldLazyRescan))
			{
				this.Job.UpdateCachedItemCounts(this.DataProvider);
			}
		}

		protected IEnumerable<MigrationJobItem> GetJobItemsByStatus(MigrationUserStatus status, int? maxCount)
		{
			return MigrationJobItem.GetByStatus(this.DataProvider, this.Job, status, maxCount);
		}

		protected JobItemOperationResult FindAndRunJobItemBatchOperation(MigrationUserStatus[] findStatuses, int count, Func<MigrationUserStatus, int, IEnumerable<MigrationJobItem>> jobItemFilter, Func<IEnumerable<MigrationJobItem>, JobItemOperationResult> jobItemBatchOperation)
		{
			MigrationUtil.ThrowOnNullArgument(findStatuses, "findStatuses");
			JobItemOperationResult jobItemOperationResult = default(JobItemOperationResult);
			if (count <= 0)
			{
				return jobItemOperationResult;
			}
			foreach (MigrationUserStatus migrationUserStatus in findStatuses)
			{
				IEnumerable<MigrationJobItem> arg = jobItemFilter(migrationUserStatus, count - jobItemOperationResult.NumItemsProcessed);
				JobItemOperationResult value = jobItemBatchOperation(arg);
				MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} operated on {1} job items for status {2}", new object[]
				{
					this.Job,
					value.NumItemsProcessed,
					migrationUserStatus
				});
				jobItemOperationResult += value;
				if (jobItemOperationResult.NumItemsProcessed >= count)
				{
					break;
				}
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} operated on {1} jobs hope to operate on max of {2}", new object[]
			{
				this.Job,
				jobItemOperationResult.NumItemsProcessed,
				count
			});
			return jobItemOperationResult;
		}

		protected JobItemOperationResult FindAndRunJobItemOperation(MigrationUserStatus[] findStatuses, MigrationUserStatus targetFailedStatus, int count, Func<MigrationUserStatus, int, IEnumerable<MigrationJobItem>> jobItemFilter, Func<MigrationJobItem, bool> jobItemOperation)
		{
			MigrationUtil.ThrowOnNullArgument(findStatuses, "findStatuses");
			return this.FindAndRunJobItemBatchOperation(findStatuses, count, jobItemFilter, delegate(IEnumerable<MigrationJobItem> jobItems)
			{
				JobProcessor.<>c__DisplayClass9.<>c__DisplayClassb <>c__DisplayClassb = new JobProcessor.<>c__DisplayClass9.<>c__DisplayClassb();
				<>c__DisplayClassb.result = default(JobItemOperationResult);
				using (IEnumerator<MigrationJobItem> enumerator = jobItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MigrationJobItem item = enumerator.Current;
						MigrationProcessorResult migrationProcessorResult = ItemStateTransitionHelper.RunJobItemOperation(this.Job, item, this.DataProvider, targetFailedStatus, delegate
						{
							try
							{
								JobProcessor.<>c__DisplayClass9.<>c__DisplayClassb cs$<>8__localsc = <>c__DisplayClassb;
								cs$<>8__localsc.result.NumItemsProcessed = cs$<>8__localsc.result.NumItemsProcessed + 1;
								if (jobItemOperation(item))
								{
									JobProcessor.<>c__DisplayClass9.<>c__DisplayClassb cs$<>8__localsc2 = <>c__DisplayClassb;
									cs$<>8__localsc2.result.NumItemsTransitioned = cs$<>8__localsc2.result.NumItemsTransitioned + 1;
									JobProcessor.<>c__DisplayClass9.<>c__DisplayClassb cs$<>8__localsc3 = <>c__DisplayClassb;
									cs$<>8__localsc3.result.NumItemsSuccessful = cs$<>8__localsc3.result.NumItemsSuccessful + 1;
								}
							}
							catch (MigrationSlotCapacityExceededException exception)
							{
								MigrationLogger.Log(MigrationEventType.Verbose, exception, "Not enough capacity at the slot provider to perform the operation.", new object[0]);
							}
						});
						if (migrationProcessorResult == MigrationProcessorResult.Failed)
						{
							JobProcessor.<>c__DisplayClass9.<>c__DisplayClassb <>c__DisplayClassb2 = <>c__DisplayClassb;
							<>c__DisplayClassb2.result.NumItemsTransitioned = <>c__DisplayClassb2.result.NumItemsTransitioned + 1;
						}
					}
				}
				return <>c__DisplayClassb.result;
			});
		}

		protected bool SyncJobItemToSubscription(MigrationJobItem item)
		{
			this.SubscriptionHandler.SyncSubscriptionSettings(item);
			MigrationSlotType consumedSlotType = item.ConsumedSlotType;
			Guid migrationSlotProviderGuid = item.MigrationSlotProviderGuid;
			MigrationUserStatus status = item.Status;
			this.SubscriptionHandler.SyncToUnderlyingSubscriptions(item);
			if (consumedSlotType != MigrationSlotType.None && migrationSlotProviderGuid != Guid.Empty)
			{
				bool flag = consumedSlotType != item.ConsumedSlotType || migrationSlotProviderGuid != item.MigrationSlotProviderGuid;
				if (flag)
				{
					this.SlotProvider.ReleaseSlot(consumedSlotType);
				}
				else
				{
					bool flag2 = item.Status != MigrationUserStatus.Completing && item.Status != MigrationUserStatus.IncrementalSyncing && item.Status != MigrationUserStatus.Syncing;
					if (flag2 && item.ConsumedSlotType != MigrationSlotType.None && item.MigrationSlotProviderGuid != Guid.Empty)
					{
						this.SlotProvider.ReleaseSlot(item, this.DataProvider);
					}
				}
			}
			if (item.Status != MigrationUserStatus.Syncing && item.Status != MigrationUserStatus.IncrementalSyncing)
			{
				MigrationUserStatus status2 = item.Status;
			}
			return item.Status != status;
		}

		protected LegacyMigrationJobProcessorResponse ProcessActions(bool scheduleNewWork, params Func<bool, LegacyMigrationJobProcessorResponse>[] processorActions)
		{
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (Func<bool, LegacyMigrationJobProcessorResponse> func in processorActions)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse2 = func(scheduleNewWork);
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(";");
				}
				stringBuilder.Append(string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					base.GetType().Name,
					num++,
					stopwatch.Elapsed.TotalSeconds,
					legacyMigrationJobProcessorResponse2.NumItemsProcessed,
					legacyMigrationJobProcessorResponse2.NumItemsOutstanding
				}));
				legacyMigrationJobProcessorResponse = MigrationProcessorResponse.MergeResponses<LegacyMigrationJobProcessorResponse>(legacyMigrationJobProcessorResponse, legacyMigrationJobProcessorResponse2);
				if (legacyMigrationJobProcessorResponse2.Result != MigrationProcessorResult.Completed)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} ran processor {1}, response {2}", new object[]
					{
						this.Job,
						func,
						legacyMigrationJobProcessorResponse
					});
					break;
				}
			}
			legacyMigrationJobProcessorResponse.DebugInfo = stringBuilder.ToString();
			if (legacyMigrationJobProcessorResponse.Result == MigrationProcessorResult.Failed)
			{
				MigrationLogger.Log(MigrationEventType.Information, "Job {0} failed stage, marking it complete", new object[]
				{
					this.Job
				});
				legacyMigrationJobProcessorResponse.Result = MigrationProcessorResult.Completed;
			}
			return legacyMigrationJobProcessorResponse;
		}

		private BasicMigrationSlotProvider InitializeSlotProvider()
		{
			MigrationLogger.Log(MigrationEventType.Instrumentation, "Getting a slot provider for processor {0}", new object[]
			{
				base.GetType().Name
			});
			MigrationEndpointBase migrationEndpointBase = this.Job.SourceEndpoint ?? this.Job.TargetEndpoint;
			if (migrationEndpointBase != null)
			{
				migrationEndpointBase.SlotProvider.UpdateAllocationCounts(this.DataProvider);
			}
			if (migrationEndpointBase != null)
			{
				return migrationEndpointBase.SlotProvider;
			}
			return BasicMigrationSlotProvider.Unlimited;
		}

		private readonly Lazy<BasicMigrationSlotProvider> slotProvider;

		private ILegacySubscriptionHandler subscriptionHandler;
	}
}
