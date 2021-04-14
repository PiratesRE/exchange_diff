using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IPerfmonCounters
	{
		void Reset();

		long CopyNotificationGenerationNumber { get; set; }

		long CopyGenerationNumber { set; }

		float CopyNotificationGenerationsPerSecond { get; }

		float InspectorGenerationsPerSecond { get; }

		long InspectorGenerationNumber { get; set; }

		long CopyQueueLength { get; }

		long CopyQueueNotKeepingUp { get; set; }

		long ReplayNotificationGenerationNumber { set; }

		long ReplayGenerationNumber { get; set; }

		float ReplayGenerationsPerSecond { get; }

		long ReplayQueueLength { get; }

		long ReplayQueueNotKeepingUp { get; set; }

		long Failed { get; set; }

		long Initializing { get; set; }

		long FailedSuspended { get; set; }

		long Resynchronizing { get; set; }

		long ActivationSuspended { get; set; }

		long ReplayLagDisabled { get; set; }

		long ReplayLagPercentage { get; set; }

		long Disconnected { get; set; }

		long PassiveSeedingSource { get; set; }

		long Seeding { get; set; }

		long SinglePageRestore { get; set; }

		long Suspended { get; set; }

		long SuspendedAndNotSeeding { get; set; }

		long CompressionEnabled { get; set; }

		long EncryptionEnabled { get; set; }

		long TruncatedGenerationNumber { set; }

		long IncReseedDBPagesReadNumber { set; }

		long IncReseedLogCopiedNumber { set; }

		void RecordOneGetCopyStatusCall();

		void RecordLogCopyThruput(long bytesCopied);

		long RawCopyQueueLength { get; }

		long GranularReplication { get; set; }

		void RecordGranularBytesReceived(long bytesCopied, bool logIsComplete);

		void RecordLogCopierNetworkReadLatency(long tics);

		void RecordBlockModeConsumerWriteLatency(long tics);

		void RecordFileModeWriteLatency(long tics);
	}
}
