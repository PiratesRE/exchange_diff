using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NullPerfmonCounters : IPerfmonCounters
	{
		public void Reset()
		{
		}

		public long CopyNotificationGenerationNumber
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long CopyGenerationNumber
		{
			set
			{
			}
		}

		public float CopyNotificationGenerationsPerSecond
		{
			get
			{
				return 0f;
			}
		}

		public float InspectorGenerationsPerSecond
		{
			get
			{
				return 0f;
			}
		}

		public long InspectorGenerationNumber
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long CopyQueueLength
		{
			get
			{
				return 0L;
			}
		}

		public long RawCopyQueueLength
		{
			get
			{
				return 0L;
			}
		}

		public long CopyQueueNotKeepingUp
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long ReplayNotificationGenerationNumber
		{
			set
			{
			}
		}

		public long ReplayGenerationNumber
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public float ReplayGenerationsPerSecond
		{
			get
			{
				return 0f;
			}
		}

		public long ReplayQueueLength
		{
			get
			{
				return 0L;
			}
		}

		public long ReplayQueueNotKeepingUp
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long Failed
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long Initializing
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long Resynchronizing
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long ActivationSuspended
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long ReplayLagDisabled
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long ReplayLagPercentage
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long FailedSuspended
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long SinglePageRestore
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long Disconnected
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long Suspended
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long SuspendedAndNotSeeding
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long Seeding
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long CompressionEnabled { get; set; }

		public long EncryptionEnabled { get; set; }

		public long TruncatedGenerationNumber
		{
			set
			{
			}
		}

		public long IncReseedDBPagesReadNumber
		{
			set
			{
			}
		}

		public long IncReseedLogCopiedNumber
		{
			set
			{
			}
		}

		public void RecordLogCopyThruput(long bytesCopied)
		{
		}

		public long GranularReplication
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public void RecordLogCopierNetworkReadLatency(long tics)
		{
		}

		public void RecordBlockModeConsumerWriteLatency(long tics)
		{
		}

		public void RecordFileModeWriteLatency(long tics)
		{
		}

		public long PassiveSeedingSource
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public void RecordGranularBytesReceived(long bytesCopied, bool logIsComplete)
		{
		}

		public void RecordOneGetCopyStatusCall()
		{
		}
	}
}
