using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationNotificationRpcImpl : IMigrationNotification
	{
		public MigrationNotificationRpcImpl(MigrationJobCache jobCache)
		{
			this.jobCache = jobCache;
		}

		public RegisterMigrationBatchResult RegisterMigrationBatch(RegisterMigrationBatchArgs args)
		{
			MigrationUtil.ThrowOnNullArgument(args, "args");
			MigrationServiceRpcMethodCode methodCode = MigrationServiceRpcMethodCode.RegisterMigrationBatch;
			try
			{
				MigrationLogContext.Current.Source = "RPC";
				OrganizationId organization = MigrationADProvider.GetOrganization(args.OrganizationId);
				MigrationLogContext.Current.Organization = args.OrganizationId;
				MigrationLogger.Log(MigrationEventType.Information, "RegisterMigrationBatch: MDB {0}, LegDn {1}", new object[]
				{
					args.MdbGuid,
					args.MailboxOwnerLegacyDN
				});
				if (!this.jobCache.Add(args.MailboxOwnerLegacyDN, args.MdbGuid, organization, args.Refresh))
				{
					LocalizedString localizedString = Strings.MigrationJobNotFound(args.OrganizationId.Name);
					MigrationApplication.NotifyOfTransientException(new ObjectNotFoundException(localizedString), "RegisterMigrationBatch : " + args.OrganizationId.Name);
					return new RegisterMigrationBatchResult(methodCode, MigrationServiceRpcResultCode.MigrationJobNotFound, localizedString);
				}
			}
			finally
			{
				MigrationLogContext.Current.Source = null;
				MigrationLogContext.Current.Organization = null;
			}
			return new RegisterMigrationBatchResult(methodCode);
		}

		public UpdateMigrationRequestResult UpdateMigrationRequest(UpdateMigrationRequestArgs args)
		{
			ADObjectId organizationalUnit = args.OrganizationalUnit;
			OrganizationId organization = MigrationADProvider.GetOrganization(organizationalUnit);
			UpdateMigrationRequestResult result2;
			try
			{
				MigrationLogContext.Current.Source = "RPC";
				MigrationLogContext.Current.Organization = organization.OrganizationalUnit;
				using (IMigrationDataProvider dataProvider = MigrationDataProvider.CreateProviderForMigrationMailbox(organization, args.MigrationMailboxUserLegacyDN))
				{
					MigrationSession migrationSession;
					MigrationJobItem jobItem;
					MigrationJob job;
					if (!MigrationApplication.TryGetEnabledMigrationSession(dataProvider, false, out migrationSession))
					{
						result2 = new UpdateMigrationRequestResult(MigrationServiceRpcMethodCode.SubscriptionStatusChanged, SubscriptionStatusChangedResponse.OK);
					}
					else if (!MigrationNotificationRpcImpl.TryGetMigrationJobItem(dataProvider, migrationSession, args, out jobItem, out job))
					{
						MigrationLogger.Log(MigrationEventType.Warning, "UpdateMigrationRequest: session {0}. Job item not found for the subscription {1}", new object[]
						{
							migrationSession,
							args
						});
						result2 = new UpdateMigrationRequestResult(MigrationServiceRpcMethodCode.SubscriptionStatusChanged, SubscriptionStatusChangedResponse.OK);
					}
					else
					{
						SubscriptionStatusChangedResponse result = SubscriptionStatusChangedResponse.OK;
						ItemStateTransitionHelper.RunJobItemOperation(job, jobItem, dataProvider, ItemStateTransitionHelper.GetEffectiveFailedJobItemStatus(job, jobItem.Status), delegate
						{
							result = this.UpdateMigrationJobItem(dataProvider, job, jobItem, args);
						});
						result2 = new UpdateMigrationRequestResult(MigrationServiceRpcMethodCode.SubscriptionStatusChanged, result);
					}
				}
			}
			finally
			{
				MigrationLogContext.Current.Source = null;
				MigrationLogContext.Current.Organization = null;
			}
			return result2;
		}

		private static bool TryGetMigrationJobItem(IMigrationDataProvider dataProvider, MigrationSession session, UpdateMigrationRequestArgs args, out MigrationJobItem jobItem, out MigrationJob job)
		{
			jobItem = null;
			if (!string.IsNullOrEmpty(args.UserExchangeMailboxLegacyDN))
			{
				jobItem = MigrationServiceHelper.GetJobItemByLegacyDN(dataProvider, args.UserExchangeMailboxLegacyDN, false);
			}
			else if (!string.IsNullOrEmpty(args.UserExchangeMailboxSmtpAddress))
			{
				MailboxData mailboxDataFromSmtpAddress = dataProvider.ADProvider.GetMailboxDataFromSmtpAddress(args.UserExchangeMailboxSmtpAddress, false, true);
				if (mailboxDataFromSmtpAddress != null)
				{
					jobItem = MigrationServiceHelper.GetJobItemByLegacyDN(dataProvider, args.UserExchangeMailboxSmtpAddress, false);
				}
			}
			else
			{
				jobItem = MigrationServiceHelper.GetIMAPJobItemBySubscriptionID(dataProvider, args.SubscriptionMessageId);
			}
			if (jobItem != null && jobItem.Status == MigrationUserStatus.Corrupted)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "UpdateMigrationRequest: skipping corrupt jobitem {0}. subscription {1}", new object[]
				{
					jobItem,
					args
				});
				jobItem = null;
			}
			job = null;
			if (jobItem == null)
			{
				return false;
			}
			job = MigrationJob.GetUniqueByJobId(dataProvider, jobItem.MigrationJobId);
			if (job == null || job.Status == MigrationJobStatus.Failed || job.Status == MigrationJobStatus.Corrupted)
			{
				MigrationLogger.Log(MigrationEventType.Error, "TryGetMigrationJobItem: founda  job item {0} but no job {1} with args {2}", new object[]
				{
					jobItem,
					jobItem.MigrationJobId,
					args
				});
				return false;
			}
			return true;
		}

		private SubscriptionStatusChangedResponse UpdateMigrationJobItem(IMigrationDataProvider dataProvider, MigrationJob job, MigrationJobItem jobItem, UpdateMigrationRequestArgs args)
		{
			MigrationUserStatus effectiveJobItemStatus = ItemStateTransitionHelper.GetEffectiveJobItemStatus(job.Status, jobItem.Status);
			MigrationLogger.Log(MigrationEventType.Information, "UpdateMigrationRequest: Job: [{0}], JobItem: [{1}], EffectiveStatus: [{2}] UpdateArgs: [{3}]", new object[]
			{
				job,
				jobItem,
				effectiveJobItemStatus,
				args
			});
			if (!ItemStateTransitionHelper.IsValidJobItemStateForTransition(job.Status, effectiveJobItemStatus))
			{
				MigrationLogger.Log(MigrationEventType.Warning, "UpdateMigrationRequest: JobItem is in unsupported state. Job [{0}], JobItem [{1}]", new object[]
				{
					job,
					jobItem
				});
				return SubscriptionStatusChangedResponse.OK;
			}
			if (jobItem.MigrationType != MigrationType.IMAP)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "UpdateMigrationRequest: job {0}, jobItem {1} - unsupported type", new object[]
				{
					job,
					jobItem
				});
				return SubscriptionStatusChangedResponse.OK;
			}
			SubscriptionStatusChangedResponse subscriptionStatusChangedResponse = ItemStateTransitionHelper.TransitionMigrationItem(dataProvider, job, jobItem, args);
			if (subscriptionStatusChangedResponse == SubscriptionStatusChangedResponse.OK && jobItem.ConsumedSlotType != MigrationSlotType.None && jobItem.Status != MigrationUserStatus.Syncing && jobItem.Status != MigrationUserStatus.Completing && jobItem.Status != MigrationUserStatus.IncrementalSyncing)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Releasing {0} slot consumed by job item {1} from item state transition helper, item status is now {2}.", new object[]
				{
					jobItem.ConsumedSlotType,
					jobItem.Identifier,
					jobItem.Status
				});
				jobItem.UpdateConsumedSlot(Guid.Empty, MigrationSlotType.None, dataProvider);
			}
			return subscriptionStatusChangedResponse;
		}

		private MigrationJobCache jobCache;
	}
}
