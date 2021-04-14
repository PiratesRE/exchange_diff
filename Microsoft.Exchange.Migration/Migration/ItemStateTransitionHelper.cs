using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ItemStateTransitionHelper
	{
		internal static MigrationUserStatus GetFinalSyncSuccededStatus(bool multiStepFinalization)
		{
			if (!multiStepFinalization)
			{
				return MigrationUserStatus.Completed;
			}
			return MigrationUserStatus.CompletionSynced;
		}

		internal static MigrationProcessorResult RunBatchOperation(MigrationJob job, IEnumerable<MigrationJobItem> jobItems, IMigrationDataProvider dataProvider, MigrationUserStatus? failedStatus, Action batchOperation)
		{
			MigrationUtil.ThrowOnNullArgument(jobItems, "jobItems");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(batchOperation, "batchOperation");
			try
			{
				batchOperation();
			}
			catch (MigrationSlotCapacityExceededException exception)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, exception, "Couldn't secure enough slots with the provider.", new object[0]);
				return MigrationProcessorResult.Waiting;
			}
			catch (WrongServerException ex)
			{
				MigrationLogger.Log(MigrationEventType.Warning, ex, "RunBatchOperation: encountered wrong server exception ", new object[0]);
				throw new MigrationTransientException(ex.LocalizedString, "RunBatchOperation", ex);
			}
			catch (MigrationDataCorruptionException ex2)
			{
				return ItemStateTransitionHelper.HandleCorruptException(job, dataProvider, jobItems, ex2);
			}
			catch (InvalidDataException ex3)
			{
				return ItemStateTransitionHelper.HandleCorruptException(job, dataProvider, jobItems, ex3);
			}
			catch (StoragePermanentException ex4)
			{
				return ItemStateTransitionHelper.HandlePermanentException(job, dataProvider, jobItems, failedStatus, ex4);
			}
			catch (MigrationServiceRpcException ex5)
			{
				return ItemStateTransitionHelper.HandlePermanentException(job, dataProvider, jobItems, failedStatus, ex5);
			}
			catch (MigrationPermanentException ex6)
			{
				return ItemStateTransitionHelper.HandlePermanentException(job, dataProvider, jobItems, failedStatus, ex6);
			}
			catch (TransientException ex7)
			{
				return ItemStateTransitionHelper.HandleTransientException(job, dataProvider, jobItems, failedStatus, ex7);
			}
			finally
			{
				if (dataProvider != null && job != null)
				{
					dataProvider.FlushReport(job.ReportData);
				}
			}
			return MigrationProcessorResult.Working;
		}

		internal static MigrationProcessorResult RunJobItemOperation(MigrationJob job, MigrationJobItem jobItem, IMigrationDataProvider dataProvider, MigrationUserStatus failedStatus, Action itemOperation)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(itemOperation, "itemOperation");
			MigrationProcessorResult result;
			try
			{
				MigrationLogContext.Current.JobItem = jobItem.Identifier;
				Func<MigrationProcessorResult> func = () => ItemStateTransitionHelper.RunBatchOperation(job, new MigrationJobItem[]
				{
					jobItem
				}, dataProvider, new MigrationUserStatus?(failedStatus), itemOperation);
				if (jobItem.LocalMailbox != null && jobItem.LocalMailbox is MailboxData)
				{
					using (new MailboxSettingsContext(((MailboxData)jobItem.LocalMailbox).UserMailboxId, null).Activate())
					{
						return func();
					}
				}
				result = func();
			}
			finally
			{
				MigrationLogContext.Current.JobItem = null;
			}
			return result;
		}

		internal static void RunJobItemRpcOperation(MigrationJobItem jobItem, IMigrationDataProvider dataProvider, ItemStateTransitionHelper.ItemRpcOperation rpcOperation)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(rpcOperation, "rpcOperation");
			int num = 0;
			MailboxData mailboxData = jobItem.LocalMailbox as MailboxData;
			bool flag = mailboxData == null || mailboxData.UserMailboxDatabaseId == Guid.Empty;
			int num2 = 0;
			while (num2++ <= 3)
			{
				string serverName = null;
				if (!flag)
				{
					try
					{
						serverName = dataProvider.ADProvider.GetDatabaseServerFqdn(mailboxData.UserMailboxDatabaseId, num2 > 1);
					}
					catch (MigrationTransientException exception)
					{
						jobItem.SetTransientError(dataProvider, exception);
						flag = true;
					}
				}
				if (flag)
				{
					serverName = ItemStateTransitionHelper.RefreshUserMailbox(jobItem, dataProvider);
				}
				try
				{
					IMigrationService migrationServiceClient = MigrationServiceFactory.Instance.GetMigrationServiceClient(serverName);
					rpcOperation(migrationServiceClient);
					break;
				}
				catch (MigrationObjectNotHostedException innerException)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "RunJobItemRpcOperation: get MigrationObjectNotHostedException for jobitem {0}", new object[]
					{
						jobItem
					});
					flag = true;
					num++;
					if (num > 2)
					{
						throw new MigrationRecipientNotFoundException(jobItem.Identifier, innerException);
					}
					if (num2 >= 3)
					{
						throw;
					}
				}
				catch (MigrationServiceRpcTransientException ex)
				{
					jobItem.SetTransientError(dataProvider, ex);
					if (MigrationApplication.HasTransientErrorReachedThreshold<MigrationUserStatus>(jobItem.StatusData))
					{
						MigrationLogger.Log(MigrationEventType.Warning, "RPC Transient error count reached threshold for jobitem " + jobItem, new object[0]);
						throw new MigrationPermanentException(ex.LocalizedString, "RunJobItemRPCOperation", ex);
					}
					MigrationApplication.NotifyOfTransientException(ex, "RunJobItemRpcOperation: jobitem " + jobItem);
					if (ex.IsConnectionError)
					{
						MigrationLogger.Log(MigrationEventType.Warning, "RunJobItemRpcOperation: get RpcConnectionError for jobitem {0}, error {1}", new object[]
						{
							jobItem,
							ex.RpcErrorCode
						});
						flag = true;
					}
					if (num2 >= 3)
					{
						throw;
					}
				}
				int num3 = (num > 0) ? 1 : 0;
				int num4 = (num2 - num3) * 1000;
				MigrationLogger.Log(MigrationEventType.Information, "RunJobItemRpcOperation: jobitem {0} sleep before RPC call {1} ms", new object[]
				{
					jobItem,
					num4
				});
				Thread.Sleep(num4);
			}
		}

		internal static MigrationProcessorResult ProcessDelayedSubscription(IMigrationDataProvider dataProvider, MigrationJob job, MigrationJobItem jobItem)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			GetSyncSubscriptionStateResult result = null;
			try
			{
				ItemStateTransitionHelper.RunJobItemRpcOperation(jobItem, dataProvider, delegate(IMigrationService migrationServiceEndPoint)
				{
					if (jobItem.LocalMailbox == null)
					{
						throw new MigrationDataCorruptionException("MailboxData missing for jobitem " + jobItem);
					}
					result = migrationServiceEndPoint.GetSyncSubscriptionState(new GetSyncSubscriptionStateArgs(dataProvider.OrganizationId.OrganizationalUnit, ((MailboxData)jobItem.LocalMailbox).MailboxLegacyDN, ((SyncSubscriptionId)jobItem.SubscriptionId).MessageId, AggregationSubscriptionType.IMAP));
				});
			}
			catch (MigrationSubscriptionNotFoundException)
			{
				result = ItemStateTransitionHelper.GetSyncSubscriptionStateFailedResult(jobItem, MigrationSubscriptionStatus.SubscriptionNotFound);
				MigrationLogger.Log(MigrationEventType.Warning, "ProcessDelayedSubscription: jobitem {0} failed because subscription not found", new object[]
				{
					jobItem
				});
			}
			catch (MigrationRecipientNotFoundException)
			{
				result = ItemStateTransitionHelper.GetSyncSubscriptionStateFailedResult(jobItem, MigrationSubscriptionStatus.MailboxNotFound);
				MigrationLogger.Log(MigrationEventType.Warning, "ProcessDelayedSubscription: jobitem {0} failed because user mailbox {1} not found", new object[]
				{
					jobItem,
					jobItem.LocalMailbox
				});
			}
			MigrationLogger.Log(MigrationEventType.Information, "SyncToSubscription, Retrieved data from subscription for jobItem. JobItem: {0}, GetSyncStateResult: {1}", new object[]
			{
				jobItem,
				result
			});
			SubscriptionStatusChangedResponse subscriptionStatusChangedResponse = ItemStateTransitionHelper.TransitionMigrationItem(dataProvider, job, jobItem, result);
			ItemStateTransitionHelper.PerformSubscriptionAction(dataProvider, jobItem, subscriptionStatusChangedResponse);
			if (subscriptionStatusChangedResponse == SubscriptionStatusChangedResponse.Delete)
			{
				jobItem.SetSubscriptionId(dataProvider, null, null);
			}
			return MigrationProcessorResult.Working;
		}

		internal static SubscriptionStatusChangedResponse TransitionMigrationItem(IMigrationDataProvider dataProvider, MigrationJob job, MigrationJobItem jobItem, ISubscriptionStatus subscriptiondata)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(subscriptiondata, "subscriptiondata");
			ItemStateTransition itemStateTransition = null;
			MigrationUserStatus effectiveJobItemStatus = ItemStateTransitionHelper.GetEffectiveJobItemStatus(job.Status, jobItem.Status);
			if (!ItemStateTransitionHelper.IsValidJobItemStateForTransition(job.Status, effectiveJobItemStatus))
			{
				return SubscriptionStatusChangedResponse.OK;
			}
			switch (effectiveJobItemStatus)
			{
			case MigrationUserStatus.Syncing:
			{
				ExDateTime value = jobItem.StateLastUpdated.Value;
				TimeSpan config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationInitialSyncTimeOutForFailingSubscriptions");
				itemStateTransition = ItemStateTransitionHelper.GetTransitionForStarted(job.Status, subscriptiondata, value, config);
				break;
			}
			case MigrationUserStatus.Synced:
				itemStateTransition = ItemStateTransitionHelper.GetTransitionForIncSync(job.Status, subscriptiondata);
				break;
			case MigrationUserStatus.IncrementalFailed:
				itemStateTransition = ItemStateTransitionHelper.GetTransitionForIncSyncFailed(job.Status, subscriptiondata);
				break;
			case MigrationUserStatus.Completing:
			{
				ExDateTime finalizationRequestTime = (job.FinalizeTime != null) ? job.FinalizeTime.Value : job.StateLastUpdated.Value;
				TimeSpan config2 = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationInitialSyncTimeOutForFailingSubscriptions");
				itemStateTransition = ItemStateTransitionHelper.GetTransitionForFinalizing(job.Status, jobItem.Status, ItemStateTransitionHelper.GetFinalSyncSuccededStatus(job.UpdateSourceOnFinalization), MigrationUserStatus.IncrementalFailed, subscriptiondata, finalizationRequestTime, config2);
				break;
			}
			}
			SubscriptionSnapshot stats = ItemStateTransitionHelper.CreateSnapshot(jobItem, subscriptiondata);
			if (itemStateTransition == null)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "TransitionMigrationItem: No transition required for jobitem {0} in {1} to due to {2}", new object[]
				{
					jobItem,
					jobItem.Status,
					subscriptiondata
				});
				jobItem.SetStatusAndSubscriptionLastChecked(dataProvider, null, null, new ExDateTime?(ExDateTime.UtcNow), true, stats);
				return SubscriptionStatusChangedResponse.OK;
			}
			MigrationLogger.Log(MigrationEventType.Information, "TransitionMigrationItem: Transitioning jobitem {0} from {1} to {2} due to {3}", new object[]
			{
				jobItem,
				jobItem.Status,
				itemStateTransition.Status,
				subscriptiondata
			});
			if (itemStateTransition.Status == MigrationUserStatus.IncrementalFailed)
			{
				int config3 = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationIncrementalSyncFailureThreshold");
				if (jobItem.IncrementalSyncFailures >= config3)
				{
					itemStateTransition = new ItemStateTransition(MigrationUserStatus.Failed, itemStateTransition.Response, new TooManyIncrementalSyncFailuresException(itemStateTransition.Error), itemStateTransition.SupportsIncrementalSync);
				}
			}
			jobItem.SetStatusAndSubscriptionLastChecked(dataProvider, new MigrationUserStatus?(itemStateTransition.Status), itemStateTransition.Error, new ExDateTime?(ExDateTime.UtcNow), itemStateTransition.SupportsIncrementalSync, stats);
			return itemStateTransition.Response;
		}

		internal static bool IsValidJobItemStateForTransition(MigrationJobStatus jobStatus, MigrationUserStatus effectiveStatus)
		{
			return jobStatus != MigrationJobStatus.Completing && jobStatus != MigrationJobStatus.Completed && jobStatus != MigrationJobStatus.Removing && jobStatus != MigrationJobStatus.Failed && jobStatus != MigrationJobStatus.Corrupted && (effectiveStatus == MigrationUserStatus.Syncing || effectiveStatus == MigrationUserStatus.Synced || effectiveStatus == MigrationUserStatus.IncrementalFailed || effectiveStatus == MigrationUserStatus.Completing);
		}

		internal static MigrationUserStatus GetEffectiveFailedJobItemStatus(MigrationJob job, MigrationUserStatus itemStatus)
		{
			if (MigrationJobStage.Completion.IsStatusSupported(job.Status))
			{
				return MigrationUserStatus.IncrementalFailed;
			}
			if (itemStatus == MigrationUserStatus.Syncing || itemStatus == MigrationUserStatus.Queued || itemStatus == MigrationUserStatus.Provisioning || itemStatus == MigrationUserStatus.ProvisionUpdating)
			{
				return MigrationUserStatus.Failed;
			}
			if (itemStatus == MigrationUserStatus.Synced)
			{
				return MigrationUserStatus.IncrementalFailed;
			}
			if (itemStatus != MigrationUserStatus.Corrupted && !MigrationJobItem.IsFailedStatus(itemStatus))
			{
				throw new MigrationDataCorruptionException("unknown item status for finding an error:" + itemStatus);
			}
			return itemStatus;
		}

		internal static MigrationUserStatus GetEffectiveJobItemStatus(MigrationJobStatus jobStatus, MigrationUserStatus itemStatus)
		{
			if (MigrationJobStage.Completion.IsStatusSupported(jobStatus) && (itemStatus == MigrationUserStatus.Synced || itemStatus == MigrationUserStatus.IncrementalFailed || itemStatus == MigrationUserStatus.Syncing))
			{
				return MigrationUserStatus.Completing;
			}
			return itemStatus;
		}

		internal static string LocalizeTimeSpan(TimeSpan timeSpan)
		{
			if (timeSpan > TimeSpan.FromDays(1.0))
			{
				return Strings.RunTimeFormatDays(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
			}
			if (timeSpan > TimeSpan.FromHours(1.0))
			{
				return Strings.RunTimeFormatHours(timeSpan.Hours, timeSpan.Minutes);
			}
			return Strings.RunTimeFormatMinutes(timeSpan.Minutes);
		}

		internal static ItemStateTransition GetTransitionForStarted(MigrationJobStatus jobStatus, ISubscriptionStatus subscription, ExDateTime subscriptionCreationTime, TimeSpan timeoutTimeSpan)
		{
			MigrationUtil.ThrowOnNullArgument(subscription, "subscription");
			if (MigrationJobStage.Completion.IsStatusSupported(jobStatus))
			{
				throw new MigrationDataCorruptionException("GetTransitionForStarted should not be called if the job status is " + jobStatus);
			}
			AggregationStatus status = subscription.Status;
			bool isInitialSyncComplete = subscription.IsInitialSyncComplete;
			switch (status)
			{
			case AggregationStatus.Succeeded:
				return new ItemStateTransition(MigrationUserStatus.Synced, SubscriptionStatusChangedResponse.OK, null);
			case AggregationStatus.InProgress:
				if (isInitialSyncComplete)
				{
					return new ItemStateTransition(MigrationUserStatus.Synced, SubscriptionStatusChangedResponse.OK, null);
				}
				if (subscription.MigrationSubscriptionStatus == MigrationSubscriptionStatus.InvalidPathPrefix)
				{
					return new ItemStateTransition(MigrationUserStatus.Failed, SubscriptionStatusChangedResponse.Disable, new MigrationPermanentException(Strings.IMAPPathPrefixInvalidStatus), false);
				}
				if (subscription.SubStatus == DetailedAggregationStatus.AuthenticationError)
				{
					return new ItemStateTransition(MigrationUserStatus.Failed, SubscriptionStatusChangedResponse.Disable, new MigrationPermanentException(Strings.AuthenticationErrorStatus), false);
				}
				return ItemStateTransitionHelper.HandleTimeout(ItemStateTransitionHelper.GetTimeoutReferenceTime(subscription), subscriptionCreationTime, timeoutTimeSpan, MigrationUserStatus.Failed, false);
			case AggregationStatus.Delayed:
				return new ItemStateTransition(MigrationUserStatus.Failed, SubscriptionStatusChangedResponse.Disable, ItemStateTransitionHelper.GetSubscriptionFailedMessage(subscription), false);
			case AggregationStatus.Disabled:
			case AggregationStatus.Poisonous:
			case AggregationStatus.InvalidVersion:
				return new ItemStateTransition(MigrationUserStatus.Failed, SubscriptionStatusChangedResponse.OK, ItemStateTransitionHelper.GetSubscriptionFailedMessage(subscription), false);
			default:
				return null;
			}
		}

		internal static ItemStateTransition GetTransitionForIncSync(MigrationJobStatus jobStatus, ISubscriptionStatus subscription)
		{
			MigrationUtil.ThrowOnNullArgument(subscription, "subscription");
			if (MigrationJobStage.Completion.IsStatusSupported(jobStatus))
			{
				throw new MigrationDataCorruptionException("GetTransitionForIncSync should not be called if the job status is " + jobStatus);
			}
			switch (subscription.Status)
			{
			case AggregationStatus.Succeeded:
			case AggregationStatus.InProgress:
				return null;
			case AggregationStatus.Delayed:
			case AggregationStatus.Disabled:
			case AggregationStatus.Poisonous:
			case AggregationStatus.InvalidVersion:
				return new ItemStateTransition(MigrationUserStatus.IncrementalFailed, SubscriptionStatusChangedResponse.OK, ItemStateTransitionHelper.GetSubscriptionFailedMessage(subscription));
			default:
				return null;
			}
		}

		internal static ItemStateTransition GetTransitionForIncSyncFailed(MigrationJobStatus jobStatus, ISubscriptionStatus subscription)
		{
			MigrationUtil.ThrowOnNullArgument(subscription, "subscription");
			if (MigrationJobStage.Completion.IsStatusSupported(jobStatus))
			{
				throw new MigrationDataCorruptionException("GetTransitionForIncSyncFailed should not be called if the job status is " + jobStatus);
			}
			switch (subscription.Status)
			{
			case AggregationStatus.Succeeded:
				return new ItemStateTransition(MigrationUserStatus.Synced, SubscriptionStatusChangedResponse.OK, null);
			case AggregationStatus.InProgress:
				return null;
			case AggregationStatus.Delayed:
			case AggregationStatus.Disabled:
			case AggregationStatus.Poisonous:
			case AggregationStatus.InvalidVersion:
				return new ItemStateTransition(MigrationUserStatus.IncrementalFailed, SubscriptionStatusChangedResponse.OK, ItemStateTransitionHelper.GetSubscriptionFailedMessage(subscription));
			default:
				return null;
			}
		}

		internal static ItemStateTransition GetTransitionForFinalizing(MigrationJobStatus jobStatus, MigrationUserStatus jobItemStatus, MigrationUserStatus successStatus, MigrationUserStatus errorStatus, ISubscriptionStatus subscription, ExDateTime finalizationRequestTime, TimeSpan finalizationTimeout)
		{
			MigrationUtil.ThrowOnNullArgument(subscription, "subscription");
			if (!MigrationJobStage.Completion.IsStatusSupported(jobStatus))
			{
				throw new MigrationDataCorruptionException("GetTransitionForFinalizing should not be called if the job status is " + jobStatus);
			}
			AggregationStatus status = subscription.Status;
			if (status == AggregationStatus.Succeeded)
			{
				DateTime? lastSuccessfulSyncTime = subscription.LastSuccessfulSyncTime;
				if (lastSuccessfulSyncTime != null && lastSuccessfulSyncTime.Value > (DateTime)finalizationRequestTime)
				{
					return new ItemStateTransition(successStatus, SubscriptionStatusChangedResponse.Disable, null);
				}
			}
			if (status == AggregationStatus.Disabled && subscription.SubStatus == DetailedAggregationStatus.Finalized)
			{
				return new ItemStateTransition(successStatus, SubscriptionStatusChangedResponse.OK, null);
			}
			if (jobItemStatus != MigrationUserStatus.Completing)
			{
				return null;
			}
			if (status == AggregationStatus.InvalidVersion || status == AggregationStatus.Poisonous || status == AggregationStatus.Disabled || status == AggregationStatus.Delayed)
			{
				return new ItemStateTransition(errorStatus, SubscriptionStatusChangedResponse.OK, ItemStateTransitionHelper.GetSubscriptionFailedMessage(subscription));
			}
			return ItemStateTransitionHelper.HandleTimeout(ItemStateTransitionHelper.GetTimeoutReferenceTime(subscription), finalizationRequestTime, finalizationTimeout, errorStatus, true);
		}

		internal static ItemStateTransition HandleTimeout(DateTime? timeoutReferenceTime, ExDateTime subscriptionCreationTime, TimeSpan timeout, MigrationUserStatus failedStatus, bool supportsIncrementalSync)
		{
			TimeSpan t;
			if (timeoutReferenceTime != null)
			{
				t = DateTime.UtcNow - timeoutReferenceTime.Value;
			}
			else
			{
				t = ExDateTime.UtcNow - subscriptionCreationTime;
			}
			if (t < timeout)
			{
				return null;
			}
			string diagnosticInfo = string.Format("ItemStateTransitionHelper.HandleTimeout: set job item status to {0} after timeout {1}", failedStatus, timeout);
			SyncTimeoutException ex = new SyncTimeoutException(ItemStateTransitionHelper.LocalizeTimeSpan(timeout));
			MigrationApplication.NotifyOfPermanentException(ex, diagnosticInfo);
			return new ItemStateTransition(failedStatus, SubscriptionStatusChangedResponse.Disable, ex, supportsIncrementalSync);
		}

		internal static void PerformSubscriptionAction(IMigrationDataProvider dataProvider, MigrationJobItem item, SubscriptionStatusChangedResponse action)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(action, "action");
			if (action != SubscriptionStatusChangedResponse.OK)
			{
				UpdateSyncSubscriptionAction updateAction;
				switch (action)
				{
				case SubscriptionStatusChangedResponse.Delete:
					updateAction = UpdateSyncSubscriptionAction.Delete;
					break;
				case SubscriptionStatusChangedResponse.Disable:
					updateAction = UpdateSyncSubscriptionAction.Disable;
					break;
				default:
					return;
				}
				SyncResourceAccessor syncResourceAccessor = new SyncResourceAccessor(dataProvider);
				syncResourceAccessor.UpdateSubscription(item, updateAction);
				return;
			}
		}

		private static SubscriptionSnapshot CreateSnapshot(MigrationJobItem jobItem, ISubscriptionStatus subscription)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.ThrowOnNullArgument(subscription, "subscription");
			LocalizedException ex = null;
			SnapshotStatus status;
			switch (subscription.Status)
			{
			case AggregationStatus.Succeeded:
			case AggregationStatus.InProgress:
				status = SnapshotStatus.InProgress;
				break;
			case AggregationStatus.Delayed:
				status = SnapshotStatus.InProgress;
				ex = ItemStateTransitionHelper.GetSubscriptionFailedMessage(subscription);
				break;
			case AggregationStatus.Disabled:
				status = (subscription.IsInitialSyncComplete ? SnapshotStatus.AutoSuspended : SnapshotStatus.Failed);
				ex = ItemStateTransitionHelper.GetSubscriptionFailedMessage(subscription);
				break;
			case AggregationStatus.Poisonous:
			case AggregationStatus.InvalidVersion:
				status = SnapshotStatus.Corrupted;
				ex = ItemStateTransitionHelper.GetSubscriptionFailedMessage(subscription);
				break;
			default:
				throw new MigrationDataCorruptionException("unknown subscription status:" + subscription.Status);
			}
			if (jobItem.StateLastUpdated == null)
			{
				throw new MigrationDataCorruptionException("expect to have a state last updated by now: " + jobItem);
			}
			ExDateTime? lastSyncTime = null;
			if (subscription.LastSuccessfulSyncTime != null)
			{
				lastSyncTime = new ExDateTime?((ExDateTime)subscription.LastSuccessfulSyncTime.Value);
			}
			ExDateTime? lastUpdateTime = null;
			if (subscription.LastSyncTime != null)
			{
				lastUpdateTime = new ExDateTime?((ExDateTime)subscription.LastSyncTime.Value);
			}
			SubscriptionSnapshot subscriptionSnapshot = new SubscriptionSnapshot((SyncSubscriptionId)jobItem.SubscriptionId, status, subscription.IsInitialSyncComplete, jobItem.StateLastUpdated.Value, lastUpdateTime, lastSyncTime, (ex != null) ? new LocalizedString?(ex.LocalizedString) : null, null);
			if (subscription.ItemsSynced != null != (subscription.ItemsSkipped != null))
			{
				throw new MigrationDataCorruptionException(string.Format("expect items skipped {0} and items synced {1} to both be set or not set", subscription.ItemsSkipped, subscription.ItemsSynced));
			}
			if (subscription.ItemsSynced != null)
			{
				MigrationUtil.AssertOrThrow(subscription.ItemsSkipped != null, "Expected ItemsSkipped to be populated whenever ItemsSynced is populated.", new object[0]);
				subscriptionSnapshot.SetStatistics(subscription.ItemsSynced.Value, subscription.ItemsSkipped.Value, null);
			}
			return subscriptionSnapshot;
		}

		private static MigrationProcessorResult HandleTransientException(MigrationJob job, IMigrationDataProvider dataProvider, IEnumerable<MigrationJobItem> jobItems, MigrationUserStatus? failedStatus, Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder("transient error occurred for job items:");
			foreach (MigrationJobItem migrationJobItem in jobItems)
			{
				migrationJobItem.SetTransientError(dataProvider, ex);
				if (stringBuilder.Length < 2048)
				{
					stringBuilder.Append(migrationJobItem);
				}
				if (MigrationApplication.HasTransientErrorReachedThreshold<MigrationUserStatus>(migrationJobItem.StatusData))
				{
					MigrationLogger.Log(MigrationEventType.Warning, "Transient error count reached threshold for job item '{0}'.", new object[]
					{
						migrationJobItem
					});
					ItemStateTransitionHelper.HandlePermanentException(job, dataProvider, new MigrationJobItem[]
					{
						migrationJobItem
					}, failedStatus, ex);
				}
			}
			MigrationApplication.NotifyOfTransientException(ex, stringBuilder.ToString());
			return MigrationProcessorResult.Waiting;
		}

		private static MigrationProcessorResult HandlePermanentException(MigrationJob job, IMigrationDataProvider dataProvider, IEnumerable<MigrationJobItem> jobItems, MigrationUserStatus? failedStatus, Exception ex)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(jobItems, "jobItems");
			if (failedStatus != null && failedStatus.Value == MigrationUserStatus.Corrupted)
			{
				return ItemStateTransitionHelper.HandleCorruptException(job, dataProvider, jobItems, ex);
			}
			LocalizedException localizedError = (ex as LocalizedException) ?? new MigrationUnknownException(ex);
			StringBuilder stringBuilder = new StringBuilder("permanent error occurred for job items:");
			foreach (MigrationJobItem migrationJobItem in jobItems)
			{
				if (failedStatus != null)
				{
					migrationJobItem.SetFailedStatus(dataProvider, failedStatus.Value, localizedError, MigrationLogger.GetDiagnosticInfo(ex, migrationJobItem.ToString()));
				}
				if (stringBuilder.Length < 2048)
				{
					stringBuilder.Append(migrationJobItem.ToString());
				}
			}
			MigrationApplication.NotifyOfPermanentException(ex, stringBuilder.ToString());
			return MigrationProcessorResult.Failed;
		}

		private static MigrationProcessorResult HandleCorruptException(MigrationJob job, IMigrationDataProvider dataProvider, IEnumerable<MigrationJobItem> jobItems, Exception ex)
		{
			foreach (MigrationJobItem migrationJobItem in jobItems)
			{
				MigrationApplication.NotifyOfCorruptJobItem(ex, "encounted a corrupt job item:" + migrationJobItem);
				migrationJobItem.SetCorruptStatus(dataProvider, ex);
				job.ReportData.Append(Strings.MigrationReportJobItemCorrupted(migrationJobItem.Identifier), ex, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
			}
			return MigrationProcessorResult.Failed;
		}

		private static string RefreshUserMailbox(MigrationJobItem jobItem, IMigrationDataProvider dataProvider)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MailboxData mailboxData;
			if (jobItem.LocalMailbox == null || string.IsNullOrEmpty(((MailboxData)jobItem.LocalMailbox).MailboxLegacyDN))
			{
				mailboxData = dataProvider.ADProvider.GetMailboxDataFromSmtpAddress(jobItem.Identifier, true, true);
			}
			else
			{
				mailboxData = dataProvider.ADProvider.GetMailboxDataFromLegacyDN(((MailboxData)jobItem.LocalMailbox).MailboxLegacyDN, true, jobItem.Identifier);
			}
			jobItem.SetUserMailboxProperties(dataProvider, null, mailboxData, null, null);
			return mailboxData.MailboxServer;
		}

		private static LocalizedException GetSubscriptionFailedMessage(ISubscriptionStatus subscriptionData)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionData, "subscriptionData");
			switch (subscriptionData.Status)
			{
			case AggregationStatus.Delayed:
			case AggregationStatus.Disabled:
				switch (subscriptionData.SubStatus)
				{
				case DetailedAggregationStatus.None:
					switch (subscriptionData.MigrationSubscriptionStatus)
					{
					case MigrationSubscriptionStatus.MailboxNotFound:
						return new SubscriptionMailboxNotFoundException();
					case MigrationSubscriptionStatus.RpcThresholdExceeded:
						return new SubscriptionRpcThresholdExceededException();
					case MigrationSubscriptionStatus.SubscriptionNotFound:
						return new SubscriptionNotFoundPermanentException();
					default:
						return new MigrationUnknownException();
					}
					break;
				case DetailedAggregationStatus.AuthenticationError:
					return new AuthenticationErrorException();
				case DetailedAggregationStatus.ConnectionError:
					return new ConnectionErrorException();
				case DetailedAggregationStatus.CommunicationError:
					return new ConnectionErrorException();
				case DetailedAggregationStatus.RemoteMailboxQuotaWarning:
					return new SourceMailboxQuotaWarningException();
				case DetailedAggregationStatus.LabsMailboxQuotaWarning:
					return new TargetMailboxQuotaWarningException();
				case DetailedAggregationStatus.Corrupted:
					return new CorruptedSubscriptionException();
				case DetailedAggregationStatus.LeaveOnServerNotSupported:
					return new LeaveOnServerNotSupportedException();
				case DetailedAggregationStatus.RemoteAccountDoesNotExist:
					return new AuthenticationErrorException();
				case DetailedAggregationStatus.RemoteServerIsSlow:
				case DetailedAggregationStatus.RemoteServerIsBackedOff:
				case DetailedAggregationStatus.RemoteServerIsPoisonous:
					return new RemoteServerIsSlowException();
				case DetailedAggregationStatus.TooManyFolders:
					return new TooManyFoldersException();
				case DetailedAggregationStatus.Finalized:
					return new SubscriptionDisabledSinceFinalizedException();
				case DetailedAggregationStatus.SyncStateSizeError:
					return new SyncStateSizeException();
				}
				return new MigrationUnknownException();
			case AggregationStatus.Poisonous:
				return new MigrationPermanentException(Strings.PoisonDetailedStatus);
			case AggregationStatus.InvalidVersion:
				return new MigrationPermanentException(Strings.InvalidVersionDetailedStatus);
			default:
				throw new InvalidOperationException("This method is to be called only if the underlying subscription has failures");
			}
		}

		private static GetSyncSubscriptionStateResult GetSyncSubscriptionStateFailedResult(MigrationJobItem jobItem, MigrationSubscriptionStatus subscriptionStatus)
		{
			GetSyncSubscriptionStateResult getSyncSubscriptionStateResult = new GetSyncSubscriptionStateResult(MigrationServiceRpcMethodCode.GetSyncSubscriptionState, AggregationStatus.Disabled, DetailedAggregationStatus.None, subscriptionStatus, false, null, null, null, null, null);
			MigrationLogger.Log(MigrationEventType.Warning, "GetSyncSubscriptionStateFailedResult, For JobItem {0}, Generating fake SyncSubscriptionStateResult: {1}", new object[]
			{
				jobItem,
				getSyncSubscriptionStateResult
			});
			return getSyncSubscriptionStateResult;
		}

		private static DateTime? GetTimeoutReferenceTime(ISubscriptionStatus subscription)
		{
			if (subscription.LastSyncTime != null && subscription.LastSyncNowRequestTime != null)
			{
				return new DateTime?(ItemStateTransitionHelper.GetMaxTime(subscription.LastSyncTime.Value, subscription.LastSyncNowRequestTime.Value));
			}
			DateTime? lastSyncTime = subscription.LastSyncTime;
			if (lastSyncTime == null)
			{
				return subscription.LastSyncNowRequestTime;
			}
			return new DateTime?(lastSyncTime.GetValueOrDefault());
		}

		private static DateTime GetMaxTime(DateTime dateTime1, DateTime dateTime2)
		{
			if (!(dateTime1 > dateTime2))
			{
				return dateTime2;
			}
			return dateTime1;
		}

		internal const MigrationUserStatus FinalSyncFailedStatus = MigrationUserStatus.IncrementalFailed;

		private const int MaximumNumberOfMigrationObjectNotHostedErrors = 2;

		private const int MaximumRpcRetryCount = 3;

		private const int RetryRpcInterval = 1000;

		private const int BatchErrorEventMaxLength = 2048;

		internal delegate void ItemRpcOperation(IMigrationService migrationServiceEndPoint);
	}
}
