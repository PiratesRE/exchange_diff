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
	internal sealed class UpdateMigrationRequestArgs : ISubscriptionStatus
	{
		internal UpdateMigrationRequestArgs(string userExchangeMailboxSmtpAddress, string userExchangeMailboxLegacyDN, string migrationMailboxLegacyDN, ADObjectId organizationalUnit, StoreObjectId subscriptionMessageId, AggregationStatus subscriptionStatus, DetailedAggregationStatus subscriptionDetailedStatus, MigrationSubscriptionStatus migrationSubscriptionStatus, bool initialSyncComplete, DateTime? lastSyncTime, DateTime? lastSuccessfulSyncTime, long? itemsSynced, long? itemsSkipped, DateTime? lastSyncNowRequestTime)
		{
			this.userExchangeMailboxSmtpAddress = userExchangeMailboxSmtpAddress;
			this.userExchangeMailboxLegacyDN = userExchangeMailboxLegacyDN;
			this.migrationMailboxLegacyDN = migrationMailboxLegacyDN;
			this.organizationalUnit = organizationalUnit;
			this.subscriptionMessageId = subscriptionMessageId;
			this.subscriptionStatus = subscriptionStatus;
			this.subscriptionDetailedStatus = subscriptionDetailedStatus;
			this.migrationSubscriptionStatus = migrationSubscriptionStatus;
			this.initialSyncComplete = initialSyncComplete;
			this.lastSyncTime = lastSyncTime;
			this.lastSuccessfulSyncTime = lastSuccessfulSyncTime;
			this.itemsSynced = itemsSynced;
			this.itemsSkipped = itemsSkipped;
			this.lastSyncNowRequestTime = lastSyncNowRequestTime;
		}

		public bool IsInitialSyncComplete
		{
			get
			{
				return this.initialSyncComplete;
			}
		}

		public AggregationStatus Status
		{
			get
			{
				return this.subscriptionStatus;
			}
		}

		public DetailedAggregationStatus SubStatus
		{
			get
			{
				return this.subscriptionDetailedStatus;
			}
		}

		public MigrationSubscriptionStatus MigrationSubscriptionStatus
		{
			get
			{
				return this.migrationSubscriptionStatus;
			}
		}

		public DateTime? LastSyncTime
		{
			get
			{
				return this.lastSyncTime;
			}
		}

		public DateTime? LastSuccessfulSyncTime
		{
			get
			{
				return this.lastSuccessfulSyncTime;
			}
		}

		public DateTime? LastSyncNowRequestTime
		{
			get
			{
				return this.lastSyncNowRequestTime;
			}
		}

		public long? ItemsSynced
		{
			get
			{
				return this.itemsSynced;
			}
		}

		public long? ItemsSkipped
		{
			get
			{
				return this.itemsSkipped;
			}
		}

		internal string MigrationMailboxUserLegacyDN
		{
			get
			{
				return this.migrationMailboxLegacyDN;
			}
		}

		internal string UserExchangeMailboxSmtpAddress
		{
			get
			{
				return this.userExchangeMailboxSmtpAddress;
			}
		}

		internal string UserExchangeMailboxLegacyDN
		{
			get
			{
				return this.userExchangeMailboxLegacyDN;
			}
		}

		internal ADObjectId OrganizationalUnit
		{
			get
			{
				return this.organizationalUnit;
			}
		}

		internal StoreObjectId SubscriptionMessageId
		{
			get
			{
				return this.subscriptionMessageId;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "UpdateMigrationRequestArgs: Status: [{0}], Substatus: [{1}], LastSync: [{2}], LastGoodSync: [{3}], InitialSyncDone: [{4}]", new object[]
			{
				this.Status,
				this.SubStatus,
				this.LastSyncTime,
				this.LastSuccessfulSyncTime,
				this.IsInitialSyncComplete
			});
		}

		internal static UpdateMigrationRequestArgs UnMarshal(MdbefPropertyCollection inputArgs)
		{
			byte[] subscriptonMessageIdBytes = MigrationRpcHelper.ReadValue<byte[]>(inputArgs, 2684551426U);
			string text = MigrationRpcHelper.ReadValue<string>(inputArgs, 2684420127U);
			ADObjectId adobjectId = MigrationRpcHelper.ReadADObjectId(inputArgs, 2688811266U);
			StoreObjectId storeObjectId = MigrationRpcHelper.TryDeserializeStoreObjectId(subscriptonMessageIdBytes);
			AggregationStatus aggregationStatus = MigrationRpcHelper.ReadEnum<AggregationStatus>(inputArgs, 2684616707U);
			DetailedAggregationStatus detailedAggregationStatus = DetailedAggregationStatus.None;
			object obj;
			if (inputArgs.TryGetValue(2684682243U, out obj))
			{
				detailedAggregationStatus = (DetailedAggregationStatus)obj;
			}
			bool flag = MigrationRpcHelper.ReadValue<bool>(inputArgs, 2684485643U);
			DateTime? dateTime = null;
			DateTime? dateTime2 = null;
			long dateData;
			if (MigrationRpcHelper.TryReadValue<long>(inputArgs, 2684813332U, out dateData))
			{
				dateTime = new DateTime?(DateTime.FromBinary(dateData));
			}
			if (MigrationRpcHelper.TryReadValue<long>(inputArgs, 2684747796U, out dateData))
			{
				dateTime2 = new DateTime?(DateTime.FromBinary(dateData));
			}
			int num;
			MigrationSubscriptionStatus migrationSubscriptionStatus;
			if (MigrationRpcHelper.TryReadValue<int>(inputArgs, 2684878851U, out num))
			{
				migrationSubscriptionStatus = (MigrationSubscriptionStatus)num;
			}
			else
			{
				migrationSubscriptionStatus = MigrationSubscriptionStatus.None;
			}
			string text2 = MigrationRpcHelper.ReadValue<string>(inputArgs, 2684944415U, null);
			string text3 = MigrationRpcHelper.ReadValue<string>(inputArgs, 2685206559U, null);
			long? num2 = null;
			long value;
			if (MigrationRpcHelper.TryReadValue<long>(inputArgs, 2685009940U, out value))
			{
				num2 = new long?(value);
			}
			long? num3 = null;
			long value2;
			if (MigrationRpcHelper.TryReadValue<long>(inputArgs, 2685075476U, out value2))
			{
				num3 = new long?(value2);
			}
			DateTime? dateTime3 = null;
			long dateData2;
			if (MigrationRpcHelper.TryReadValue<long>(inputArgs, 2685141012U, out dateData2))
			{
				dateTime3 = new DateTime?(DateTime.FromBinary(dateData2));
			}
			return new UpdateMigrationRequestArgs(text2, text3, text, adobjectId, storeObjectId, aggregationStatus, detailedAggregationStatus, migrationSubscriptionStatus, flag, dateTime, dateTime2, num2, num3, dateTime3);
		}

		internal MdbefPropertyCollection Marshal()
		{
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			mdbefPropertyCollection[2684420127U] = this.migrationMailboxLegacyDN;
			mdbefPropertyCollection[2685206559U] = this.UserExchangeMailboxLegacyDN;
			mdbefPropertyCollection[2688811266U] = this.organizationalUnit.GetBytes();
			mdbefPropertyCollection[2684551426U] = MigrationRpcHelper.SerializeStoreObjectId(this.subscriptionMessageId);
			mdbefPropertyCollection[2684616707U] = (int)this.subscriptionStatus;
			mdbefPropertyCollection[2684682243U] = (int)this.subscriptionDetailedStatus;
			mdbefPropertyCollection[2684485643U] = this.initialSyncComplete;
			mdbefPropertyCollection[2684878851U] = (int)this.migrationSubscriptionStatus;
			mdbefPropertyCollection[2684944415U] = this.userExchangeMailboxSmtpAddress;
			if (this.LastSuccessfulSyncTime != null)
			{
				mdbefPropertyCollection[2684747796U] = this.LastSuccessfulSyncTime.Value.ToBinary();
			}
			if (this.LastSyncTime != null)
			{
				mdbefPropertyCollection[2684813332U] = this.LastSyncTime.Value.ToBinary();
			}
			if (this.ItemsSynced != null)
			{
				mdbefPropertyCollection[2685009940U] = this.ItemsSynced.Value;
			}
			if (this.ItemsSkipped != null)
			{
				mdbefPropertyCollection[2685075476U] = this.ItemsSkipped.Value;
			}
			if (this.LastSyncNowRequestTime != null)
			{
				mdbefPropertyCollection[2685141012U] = this.LastSyncNowRequestTime.Value.ToBinary();
			}
			return mdbefPropertyCollection;
		}

		private readonly string migrationMailboxLegacyDN;

		private readonly string userExchangeMailboxSmtpAddress;

		private readonly string userExchangeMailboxLegacyDN;

		private readonly ADObjectId organizationalUnit;

		private readonly bool initialSyncComplete;

		private readonly StoreObjectId subscriptionMessageId;

		private readonly AggregationStatus subscriptionStatus;

		private readonly DetailedAggregationStatus subscriptionDetailedStatus;

		private readonly MigrationSubscriptionStatus migrationSubscriptionStatus;

		private readonly DateTime? lastSyncTime;

		private readonly DateTime? lastSuccessfulSyncTime;

		private readonly long? itemsSynced;

		private readonly long? itemsSkipped;

		private readonly DateTime? lastSyncNowRequestTime;
	}
}
