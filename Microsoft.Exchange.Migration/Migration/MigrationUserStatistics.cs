using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[Serializable]
	public sealed class MigrationUserStatistics : MigrationUser
	{
		public new MigrationUserId Identity
		{
			get
			{
				return base.Identity;
			}
			internal set
			{
				base.Identity = value;
			}
		}

		public new MigrationBatchId BatchId
		{
			get
			{
				return base.BatchId;
			}
			internal set
			{
				base.BatchId = value;
			}
		}

		public new SmtpAddress EmailAddress
		{
			get
			{
				return base.EmailAddress;
			}
			internal set
			{
				base.EmailAddress = value;
			}
		}

		public new MigrationUserRecipientType RecipientType
		{
			get
			{
				return base.RecipientType;
			}
			internal set
			{
				base.RecipientType = value;
			}
		}

		public new long SkippedItemCount
		{
			get
			{
				return base.SkippedItemCount;
			}
			internal set
			{
				base.SkippedItemCount = value;
			}
		}

		public long? TotalItemsInSourceMailboxCount
		{
			get
			{
				return (long?)this[MigrationUserStatisticsSchema.TotalItemsInSourceMailboxCount];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.TotalItemsInSourceMailboxCount] = value;
			}
		}

		public new long SyncedItemCount
		{
			get
			{
				return base.SyncedItemCount;
			}
			internal set
			{
				base.SyncedItemCount = value;
			}
		}

		public new Guid MailboxGuid
		{
			get
			{
				return base.MailboxGuid;
			}
			internal set
			{
				base.MailboxGuid = value;
			}
		}

		public new string MailboxLegacyDN
		{
			get
			{
				return base.MailboxLegacyDN;
			}
			internal set
			{
				base.MailboxLegacyDN = value;
			}
		}

		public new Guid RequestGuid
		{
			get
			{
				return base.RequestGuid;
			}
			internal set
			{
				base.RequestGuid = value;
			}
		}

		public new DateTime? LastSuccessfulSyncTime
		{
			get
			{
				return base.LastSuccessfulSyncTime;
			}
			internal set
			{
				base.LastSuccessfulSyncTime = value;
			}
		}

		public new MigrationUserStatus Status
		{
			get
			{
				return base.Status;
			}
			internal set
			{
				base.Status = value;
			}
		}

		public new MigrationUserStatusSummary StatusSummary
		{
			get
			{
				return base.StatusSummary;
			}
		}

		public new DateTime? LastSubscriptionCheckTime
		{
			get
			{
				return base.LastSubscriptionCheckTime;
			}
			internal set
			{
				base.LastSubscriptionCheckTime = value;
			}
		}

		public EnhancedTimeSpan? TotalQueuedDuration
		{
			get
			{
				return (EnhancedTimeSpan?)this[MigrationUserStatisticsSchema.TotalQueuedDuration];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.TotalQueuedDuration] = value;
			}
		}

		public EnhancedTimeSpan? TotalInProgressDuration
		{
			get
			{
				return (EnhancedTimeSpan?)this[MigrationUserStatisticsSchema.TotalInProgressDuration];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.TotalInProgressDuration] = value;
			}
		}

		public EnhancedTimeSpan? TotalSyncedDuration
		{
			get
			{
				return (EnhancedTimeSpan?)this[MigrationUserStatisticsSchema.TotalSyncedDuration];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.TotalSyncedDuration] = value;
			}
		}

		public EnhancedTimeSpan? TotalStalledDuration
		{
			get
			{
				return (EnhancedTimeSpan?)this[MigrationUserStatisticsSchema.TotalStalledDuration];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.TotalStalledDuration] = value;
			}
		}

		public FailureRec Error
		{
			get
			{
				return (FailureRec)this[MigrationUserStatisticsSchema.Error];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.Error] = value;
			}
		}

		public MigrationType MigrationType
		{
			get
			{
				return (MigrationType)this[MigrationUserStatisticsSchema.MigrationType];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.MigrationType] = value;
			}
		}

		public ByteQuantifiedSize? EstimatedTotalTransferSize
		{
			get
			{
				return (ByteQuantifiedSize?)this[MigrationUserStatisticsSchema.EstimatedTotalTransferSize];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.EstimatedTotalTransferSize] = value;
			}
		}

		public ulong? EstimatedTotalTransferCount
		{
			get
			{
				return (ulong?)this[MigrationUserStatisticsSchema.EstimatedTotalTransferCount];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.EstimatedTotalTransferCount] = value;
			}
		}

		public ByteQuantifiedSize? BytesTransferred
		{
			get
			{
				return (ByteQuantifiedSize?)this[MigrationUserStatisticsSchema.BytesTransferred];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.BytesTransferred] = value;
			}
		}

		public ByteQuantifiedSize? AverageBytesTransferredPerHour
		{
			get
			{
				return (ByteQuantifiedSize?)this[MigrationUserStatisticsSchema.AverageBytesTransferredPerHour];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.AverageBytesTransferredPerHour] = value;
			}
		}

		public ByteQuantifiedSize? CurrentBytesTransferredPerMinute
		{
			get
			{
				return (ByteQuantifiedSize?)this[MigrationUserStatisticsSchema.CurrentBytesTransferredPerMinute];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.CurrentBytesTransferredPerMinute] = value;
			}
		}

		public int? PercentageComplete
		{
			get
			{
				return (int?)this[MigrationUserStatisticsSchema.PercentageComplete];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.PercentageComplete] = value;
			}
		}

		public MultiValuedProperty<MigrationUserSkippedItem> SkippedItems
		{
			get
			{
				return (MultiValuedProperty<MigrationUserSkippedItem>)this[MigrationUserStatisticsSchema.SkippedItems];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.SkippedItems] = value;
			}
		}

		public string DiagnosticInfo
		{
			get
			{
				return (string)this[MigrationUserStatisticsSchema.DiagnosticInfo];
			}
			internal set
			{
				this[MigrationUserStatisticsSchema.DiagnosticInfo] = value;
			}
		}

		public Report Report { get; internal set; }

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MigrationUserStatistics.schema;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is MigrationUserStatistics && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static MigrationUserStatisticsSchema schema = ObjectSchema.GetInstance<MigrationUserStatisticsSchema>();
	}
}
