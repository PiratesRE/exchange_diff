using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractCreateSyncSubscriptionArgs
	{
		internal AbstractCreateSyncSubscriptionArgs(AggregationSubscriptionType subscriptionType, ADObjectId organizationalUnit, string subscriptionName, string userLegacyDN, string userDisplayName, SmtpAddress remoteEmailAddress, bool forceNew)
		{
			this.SubscriptionType = subscriptionType;
			this.OrganizationalUnit = organizationalUnit;
			this.userLegacyDn = userLegacyDN;
			this.subscriptionName = subscriptionName;
			this.userDisplayName = userDisplayName;
			this.remoteEmailAddress = remoteEmailAddress;
			this.forceNew = forceNew;
		}

		internal string SubscriptionName
		{
			get
			{
				return this.subscriptionName;
			}
		}

		internal AggregationSubscriptionType SubscriptionType { get; private set; }

		internal string UserLegacyDn
		{
			get
			{
				return this.userLegacyDn;
			}
		}

		internal string UserDisplayName
		{
			get
			{
				return this.userDisplayName;
			}
		}

		internal SmtpAddress SmtpAddress
		{
			get
			{
				return this.remoteEmailAddress;
			}
		}

		internal bool ForceNew
		{
			get
			{
				return this.forceNew;
			}
		}

		internal ADObjectId OrganizationalUnit { get; set; }

		internal static AbstractCreateSyncSubscriptionArgs Create(MdbefPropertyCollection inputArgs, int version)
		{
			if (version == 1)
			{
				return CreateIMAPSyncSubscriptionArgs.Unmarshal(inputArgs);
			}
			if (version > 2)
			{
				throw new MigrationServiceRpcException(MigrationServiceRpcResultCode.VersionMismatchError, string.Format("This method was invoked with a version too high : {0}", version));
			}
			AggregationSubscriptionType aggregationSubscriptionType = MigrationRpcHelper.ReadEnum<AggregationSubscriptionType>(inputArgs, 2684616707U);
			AggregationSubscriptionType aggregationSubscriptionType2 = aggregationSubscriptionType;
			if (aggregationSubscriptionType2 == AggregationSubscriptionType.IMAP)
			{
				return CreateIMAPSyncSubscriptionArgs.Unmarshal(inputArgs);
			}
			throw new MigrationServiceRpcException(MigrationServiceRpcResultCode.VersionMismatchError, string.Format("This method was invoked with an unsupported aggregation type {0}", aggregationSubscriptionType));
		}

		internal abstract AggregationSubscription CreateInMemorySubscription();

		internal virtual MdbefPropertyCollection Marshal()
		{
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			mdbefPropertyCollection[2684616707U] = (int)this.SubscriptionType;
			mdbefPropertyCollection[2688811266U] = this.OrganizationalUnit.GetBytes();
			mdbefPropertyCollection[2684485663U] = this.UserLegacyDn;
			mdbefPropertyCollection[2685403167U] = this.SubscriptionName;
			mdbefPropertyCollection[2685927455U] = this.userDisplayName;
			mdbefPropertyCollection[2685599775U] = this.SmtpAddress.ToString();
			mdbefPropertyCollection[2686255115U] = this.forceNew;
			return mdbefPropertyCollection;
		}

		internal virtual void FillSubscription(AggregationSubscription subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			subscription.AggregationType = AggregationType.Migration;
			subscription.SubscriptionEvents = (SubscriptionEvents.WorkItemCompleted | SubscriptionEvents.WorkItemFailedLoadSubscription);
			subscription.Name = this.SubscriptionName;
		}

		private readonly string userLegacyDn;

		private readonly string subscriptionName;

		private readonly string userDisplayName;

		private readonly SmtpAddress remoteEmailAddress;

		private readonly bool forceNew;
	}
}
