using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncResourceAccessor
	{
		internal SyncResourceAccessor(IMigrationDataProvider dataProvider)
		{
			this.dataProvider = dataProvider;
		}

		private IMigrationDataProvider DataProvider
		{
			get
			{
				return this.dataProvider;
			}
		}

		internal void CreateSubscription(MigrationJobItem jobItem, AbstractCreateSyncSubscriptionArgs subscriptionCreationArgs)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			MigrationUtil.ThrowOnNullArgument(subscriptionCreationArgs, "subscriptionCreationArgs");
			try
			{
				CreateSyncSubscriptionResult result = null;
				ItemStateTransitionHelper.RunJobItemRpcOperation(jobItem, this.DataProvider, delegate(IMigrationService stub)
				{
					result = stub.CreateSyncSubscription(subscriptionCreationArgs);
					MigrationLogger.Log(MigrationEventType.Information, "Sync subscription created: jobitem {0}, type {1}, CreateSyncSubscriptionResult (SubscriptionID): {2}", new object[]
					{
						jobItem,
						subscriptionCreationArgs.SubscriptionType,
						result
					});
				});
				SyncSubscriptionId subscriptionId = new SyncSubscriptionId(result.SubscriptionGuid, result.SubscriptionMessageId, jobItem.LocalMailbox);
				jobItem.SetSubscriptionId(this.DataProvider, subscriptionId, new MigrationUserStatus?(MigrationUserStatus.Syncing));
			}
			catch (MigrationServiceRpcException ex)
			{
				MigrationLogger.Log(MigrationEventType.Warning, ex, "JobItem {0} couldn't create subscription", new object[]
				{
					jobItem
				});
				jobItem.SetSubscriptionFailed(this.DataProvider, MigrationUserStatus.Failed, new MigrationSubscriptionCreationFailedException(jobItem.Identifier, ex));
			}
			catch (MigrationRecipientNotFoundException ex2)
			{
				MigrationLogger.Log(MigrationEventType.Warning, ex2, "JobItem {0} couldn't create subscription", new object[]
				{
					jobItem
				});
				jobItem.SetSubscriptionFailed(this.DataProvider, MigrationUserStatus.Failed, ex2);
			}
		}

		internal void FinalizeSubscription(MigrationJobItem jobItem, MigrationUserStatus successStatus, MigrationUserStatus alreadyFinalizedStatus, MigrationUserStatus permanentFailure)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			Exception exception = null;
			try
			{
				ItemStateTransitionHelper.RunJobItemRpcOperation(jobItem, this.DataProvider, delegate(IMigrationService stub)
				{
					MigrationLogger.Log(MigrationEventType.Information, "Invoking SyncNow on Sync subscription. jobitem {0}, type {1}, subscription {2}", new object[]
					{
						jobItem,
						AggregationSubscriptionType.IMAP,
						jobItem.SubscriptionId
					});
					stub.UpdateSyncSubscription(new UpdateSyncSubscriptionArgs(this.DataProvider.OrganizationId.OrganizationalUnit, ((MailboxData)jobItem.LocalMailbox).MailboxLegacyDN, ((SyncSubscriptionId)jobItem.SubscriptionId).MessageId, AggregationSubscriptionType.IMAP, UpdateSyncSubscriptionAction.Finalize));
				});
				jobItem.SetStatusAndSubscriptionLastChecked(this.DataProvider, new MigrationUserStatus?(successStatus), null, new ExDateTime?(ExDateTime.UtcNow), true, null);
				return;
			}
			catch (MigrationTargetInvocationException ex)
			{
				if (ex.ResultCode == MigrationServiceRpcResultCode.SubscriptionAlreadyFinalized)
				{
					jobItem.SetStatusAndSubscriptionLastChecked(this.DataProvider, new MigrationUserStatus?(alreadyFinalizedStatus), null, new ExDateTime?(ExDateTime.UtcNow), true, null);
					return;
				}
				exception = ex;
			}
			catch (MigrationPermanentException ex2)
			{
				MigrationApplication.NotifyOfPermanentException(ex2, "Permanent Exception while invoking FinalizeSubscription");
				exception = ex2;
			}
			MigrationLogger.Log(MigrationEventType.Error, exception, "JobItem {0} couldn't invoke syncnow", new object[]
			{
				jobItem
			});
			jobItem.SetSubscriptionFailed(this.DataProvider, permanentFailure, new MigrationPermanentException(Strings.CorruptedSubscriptionStatus));
		}

		internal void UpdateSubscription(MigrationJobItem jobItem, UpdateSyncSubscriptionAction updateAction)
		{
			MigrationUtil.ThrowOnNullArgument(jobItem, "jobItem");
			if (updateAction == UpdateSyncSubscriptionAction.Finalize)
			{
				throw new ArgumentException("Do not invoke SyncResourceAccessor.UpdateSubscription with Finalize update action. Invoke FinalizeSubscription instead");
			}
			if (jobItem.SubscriptionId == null)
			{
				return;
			}
			try
			{
				ItemStateTransitionHelper.RunJobItemRpcOperation(jobItem, this.DataProvider, delegate(IMigrationService stub)
				{
					if (jobItem.LocalMailbox == null)
					{
						throw new MigrationDataCorruptionException("TargetMailbox missing for job item " + jobItem);
					}
					MigrationLogger.Log(MigrationEventType.Information, "Sync subscription updating: action {0}, jobitem {1}, type {2}, subscription {3}", new object[]
					{
						updateAction,
						jobItem,
						AggregationSubscriptionType.IMAP,
						jobItem.SubscriptionId
					});
					stub.UpdateSyncSubscription(new UpdateSyncSubscriptionArgs(this.DataProvider.OrganizationId.OrganizationalUnit, ((MailboxData)jobItem.LocalMailbox).MailboxLegacyDN, ((SyncSubscriptionId)jobItem.SubscriptionId).MessageId, AggregationSubscriptionType.IMAP, updateAction));
				});
			}
			catch (MigrationSubscriptionNotFoundException exception)
			{
				MigrationLogger.Log(MigrationEventType.Warning, exception, "JobItem {0} couldn't update subscription with action {1}", new object[]
				{
					jobItem,
					updateAction
				});
			}
			catch (MigrationRecipientNotFoundException exception2)
			{
				MigrationLogger.Log(MigrationEventType.Warning, exception2, "JobItem {0} couldn't update subscription with action {1}", new object[]
				{
					jobItem,
					updateAction
				});
			}
			catch (MigrationPermanentException exception3)
			{
				MigrationLogger.Log(MigrationEventType.Error, exception3, "JobItem {0} couldn't update subscription with action {1}", new object[]
				{
					jobItem,
					updateAction
				});
				throw;
			}
		}

		private IMigrationDataProvider dataProvider;
	}
}
