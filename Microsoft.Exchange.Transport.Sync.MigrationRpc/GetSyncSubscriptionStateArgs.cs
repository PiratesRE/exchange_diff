using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GetSyncSubscriptionStateArgs
	{
		internal GetSyncSubscriptionStateArgs(ADObjectId organizationalUnit, string userLegacyDN, StoreObjectId subscriptionMessageId, AggregationSubscriptionType subscriptionType)
		{
			this.userLegacyDN = userLegacyDN;
			this.subscriptionMessageId = subscriptionMessageId;
			this.subscriptionType = subscriptionType;
			this.OrganizationalUnit = organizationalUnit;
		}

		internal ADObjectId OrganizationalUnit { get; private set; }

		internal string UserLegacyDN
		{
			get
			{
				return this.userLegacyDN;
			}
		}

		internal StoreObjectId SubscriptionMessageId
		{
			get
			{
				return this.subscriptionMessageId;
			}
		}

		internal AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return this.subscriptionType;
			}
		}

		internal static GetSyncSubscriptionStateArgs UnMarshal(MdbefPropertyCollection inputArgs)
		{
			byte[] subscriptonMessageIdBytes = MigrationRpcHelper.ReadValue<byte[]>(inputArgs, 2684551426U);
			return new GetSyncSubscriptionStateArgs(MigrationRpcHelper.ReadADObjectId(inputArgs, 2688811266U), MigrationRpcHelper.ReadValue<string>(inputArgs, 2684485663U), MigrationRpcHelper.TryDeserializeStoreObjectId(subscriptonMessageIdBytes), MigrationRpcHelper.ReadEnum<AggregationSubscriptionType>(inputArgs, 2684616707U));
		}

		internal MdbefPropertyCollection Marshal()
		{
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			mdbefPropertyCollection[2688811266U] = this.OrganizationalUnit.GetBytes();
			mdbefPropertyCollection[2684485663U] = this.userLegacyDN;
			mdbefPropertyCollection[2684551426U] = MigrationRpcHelper.SerializeStoreObjectId(this.subscriptionMessageId);
			mdbefPropertyCollection[2684616707U] = (int)this.subscriptionType;
			return mdbefPropertyCollection;
		}

		private readonly string userLegacyDN;

		private readonly StoreObjectId subscriptionMessageId;

		private readonly AggregationSubscriptionType subscriptionType;
	}
}
