using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SubscriptionSnapshot : ISubscriptionStatistics, IStepSnapshot, IMigrationSerializable
	{
		public SubscriptionSnapshot(ISubscriptionId id, SnapshotStatus status, bool isInitialSyncComplete, ExDateTime createTime, ExDateTime? lastUpdateTime, ExDateTime? lastSyncTime, LocalizedString? errorMessage, string batchName)
		{
			this.id = id;
			this.Status = status;
			this.IsInitialSyncComplete = isInitialSyncComplete;
			this.CreateTime = createTime;
			this.LastUpdateTime = lastUpdateTime;
			this.LastSyncTime = lastSyncTime;
			this.ErrorMessage = errorMessage;
			this.BatchName = batchName;
		}

		protected SubscriptionSnapshot()
		{
		}

		public ISnapshotId Id
		{
			get
			{
				return this.id;
			}
			protected set
			{
				this.id = (ISubscriptionId)value;
			}
		}

		public SnapshotStatus Status
		{
			get
			{
				return this.status;
			}
			protected set
			{
				this.status = value;
			}
		}

		public bool IsInitialSyncComplete
		{
			get
			{
				return this.isInitialSyncComplete;
			}
			protected set
			{
				this.isInitialSyncComplete = value;
			}
		}

		public ExDateTime CreateTime
		{
			get
			{
				return this.createTime;
			}
			protected set
			{
				this.createTime = value;
			}
		}

		public ExDateTime? LastUpdateTime
		{
			get
			{
				return this.lastUpdateTime;
			}
			protected set
			{
				this.lastUpdateTime = value;
			}
		}

		public ExDateTime? LastSyncTime
		{
			get
			{
				return this.lastSyncTime;
			}
			protected set
			{
				this.lastSyncTime = value;
			}
		}

		public LocalizedString? ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			protected set
			{
				this.errorMessage = value;
			}
		}

		public ExDateTime? InjectionCompletedTime
		{
			get
			{
				return new ExDateTime?(this.CreateTime);
			}
		}

		public long NumItemsSkipped
		{
			get
			{
				return this.numItemsSkipped;
			}
			protected set
			{
				this.numItemsSkipped = value;
			}
		}

		public long NumItemsSynced
		{
			get
			{
				return this.numItemsSynced;
			}
			protected set
			{
				this.numItemsSynced = value;
			}
		}

		public long? NumTotalItemsInMailbox
		{
			get
			{
				return this.numTotalItemsInMailbox;
			}
			protected set
			{
				this.numTotalItemsInMailbox = value;
			}
		}

		public EnhancedTimeSpan? TotalQueuedDuration
		{
			get
			{
				return this.totalQueuedDuration;
			}
			set
			{
				this.totalQueuedDuration = value;
			}
		}

		public EnhancedTimeSpan? TotalInProgressDuration
		{
			get
			{
				return this.totalInProgressDuration;
			}
			set
			{
				this.totalInProgressDuration = value;
			}
		}

		public EnhancedTimeSpan? TotalSyncedDuration
		{
			get
			{
				return this.totalSyncedDuration;
			}
			set
			{
				this.totalSyncedDuration = value;
			}
		}

		public EnhancedTimeSpan? TotalStalledDuration
		{
			get
			{
				return this.totalStalledDuration;
			}
			set
			{
				this.totalStalledDuration = value;
			}
		}

		public ByteQuantifiedSize? EstimatedTotalTransferSize
		{
			get
			{
				return this.estimatedTotalTransferSize;
			}
			set
			{
				this.estimatedTotalTransferSize = value;
			}
		}

		public ulong? EstimatedTotalTransferCount
		{
			get
			{
				return this.estimatedTotalTransferCount;
			}
			set
			{
				this.estimatedTotalTransferCount = value;
			}
		}

		public ByteQuantifiedSize? BytesTransferred
		{
			get
			{
				return this.bytesTransferred;
			}
			set
			{
				this.bytesTransferred = value;
			}
		}

		public ByteQuantifiedSize? AverageBytesTransferredPerHour
		{
			get
			{
				return this.averageBytesTransferredPerHour;
			}
			set
			{
				this.averageBytesTransferredPerHour = value;
			}
		}

		public ByteQuantifiedSize? CurrentBytesTransferredPerMinute
		{
			get
			{
				return this.currentBytesTransferredPerMinute;
			}
			set
			{
				this.currentBytesTransferredPerMinute = value;
			}
		}

		public int? PercentageComplete
		{
			get
			{
				return this.percentageComplete;
			}
			set
			{
				this.percentageComplete = value;
			}
		}

		public Report Report
		{
			get
			{
				return this.report;
			}
			set
			{
				this.report = value;
			}
		}

		public string BatchName { get; protected set; }

		public PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return new PropertyDefinition[]
				{
					MigrationBatchMessageSchema.MigrationJobItemItemsSynced,
					MigrationBatchMessageSchema.MigrationJobItemItemsSkipped,
					MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime
				};
			}
		}

		public static SubscriptionSnapshot CreateRemoved()
		{
			return new SubscriptionSnapshot
			{
				Status = SnapshotStatus.Removed
			};
		}

		public static SubscriptionSnapshot CreateFailed(LocalizedString errorMessage)
		{
			return new SubscriptionSnapshot
			{
				Status = SnapshotStatus.Failed,
				ErrorMessage = new LocalizedString?(errorMessage)
			};
		}

		public static SubscriptionSnapshot CreateId(ISubscriptionId id)
		{
			return new SubscriptionSnapshot
			{
				id = id,
				Status = SnapshotStatus.InProgress
			};
		}

		public void SetStatistics(long numberItemsSynced, long numberItemsSkipped, long? numberItemsTotal)
		{
			this.NumItemsSynced = numberItemsSynced;
			this.NumItemsSkipped = numberItemsSkipped;
			this.NumTotalItemsInMailbox = numberItemsTotal;
		}

		public bool IsTimedOut(TimeSpan timeout)
		{
			TimeSpan t;
			if (this.LastUpdateTime != null)
			{
				t = ExDateTime.UtcNow - this.LastUpdateTime.Value;
			}
			else
			{
				t = ExDateTime.UtcNow - this.CreateTime;
			}
			return t > timeout;
		}

		public bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.NumItemsSynced = (long)message[MigrationBatchMessageSchema.MigrationJobItemItemsSynced];
			this.NumItemsSkipped = (long)message[MigrationBatchMessageSchema.MigrationJobItemItemsSkipped];
			this.LastSyncTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime);
			return true;
		}

		public void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			message[MigrationBatchMessageSchema.MigrationJobItemItemsSynced] = this.NumItemsSynced;
			message[MigrationBatchMessageSchema.MigrationJobItemItemsSkipped] = this.NumItemsSkipped;
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationLastSuccessfulSyncTime, this.LastSyncTime);
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("SubscriptionSnapshot");
			xelement.Add(new object[]
			{
				new XElement("ItemsSynced", this.NumItemsSynced),
				new XElement("ItemsSkipped", this.NumItemsSkipped),
				new XElement("LastSyncTime", this.LastSyncTime)
			});
			return xelement;
		}

		internal static EnhancedTimeSpan? Subtract(EnhancedTimeSpan? value1, EnhancedTimeSpan? value2)
		{
			if (value1 == null)
			{
				return null;
			}
			if (value2 == null)
			{
				return value1;
			}
			return new EnhancedTimeSpan?(value1.Value - value2.Value);
		}

		internal static SubscriptionSnapshot CreateFromMessage(IMigrationStoreObject message)
		{
			SubscriptionSnapshot subscriptionSnapshot = new SubscriptionSnapshot();
			subscriptionSnapshot.ReadFromMessageItem(message);
			return subscriptionSnapshot;
		}

		private ISubscriptionId id;

		private SnapshotStatus status;

		private bool isInitialSyncComplete;

		private ExDateTime createTime;

		private ExDateTime? lastUpdateTime;

		private ExDateTime? lastSyncTime;

		private EnhancedTimeSpan? totalQueuedDuration;

		private EnhancedTimeSpan? totalInProgressDuration;

		private EnhancedTimeSpan? totalSyncedDuration;

		private EnhancedTimeSpan? totalStalledDuration;

		private LocalizedString? errorMessage;

		private long numItemsSkipped;

		private long numItemsSynced;

		private long? numTotalItemsInMailbox;

		private ByteQuantifiedSize? estimatedTotalTransferSize;

		private ulong? estimatedTotalTransferCount;

		private ByteQuantifiedSize? bytesTransferred;

		private ByteQuantifiedSize? currentBytesTransferredPerMinute;

		private ByteQuantifiedSize? averageBytesTransferredPerHour;

		private int? percentageComplete;

		private Report report;
	}
}
