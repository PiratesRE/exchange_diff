using System;
using System.Globalization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UpdateSyncSubscriptionArgs
	{
		internal UpdateSyncSubscriptionArgs(ADObjectId organizationalUnit, string userLegacyDN, StoreObjectId subscriptionMessageId, AggregationSubscriptionType subscriptionType, UpdateSyncSubscriptionAction action)
		{
			this.OrganizationalUnit = organizationalUnit;
			this.userLegacyDN = userLegacyDN;
			this.subscriptionMessageId = subscriptionMessageId;
			this.subscriptionType = subscriptionType;
			this.action = action;
		}

		internal UpdateSyncSubscriptionArgs(ADObjectId organizationalUnit, string userLegacyDN, Guid subscriptionId, AggregationSubscriptionType subscriptionType, UpdateSyncSubscriptionAction action)
		{
			this.OrganizationalUnit = organizationalUnit;
			this.userLegacyDN = userLegacyDN;
			this.subscriptionId = new Guid?(subscriptionId);
			this.subscriptionType = subscriptionType;
			this.action = action;
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

		internal Guid? SubscriptionId
		{
			get
			{
				return this.subscriptionId;
			}
		}

		internal AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return this.subscriptionType;
			}
		}

		internal UpdateSyncSubscriptionAction Action
		{
			get
			{
				return this.action;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Action: {0}, Id: {1}, UserLegacyDN: {2}, Type: {3}", new object[]
			{
				this.Action,
				(this.subscriptionId != null) ? this.subscriptionId.Value.ToString() : this.subscriptionMessageId.ToString(),
				this.userLegacyDN,
				this.SubscriptionType
			});
		}

		internal static UpdateSyncSubscriptionArgs UnMarshal(MdbefPropertyCollection inputArgs)
		{
			byte[] subscriptonMessageIdBytes;
			if (MigrationRpcHelper.TryReadValue<byte[]>(inputArgs, 2684551426U, out subscriptonMessageIdBytes))
			{
				return new UpdateSyncSubscriptionArgs(MigrationRpcHelper.ReadADObjectId(inputArgs, 2688811266U), MigrationRpcHelper.ReadValue<string>(inputArgs, 2684485663U), MigrationRpcHelper.TryDeserializeStoreObjectId(subscriptonMessageIdBytes), MigrationRpcHelper.ReadEnum<AggregationSubscriptionType>(inputArgs, 2684616707U), MigrationRpcHelper.ReadEnum<UpdateSyncSubscriptionAction>(inputArgs, 2162691U));
			}
			return new UpdateSyncSubscriptionArgs(MigrationRpcHelper.ReadADObjectId(inputArgs, 2688811266U), MigrationRpcHelper.ReadValue<string>(inputArgs, 2684485663U), MigrationRpcHelper.ReadValue<Guid>(inputArgs, 2228296U), MigrationRpcHelper.ReadEnum<AggregationSubscriptionType>(inputArgs, 2684616707U), MigrationRpcHelper.ReadEnum<UpdateSyncSubscriptionAction>(inputArgs, 2162691U));
		}

		internal MdbefPropertyCollection Marshal()
		{
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			if (this.subscriptionMessageId != null)
			{
				mdbefPropertyCollection[2684551426U] = MigrationRpcHelper.SerializeStoreObjectId(this.subscriptionMessageId);
			}
			else
			{
				if (this.SubscriptionId == null)
				{
					throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, "No valid identity specified");
				}
				mdbefPropertyCollection[2228296U] = this.SubscriptionId;
			}
			mdbefPropertyCollection[2684485663U] = this.userLegacyDN;
			mdbefPropertyCollection[2684616707U] = (int)this.subscriptionType;
			mdbefPropertyCollection[2162691U] = (int)this.action;
			mdbefPropertyCollection[2688811266U] = this.OrganizationalUnit.GetBytes();
			return mdbefPropertyCollection;
		}

		private readonly string userLegacyDN;

		private readonly StoreObjectId subscriptionMessageId;

		private readonly Guid? subscriptionId;

		private readonly AggregationSubscriptionType subscriptionType;

		private readonly UpdateSyncSubscriptionAction action;
	}
}
