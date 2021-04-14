using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncSubscriptionRunspaceAccessor : RunspaceAccessorBase
	{
		public SyncSubscriptionRunspaceAccessor(IMigrationDataProvider dataProvider) : base(dataProvider)
		{
		}

		public override SubscriptionSnapshot CreateSubscription(MigrationJobItem jobItem)
		{
			throw new InvalidOperationException("still use rpc");
		}

		public override SubscriptionSnapshot TestCreateSubscription(MigrationJobItem jobItem)
		{
			throw new InvalidOperationException("still use rpc");
		}

		public override SnapshotStatus RetrieveSubscriptionStatus(ISubscriptionId subscriptionId)
		{
			throw new InvalidOperationException("still use rpc");
		}

		public override SubscriptionSnapshot RetrieveSubscriptionSnapshot(ISubscriptionId subscriptionId)
		{
			MigrationUtil.ThrowOnNullArgument(subscriptionId, "subscriptionId");
			SyncSubscriptionId syncSubscriptionId = subscriptionId as SyncSubscriptionId;
			MigrationUtil.AssertOrThrow(syncSubscriptionId != null, "subscription id type not valid txsync:" + subscriptionId.GetType(), new object[0]);
			MigrationUtil.AssertOrThrow(syncSubscriptionId.Id != null, "subscription id set, but guid is missing:" + syncSubscriptionId, new object[0]);
			PSCommand pscommand = new PSCommand().AddCommand("Get-Subscription");
			pscommand.AddParameter("AggregationType", "Migration");
			pscommand.AddParameter("IncludeReport", true);
			pscommand.AddParameter("Identity", syncSubscriptionId.Id.ToString());
			pscommand.AddParameter("Mailbox", syncSubscriptionId.MailboxData.GetIdParameter<MailboxIdParameter>());
			PimSubscriptionProxy pimSubscriptionProxy = base.RunCommand<PimSubscriptionProxy>(pscommand, null, null);
			if (pimSubscriptionProxy == null)
			{
				MigrationLogger.Log(MigrationEventType.Warning, "subscription id stored {0} but no subscription found", new object[]
				{
					syncSubscriptionId
				});
				return null;
			}
			SnapshotStatus status = SyncSubscriptionRunspaceAccessor.SubscriptionStatusFromSubscription(pimSubscriptionProxy);
			SubscriptionSnapshot subscriptionSnapshot = new SubscriptionSnapshot(syncSubscriptionId, status, pimSubscriptionProxy.SyncPhase != SyncPhase.Initial, (ExDateTime)pimSubscriptionProxy.CreationTime, new ExDateTime?((ExDateTime)pimSubscriptionProxy.LastModifiedTime), (ExDateTime?)pimSubscriptionProxy.LastSyncTime, new LocalizedString?(Strings.DetailedAggregationStatus(pimSubscriptionProxy.DetailedStatus)), null);
			subscriptionSnapshot.SetStatistics(pimSubscriptionProxy.TotalItemsSynced, pimSubscriptionProxy.TotalItemsSkipped, pimSubscriptionProxy.TotalItemsInSourceMailbox);
			ByteQuantifiedSize value;
			if (!string.IsNullOrEmpty(pimSubscriptionProxy.TotalSizeOfSourceMailbox) && ByteQuantifiedSize.TryParse(pimSubscriptionProxy.TotalSizeOfSourceMailbox, out value))
			{
				subscriptionSnapshot.EstimatedTotalTransferSize = new ByteQuantifiedSize?(value);
			}
			if (pimSubscriptionProxy.TotalItemsInSourceMailbox != null)
			{
				subscriptionSnapshot.EstimatedTotalTransferCount = new ulong?((ulong)pimSubscriptionProxy.TotalItemsInSourceMailbox.Value);
			}
			subscriptionSnapshot.Report = pimSubscriptionProxy.Report;
			return subscriptionSnapshot;
		}

		public override bool UpdateSubscription(ISubscriptionId subscriptionId, MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool adoptingSubscription)
		{
			throw new NotImplementedException();
		}

		public override bool ResumeSubscription(ISubscriptionId subscriptionId, bool finalize = false)
		{
			throw new NotImplementedException();
		}

		public override bool SuspendSubscription(ISubscriptionId subscriptionId)
		{
			throw new NotImplementedException();
		}

		public override bool RemoveSubscription(ISubscriptionId subscriptionId)
		{
			throw new NotImplementedException();
		}

		protected override T HandleException<T>(string commandString, Exception ex, ICollection<Type> transientExceptions)
		{
			MigrationUtil.ThrowOnNullArgument(ex, "ex");
			throw new MigrationPermanentException(ServerStrings.MigrationRunspaceError(commandString, ex.Message), ex);
		}

		private static SnapshotStatus SubscriptionStatusFromSubscription(PimSubscriptionProxy subscription)
		{
			switch (subscription.Status)
			{
			case AggregationStatus.Succeeded:
			case AggregationStatus.InProgress:
			case AggregationStatus.Delayed:
				return SnapshotStatus.InProgress;
			case AggregationStatus.Disabled:
				if (subscription.SyncPhase != SyncPhase.Initial)
				{
					return SnapshotStatus.Suspended;
				}
				return SnapshotStatus.Failed;
			case AggregationStatus.Poisonous:
			case AggregationStatus.InvalidVersion:
				return SnapshotStatus.Corrupted;
			default:
				MigrationLogger.Log(MigrationEventType.Error, "Unknown status for subscription: {0}", new object[]
				{
					subscription.Status
				});
				return SnapshotStatus.Corrupted;
			}
		}

		private const string GetSubscriptionCmdletName = "Get-Subscription";

		private const string AggregationTypeParameter = "AggregationType";

		private const string AggregationTypeValue = "Migration";

		private const string IdentityParameter = "Identity";

		private const string MailboxParameter = "Mailbox";

		private const string IncludeReportParameter = "IncludeReport";
	}
}
