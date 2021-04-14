using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class PerfmonCounters : IPerfmonCounters
	{
		internal PerfmonCounters(ReplayServicePerfmonInstance perfmonInstance)
		{
			this.m_instance = perfmonInstance;
			this.m_copyQueueLength = new SafeCounter(perfmonInstance.CopyQueueLength);
			this.m_rawCopyQueueLength = new SafeCounter(perfmonInstance.RawCopyQueueLength);
			this.m_replayQueueLength = new SafeCounter(perfmonInstance.ReplayQueueLength);
		}

		public void Reset()
		{
			this.m_copyQueueLength.Reset();
			this.m_rawCopyQueueLength.Reset();
			this.m_replayQueueLength.Reset();
			this.m_instance.Reset();
		}

		public long CopyNotificationGenerationNumber
		{
			get
			{
				return this.m_instance.CopyNotificationGenerationNumber.RawValue;
			}
			set
			{
				this.m_instance.CopyNotificationGenerationNumber.RawValue = value;
				this.m_instance.CopyNotificationGenerationsPerSecond.RawValue = value;
				this.UpdateCopyQueueLength();
				this.UpdateRawCopyQueueLength();
			}
		}

		public long CopyGenerationNumber
		{
			set
			{
				this.m_instance.CopyGenerationNumber.RawValue = value;
				this.UpdateRawCopyQueueLength();
			}
		}

		public long InspectorGenerationNumber
		{
			get
			{
				return this.m_instance.InspectorGenerationNumber.RawValue;
			}
			set
			{
				this.m_instance.InspectorGenerationNumber.RawValue = value;
				this.m_instance.InspectorGenerationsPerSecond.RawValue = value;
				this.UpdateReplayQueueLength();
				this.UpdateCopyQueueLength();
			}
		}

		public float CopyNotificationGenerationsPerSecond
		{
			get
			{
				return this.m_instance.CopyNotificationGenerationsPerSecond.NextValue();
			}
		}

		public float InspectorGenerationsPerSecond
		{
			get
			{
				return this.m_instance.InspectorGenerationsPerSecond.NextValue();
			}
		}

		public long CopyQueueLength
		{
			get
			{
				return this.m_copyQueueLength.Value;
			}
		}

		public long RawCopyQueueLength
		{
			get
			{
				return this.m_rawCopyQueueLength.Value;
			}
		}

		public long CopyQueueNotKeepingUp
		{
			get
			{
				return this.m_instance.CopyQueueNotKeepingUp.RawValue;
			}
			set
			{
				this.m_instance.CopyQueueNotKeepingUp.RawValue = value;
			}
		}

		public long ReplayNotificationGenerationNumber
		{
			set
			{
				this.m_instance.ReplayNotificationGenerationNumber.RawValue = value;
			}
		}

		public long ReplayGenerationNumber
		{
			get
			{
				return this.m_instance.ReplayGenerationNumber.RawValue;
			}
			set
			{
				this.m_instance.ReplayGenerationNumber.RawValue = value;
				this.m_instance.ReplayGenerationsPerSecond.RawValue = value;
				this.UpdateReplayQueueLength();
			}
		}

		public float ReplayGenerationsPerSecond
		{
			get
			{
				return this.m_instance.ReplayGenerationsPerSecond.NextValue();
			}
		}

		public long ReplayQueueLength
		{
			get
			{
				return this.m_replayQueueLength.Value;
			}
		}

		public long ReplayQueueNotKeepingUp
		{
			get
			{
				return this.m_instance.ReplayQueueNotKeepingUp.RawValue;
			}
			set
			{
				this.m_instance.ReplayQueueNotKeepingUp.RawValue = value;
			}
		}

		public long Failed
		{
			get
			{
				return this.m_instance.Failed.RawValue;
			}
			set
			{
				this.m_instance.Failed.RawValue = value;
			}
		}

		public long Initializing
		{
			get
			{
				return this.m_instance.Initializing.RawValue;
			}
			set
			{
				this.m_instance.Initializing.RawValue = value;
			}
		}

		public long Resynchronizing
		{
			get
			{
				return this.m_instance.Resynchronizing.RawValue;
			}
			set
			{
				this.m_instance.Resynchronizing.RawValue = value;
			}
		}

		public long ActivationSuspended
		{
			get
			{
				return this.m_instance.ActivationSuspended.RawValue;
			}
			set
			{
				this.m_instance.ActivationSuspended.RawValue = value;
			}
		}

		public long ReplayLagDisabled
		{
			get
			{
				return this.m_instance.ReplayLagDisabled.RawValue;
			}
			set
			{
				this.m_instance.ReplayLagDisabled.RawValue = value;
			}
		}

		public long ReplayLagPercentage
		{
			get
			{
				return this.m_instance.ReplayLagPercentage.RawValue;
			}
			set
			{
				this.m_instance.ReplayLagPercentage.RawValue = value;
			}
		}

		public long FailedSuspended
		{
			get
			{
				return this.m_instance.FailedSuspended.RawValue;
			}
			set
			{
				this.m_instance.FailedSuspended.RawValue = value;
			}
		}

		public long SinglePageRestore
		{
			get
			{
				return this.m_instance.SinglePageRestore.RawValue;
			}
			set
			{
				this.m_instance.SinglePageRestore.RawValue = value;
			}
		}

		public long Disconnected
		{
			get
			{
				return this.m_instance.Disconnected.RawValue;
			}
			set
			{
				this.m_instance.Disconnected.RawValue = value;
			}
		}

		public long Suspended
		{
			get
			{
				return this.m_instance.Suspended.RawValue;
			}
			set
			{
				this.m_instance.Suspended.RawValue = value;
			}
		}

		public long SuspendedAndNotSeeding
		{
			get
			{
				return this.m_instance.SuspendedAndNotSeeding.RawValue;
			}
			set
			{
				this.m_instance.SuspendedAndNotSeeding.RawValue = value;
			}
		}

		public long Seeding
		{
			get
			{
				return this.m_instance.Seeding.RawValue;
			}
			set
			{
				this.m_instance.Seeding.RawValue = value;
			}
		}

		public long CompressionEnabled
		{
			get
			{
				return this.m_instance.CompressionEnabled.RawValue;
			}
			set
			{
				this.m_instance.CompressionEnabled.RawValue = value;
			}
		}

		public long EncryptionEnabled
		{
			get
			{
				return this.m_instance.EncryptionEnabled.RawValue;
			}
			set
			{
				this.m_instance.EncryptionEnabled.RawValue = value;
			}
		}

		public long TruncatedGenerationNumber
		{
			set
			{
				this.m_instance.TruncatedGenerationNumber.RawValue = value;
			}
		}

		public long IncReseedDBPagesReadNumber
		{
			set
			{
				this.m_instance.IncReseedDBPagesReadNumber.RawValue = value;
			}
		}

		public long IncReseedLogCopiedNumber
		{
			set
			{
				this.m_instance.IncReseedLogCopiedNumber.RawValue = value;
			}
		}

		public long GranularReplication
		{
			get
			{
				return this.m_instance.GranularReplication.RawValue;
			}
			set
			{
				this.m_instance.GranularReplication.RawValue = value;
			}
		}

		public void RecordLogCopierNetworkReadLatency(long tics)
		{
			this.m_instance.AvgLogCopyNetReadLatency.IncrementBy(tics);
			this.m_instance.AvgLogCopyNetReadLatencyBase.Increment();
		}

		public void RecordBlockModeConsumerWriteLatency(long tics)
		{
			this.m_instance.AvgBlockModeConsumerWriteTime.IncrementBy(tics);
			this.m_instance.AvgBlockModeConsumerWriteTimeBase.Increment();
		}

		public void RecordFileModeWriteLatency(long tics)
		{
			this.m_instance.AvgFileModeWriteTime.IncrementBy(tics);
			this.m_instance.AvgFileModeWriteTimeBase.Increment();
		}

		public long PassiveSeedingSource
		{
			get
			{
				return this.m_instance.PassiveSeedingSource.RawValue;
			}
			set
			{
				this.m_instance.PassiveSeedingSource.RawValue = value;
			}
		}

		public void RecordLogCopyThruput(long bytesCopied)
		{
			this.m_instance.LogCopyThruput.IncrementBy(bytesCopied / 1024L);
		}

		public void RecordGranularBytesReceived(long bytesCopied, bool logIsComplete)
		{
			this.m_instance.LogCopyThruput.IncrementBy(bytesCopied / 1024L);
			this.m_instance.TotalGranularBytesReceived.IncrementBy(bytesCopied);
			if (logIsComplete)
			{
				this.m_granularLogsReceived += 1L;
				this.m_instance.AverageGranularBytesPerLog.RawValue = this.m_instance.TotalGranularBytesReceived.RawValue / this.m_granularLogsReceived;
			}
		}

		public void RecordOneGetCopyStatusCall()
		{
			this.m_instance.GetCopyStatusInstanceCalls.Increment();
			this.m_instance.GetCopyStatusInstanceCallsPerSec.Increment();
		}

		private void UpdateCopyQueueLength()
		{
			long num = Math.Max(this.m_instance.InspectorGenerationNumber.RawValue - 1L, this.m_instance.CopyNotificationGenerationNumber.RawValue) - this.m_instance.InspectorGenerationNumber.RawValue;
			this.m_copyQueueLength.Value = ((num > 0L) ? num : 0L);
		}

		private void UpdateRawCopyQueueLength()
		{
			long num = this.m_instance.CopyNotificationGenerationNumber.RawValue - this.m_instance.CopyGenerationNumber.RawValue;
			this.m_rawCopyQueueLength.Value = ((num > 0L) ? num : 0L);
		}

		private void UpdateReplayQueueLength()
		{
			this.m_replayQueueLength.Value = Math.Max(0L, this.m_instance.InspectorGenerationNumber.RawValue - this.m_instance.ReplayGenerationNumber.RawValue);
		}

		private long m_granularLogsReceived;

		private ReplayServicePerfmonInstance m_instance;

		private SafeCounter m_copyQueueLength;

		private SafeCounter m_rawCopyQueueLength;

		private SafeCounter m_replayQueueLength;
	}
}
