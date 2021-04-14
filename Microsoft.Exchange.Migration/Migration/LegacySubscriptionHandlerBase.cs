using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class LegacySubscriptionHandlerBase : DisposeTrackableBase
	{
		internal LegacySubscriptionHandlerBase(IMigrationDataProvider dataProvider, MigrationJob migrationJob)
		{
			this.DataProvider = dataProvider;
			this.Job = migrationJob;
			string jobName = (migrationJob == null) ? null : migrationJob.JobName;
			this.SubscriptionAccessor = MigrationServiceFactory.Instance.GetSubscriptionAccessor(this.DataProvider, this.Job.MigrationType, jobName, false, !this.Job.AutoComplete);
		}

		public abstract bool SupportsActiveIncrementalSync { get; }

		public abstract bool SupportsAdvancedValidation { get; }

		private protected IMigrationDataProvider DataProvider { protected get; private set; }

		private protected MigrationJob Job { protected get; private set; }

		private protected SubscriptionAccessorBase SubscriptionAccessor { protected get; private set; }

		public static ILegacySubscriptionHandler CreateSubscriptionHandler(IMigrationDataProvider dataProvider, MigrationJob migrationJob)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(migrationJob, "migrationJob");
			MigrationType migrationType = migrationJob.MigrationType;
			if (migrationType <= MigrationType.BulkProvisioning)
			{
				if (migrationType == MigrationType.IMAP)
				{
					return new LegacyIMAPSubscriptionHandler(dataProvider, migrationJob);
				}
				if (migrationType == MigrationType.ExchangeOutlookAnywhere)
				{
					return new LegacyExchangeSubscriptionHandler(dataProvider, migrationJob);
				}
				if (migrationType == MigrationType.BulkProvisioning)
				{
					return null;
				}
			}
			else
			{
				if (migrationType == MigrationType.ExchangeRemoteMove)
				{
					return new LegacyRemoteMoveSubscriptionHandler(dataProvider, migrationJob);
				}
				if (migrationType == MigrationType.ExchangeLocalMove)
				{
					return new LegacyLocalMoveSubscriptionHandler(dataProvider, migrationJob);
				}
				if (migrationType == MigrationType.PublicFolder)
				{
					return new LegacyPublicFolderSubscriptionHandler(dataProvider, migrationJob);
				}
			}
			throw new ArgumentException("No handler defined for protocol " + migrationJob.MigrationType);
		}

		public virtual IEnumerable<MigrationJobItem> GetJobItemsForSubscriptionCheck(ExDateTime? cutoffTime, MigrationUserStatus status, int maxItemsToCheck)
		{
			MigrationUserStatus effectiveJobItemStatus = SubscriptionArbiterBase.GetEffectiveJobItemStatus(this.Job, status);
			MigrationUserStatus migrationUserStatus = effectiveJobItemStatus;
			switch (migrationUserStatus)
			{
			case MigrationUserStatus.Syncing:
			case MigrationUserStatus.Completing:
				break;
			case MigrationUserStatus.Failed:
				goto IL_46;
			case MigrationUserStatus.Synced:
			case MigrationUserStatus.IncrementalFailed:
				return this.GetJobItemsForIncrementalSubscriptionCheck(cutoffTime, status, maxItemsToCheck);
			default:
				if (migrationUserStatus != MigrationUserStatus.IncrementalSyncing)
				{
					goto IL_46;
				}
				break;
			}
			return this.GetJobItemsForActiveSubscriptionCheck(cutoffTime, status, maxItemsToCheck);
			IL_46:
			throw new MigrationDataCorruptionException(string.Format(CultureInfo.InvariantCulture, "invalid effective job item status {0} for subscription check job status {1}, job item status {2}", new object[]
			{
				effectiveJobItemStatus,
				this.Job.Status,
				status
			}));
		}

		public void CancelUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			this.DisableSubscriptions(jobItem);
			jobItem.DisableMigration(this.DataProvider, MigrationUserStatus.Stopped);
		}

		public void StopUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			if (this.SupportsActiveIncrementalSync)
			{
				this.DisableSubscriptions(jobItem);
			}
			jobItem.DisableMigration(this.DataProvider, MigrationUserStatus.IncrementalStopped);
		}

		public abstract void DisableSubscriptions(MigrationJobItem jobItem);

		public abstract void SyncSubscriptionSettings(MigrationJobItem jobItem);

		protected override void InternalDispose(bool disposing)
		{
		}

		protected virtual IEnumerable<MigrationJobItem> GetJobItemsForIncrementalSubscriptionCheck(ExDateTime? cutoffTime, MigrationUserStatus status, int maxItemsToCheck)
		{
			throw new NotSupportedException(string.Format("GetJobItemsForIncrementalSubscriptionCheck is not supported by {0}", base.GetType().Name));
		}

		protected virtual IEnumerable<MigrationJobItem> GetJobItemsForActiveSubscriptionCheck(ExDateTime? cutoffTime, MigrationUserStatus status, int maxItemsToCheck)
		{
			Dictionary<ISubscriptionId, MigrationJobItem> subscriptionMap = this.GetSubscriptionIdsForActiveSubscriptions(status, maxItemsToCheck);
			MigrationLogger.Log(MigrationEventType.Verbose, "querying for active subscription, Found {0} for job {1}", new object[]
			{
				subscriptionMap.Count,
				this.Job
			});
			ExDateTime statisticCutoffTime = ExDateTime.UtcNow - ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationInitialSyncStartPollingTimeout");
			if (cutoffTime == null || cutoffTime.Value < statisticCutoffTime)
			{
				cutoffTime = new ExDateTime?(statisticCutoffTime);
			}
			Dictionary<ISubscriptionId, MigrationJobItem> skippedIds = new Dictionary<ISubscriptionId, MigrationJobItem>();
			foreach (KeyValuePair<ISubscriptionId, MigrationJobItem> item in subscriptionMap)
			{
				KeyValuePair<ISubscriptionId, MigrationJobItem> keyValuePair = item;
				MigrationJobItem jobItem = keyValuePair.Value;
				if (jobItem.SubscriptionLastChecked == null || jobItem.SubscriptionLastChecked.Value < cutoffTime.Value)
				{
					MigrationLogger.Log(MigrationEventType.Verbose, "Found old job item {0} for job {1}", new object[]
					{
						jobItem,
						this.Job
					});
					yield return jobItem;
				}
				else
				{
					Dictionary<ISubscriptionId, MigrationJobItem> dictionary = skippedIds;
					KeyValuePair<ISubscriptionId, MigrationJobItem> keyValuePair2 = item;
					ISubscriptionId key = keyValuePair2.Key;
					KeyValuePair<ISubscriptionId, MigrationJobItem> keyValuePair3 = item;
					dictionary.Add(key, keyValuePair3.Value);
				}
			}
			foreach (KeyValuePair<ISubscriptionId, MigrationJobItem> item2 in skippedIds)
			{
				SnapshotStatus subscriptionStatus = SnapshotStatus.InProgress;
				KeyValuePair<ISubscriptionId, MigrationJobItem> localItem = item2;
				KeyValuePair<ISubscriptionId, MigrationJobItem> keyValuePair4 = item2;
				this.RunJobItemOperation(keyValuePair4.Value, delegate
				{
					MigrationJobItem value = localItem.Value;
					ISubscriptionId key2 = localItem.Key;
					subscriptionStatus = this.GetJobItemSubscriptionStatus(key2, value);
				});
				if (subscriptionStatus != SnapshotStatus.InProgress)
				{
					MigrationEventType eventType = MigrationEventType.Verbose;
					string format = "subscription has finished with status {0} for job item {1}";
					object[] array = new object[2];
					array[0] = subscriptionStatus;
					object[] array2 = array;
					int num = 1;
					KeyValuePair<ISubscriptionId, MigrationJobItem> keyValuePair5 = item2;
					array2[num] = keyValuePair5.Value;
					MigrationLogger.Log(eventType, format, array);
					KeyValuePair<ISubscriptionId, MigrationJobItem> keyValuePair6 = item2;
					yield return keyValuePair6.Value;
				}
			}
			yield break;
		}

		protected virtual Dictionary<ISubscriptionId, MigrationJobItem> GetSubscriptionIdsForActiveSubscriptions(MigrationUserStatus status, int maxItemsToCheck)
		{
			Dictionary<ISubscriptionId, MigrationJobItem> subscriptionIds = new Dictionary<ISubscriptionId, MigrationJobItem>(maxItemsToCheck);
			foreach (MigrationJobItem migrationJobItem in MigrationJobItem.GetBySubscriptionLastChecked(this.DataProvider, this.Job, null, status, maxItemsToCheck))
			{
				MigrationJobItem item = migrationJobItem;
				this.RunJobItemOperation(migrationJobItem, delegate
				{
					ISubscriptionId subscriptionId = item.SubscriptionId;
					if (subscriptionId != null)
					{
						if (subscriptionIds.ContainsKey(subscriptionId))
						{
							string internalDetails = string.Format("subscription id {0} already found for item {1} with id {2}, other item {3} with id {4}", new object[]
							{
								subscriptionId,
								subscriptionIds[subscriptionId],
								subscriptionIds[subscriptionId].JobItemGuid,
								item,
								item.JobItemGuid
							});
							throw new MigrationDataCorruptionException(internalDetails);
						}
						subscriptionIds.Add(subscriptionId, item);
					}
				});
			}
			return subscriptionIds;
		}

		protected virtual SnapshotStatus GetJobItemSubscriptionStatus(ISubscriptionId subscriptionId, MigrationJobItem migrationJobItem)
		{
			throw new NotSupportedException(string.Format("GetJobItemSubscriptionStatus is not supported by {0}", base.GetType().Name));
		}

		protected virtual MigrationProcessorResult RunJobItemOperation(MigrationJobItem jobItem, Action itemOperation)
		{
			throw new NotSupportedException(string.Format("RunJobItemOperation is not supported by {0}", base.GetType().Name));
		}

		protected virtual ISubscriptionId GetJobItemSubscriptionId(MigrationJobItem item)
		{
			throw new NotSupportedException(string.Format("GetJobItemSubscriptionId is not supported by {0}", base.GetType().Name));
		}
	}
}
