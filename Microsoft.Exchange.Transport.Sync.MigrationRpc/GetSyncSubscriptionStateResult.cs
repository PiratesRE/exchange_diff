using System;
using System.Globalization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetSyncSubscriptionStateResult : MigrationServiceRpcResult, ISubscriptionStatus
	{
		internal GetSyncSubscriptionStateResult(MdbefPropertyCollection args, MigrationServiceRpcMethodCode expectedMethodCode) : base(args)
		{
			object obj;
			if (args.TryGetValue(2936143875U, out obj))
			{
				this.status = (AggregationStatus)obj;
			}
			if (args.TryGetValue(2936209411U, out obj))
			{
				this.detailedStatus = (DetailedAggregationStatus)obj;
			}
			if (args.TryGetValue(2936406027U, out obj))
			{
				this.isInitialSyncComplete = (bool)obj;
			}
			if (args.TryGetValue(2936537108U, out obj))
			{
				this.lastSyncTime = new DateTime?(DateTime.FromBinary((long)obj));
			}
			if (args.TryGetValue(2936471572U, out obj))
			{
				this.lastSuccessfulSyncTime = new DateTime?(DateTime.FromBinary((long)obj));
			}
			if (args.TryGetValue(2936602644U, out obj))
			{
				this.itemsSynced = new long?((long)obj);
			}
			if (args.TryGetValue(2936668180U, out obj))
			{
				this.itemsSkipped = new long?((long)obj);
			}
			if (args.TryGetValue(2936733716U, out obj))
			{
				this.lastSyncNowRequestTime = new DateTime?(DateTime.FromBinary((long)obj));
			}
			base.ThrowIfVerifyFails(expectedMethodCode);
		}

		internal GetSyncSubscriptionStateResult(MigrationServiceRpcMethodCode methodCode, AggregationStatus status, DetailedAggregationStatus substatus, MigrationSubscriptionStatus migrationSubscriptionStatus, bool isInitialSyncComplete, DateTime? lastSyncTime, DateTime? lastSuccessfulSyncTime, long? itemsSynced, long? itemsSkipped, DateTime? lastSyncNowRequestTime) : base(methodCode)
		{
			this.status = status;
			this.detailedStatus = substatus;
			this.isInitialSyncComplete = isInitialSyncComplete;
			this.lastSyncTime = lastSyncTime;
			this.lastSuccessfulSyncTime = lastSuccessfulSyncTime;
			this.itemsSynced = itemsSynced;
			this.itemsSkipped = itemsSkipped;
			this.migrationStatus = migrationSubscriptionStatus;
			this.lastSyncNowRequestTime = lastSyncNowRequestTime;
		}

		internal GetSyncSubscriptionStateResult(MigrationServiceRpcMethodCode methodCode, MigrationServiceRpcResultCode resultCode, string errorDetails) : base(methodCode, resultCode, errorDetails)
		{
		}

		public AggregationStatus Status
		{
			get
			{
				return this.status;
			}
		}

		public DetailedAggregationStatus SubStatus
		{
			get
			{
				return this.detailedStatus;
			}
		}

		public bool IsInitialSyncComplete
		{
			get
			{
				return this.isInitialSyncComplete;
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

		public MigrationSubscriptionStatus MigrationSubscriptionStatus
		{
			get
			{
				return this.migrationStatus;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "GetSyncSubscriptionStateResult: Status: [{0}], Substatus: [{1}], LastSync: [{2}], LastGoodSync: [{3}], InitialSyncDone: [{4}]", new object[]
			{
				this.Status,
				this.SubStatus,
				this.LastSyncTime,
				this.LastSuccessfulSyncTime,
				this.IsInitialSyncComplete
			});
		}

		protected override void WriteTo(MdbefPropertyCollection collection)
		{
			collection[2936143875U] = (int)this.status;
			collection[2936209411U] = (int)this.detailedStatus;
			collection[2936406027U] = this.isInitialSyncComplete;
			if (this.lastSyncTime != null)
			{
				collection[2936537108U] = this.LastSyncTime.Value.ToBinary();
			}
			if (this.lastSuccessfulSyncTime != null)
			{
				collection[2936471572U] = this.LastSuccessfulSyncTime.Value.ToBinary();
			}
			if (this.itemsSynced != null)
			{
				collection[2936602644U] = this.itemsSynced.Value;
			}
			if (this.itemsSkipped != null)
			{
				collection[2936668180U] = this.itemsSkipped.Value;
			}
			if (this.lastSyncNowRequestTime != null)
			{
				collection[2936733716U] = this.lastSyncNowRequestTime.Value.ToBinary();
			}
		}

		private readonly AggregationStatus status;

		private readonly DetailedAggregationStatus detailedStatus;

		private readonly MigrationSubscriptionStatus migrationStatus = MigrationSubscriptionStatus.None;

		private readonly DateTime? lastSyncTime;

		private readonly DateTime? lastSuccessfulSyncTime;

		private readonly bool isInitialSyncComplete;

		private readonly long? itemsSynced;

		private readonly long? itemsSkipped;

		private readonly DateTime? lastSyncNowRequestTime;
	}
}
