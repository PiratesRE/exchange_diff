using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	public class CommonSettings
	{
		[DataMember]
		public string InstanceProcessName { get; set; }

		[DataMember]
		public int TruncationLimit { get; set; }

		[DataMember]
		public int TruncationPaddingLength { get; set; }

		[DataMember]
		public int AccessEndpointPortNumber { get; set; }

		[DataMember]
		public string AccessEndpointProtocolName { get; set; }

		[DataMember]
		public int InstanceEndpointPortNumber { get; set; }

		[DataMember]
		public string InstanceEndpointProtocolName { get; set; }

		[DataMember]
		public int MaxEntriesToKeep { get; set; }

		[DataMember]
		public int MaximumAllowedInstanceNumberLag { get; set; }

		[DataMember]
		public int DefaultHealthCheckRequiredNodePercent { get; set; }

		[DataMember]
		public int MaxAllowedLagToCatchup { get; set; }

		[DataMember]
		public string DefaultSnapshotFileName { get; set; }

		[DataMember]
		public bool IsAllowDynamicReconfig { get; set; }

		[DataMember]
		public bool IsAppendOnlyMembership { get; set; }

		[DataMember]
		public bool IsKillInstanceProcessWhenParentDies { get; set; }

		[DataMember]
		public WcfTimeout StoreAccessWcfTimeout { get; set; }

		[DataMember]
		public int StoreAccessHttpTimeoutInMSec { get; set; }

		[DataMember]
		public WcfTimeout StoreInstanceWcfTimeout { get; set; }

		[DataMember]
		public TimeSpan TruncationPeriodicCheckInterval { get; set; }

		[DataMember]
		public TimeSpan InstanceHealthCheckPeriodicInterval { get; set; }

		[DataMember]
		public TimeSpan DurationToWaitBeforeRestart { get; set; }

		[DataMember]
		public TimeSpan StateMachineStopTimeout { get; set; }

		[DataMember]
		public TimeSpan LeaderPromotionTimeout { get; set; }

		[DataMember]
		public TimeSpan PaxosCommandExecutionTimeout { get; set; }

		[DataMember]
		public TimeSpan GroupHealthCheckDuration { get; set; }

		[DataMember]
		public TimeSpan GroupHealthCheckAggressiveDuration { get; set; }

		[DataMember]
		public TimeSpan GroupStatusWaitTimeout { get; set; }

		[DataMember]
		public TimeSpan MemberReconfigureTimeout { get; set; }

		[DataMember]
		public TimeSpan PaxosUpdateTimeout { get; set; }

		[DataMember]
		public TimeSpan SnapshotUpdateInterval { get; set; }

		[DataMember]
		public TimeSpan PeriodicExceptionLoggingDuration { get; set; }

		[DataMember]
		public TimeSpan PeriodicTimeoutLoggingDuration { get; set; }

		[DataMember]
		public TimeSpan ServiceHostCloseTimeout { get; set; }

		[DataMember]
		public TimeSpan InstanceStartSilenceDuration { get; set; }

		[DataMember]
		public int InstanceStartHoldupDurationMaxAllowedStarts { get; set; }

		[DataMember]
		public TimeSpan InstanceStartHoldUpDuration { get; set; }

		[DataMember]
		public int InstanceMemoryCommitSizeLimitInMb { get; set; }

		[DataMember]
		public int AdditionalLogOptionsAsInt { get; set; }

		[DataMember]
		public bool IsUseHttpTransportForInstanceCommunication { get; set; }

		[DataMember]
		public bool IsUseHttpTransportForClientCommunication { get; set; }

		[DataMember]
		public bool IsUseBinarySerializerForClientCommunication { get; set; }

		[DataMember]
		public bool IsUseEncryption { get; set; }

		[DataMember]
		public TimeSpan StartupDelay { get; set; }

		[IgnoreDataMember]
		public LogOptions AdditionalLogOptions
		{
			get
			{
				return (LogOptions)this.AdditionalLogOptionsAsInt;
			}
			set
			{
				this.AdditionalLogOptionsAsInt = (int)value;
			}
		}

		public static class PropertyNames
		{
			public const string AccessEndpointPortNumber = "AccessEndpointPortNumber";

			public const string AccessEndpointProtocolName = "AccessEndpointProtocolName";

			public const string AdditionalLogOptions = "AdditionalLogOptions";

			public const string DefaultHealthCheckRequiredNodePercent = "DefaultHealthCheckRequiredNodePercent";

			public const string DefaultSnapshotFileName = "DefaultSnapshotFileName";

			public const string DurationToWaitBeforeRestartInMSec = "DurationToWaitBeforeRestartInMSec";

			public const string GroupHealthCheckDurationInMSec = "GroupHealthCheckDurationInMSec";

			public const string GroupHealthCheckAggressiveDurationInMSec = "GroupHealthCheckAggressiveDurationInMSec";

			public const string GroupStatusWaitTimeoutInMSec = "GroupStatusWaitTimeoutInMSec";

			public const string InstanceEndpointPortNumber = "InstanceEndpointPortNumber";

			public const string InstanceEndpointProtocolName = "InstanceEndpointProtocolName";

			public const string InstanceHealthCheckPeriodicIntervalInMSec = "InstanceHealthCheckPeriodicIntervalInMSec";

			public const string InstanceProcessName = "InstanceProcessName";

			public const string IsAllowDynamicReconfig = "IsAllowDynamicReconfig";

			public const string IsAppendOnlyMembership = "IsAppendOnlyMembership";

			public const string IsKillInstanceProcessWhenParentDies = "IsKillInstanceProcessWhenParentDies";

			public const string LeaderPromotionTimeoutInMSec = "LeaderPromotionTimeoutInMSec";

			public const string MaxAllowedLagToCatchup = "MaxAllowedLagToCatchup";

			public const string MaxEntriesToKeep = "MaxEntriesToKeep";

			public const string MaximumAllowedInstanceNumberLag = "MaximumAllowedInstanceNumberLag";

			public const string MemberReconfigureTimeoutInMSec = "MemberReconfigureTimeoutInMSec";

			public const string PaxosCommandExecutionTimeoutInMSec = "PaxosCommandExecutionTimeoutInMSec";

			public const string PaxosUpdateTimeoutInMSec = "PaxosUpdateTimeoutInMSec";

			public const string PeriodicExceptionLoggingDurationInMSec = "PeriodicExceptionLoggingDurationInMSec";

			public const string PeriodicTimeoutLoggingDurationInMSec = "PeriodicTimeoutLoggingDurationInMSec";

			public const string ServiceHostCloseTimeoutInMSec = "ServiceHostCloseTimeoutInMSec";

			public const string SnapshotUpdateIntervalInMSec = "SnapshotUpdateIntervalInMSec";

			public const string StateMachineStopTimeoutInMSec = "StateMachineStopTimeoutInMSec";

			public const string StoreAccessWcfTimeout = "StoreAccessWcfTimeout";

			public const string StoreInstanceWcfTimeout = "StoreInstanceWcfTimeout";

			public const string StoreAccessHttpTimeoutInMSec = "StoreAccessHttpTimeoutInMSec";

			public const string TruncationLimit = "TruncationLimit";

			public const string TruncationPaddingLength = "TruncationPaddingLength";

			public const string TruncationPeriodicCheckIntervalInMSec = "TruncationPeriodicCheckIntervalInMSec";

			public const string InstanceStartSilenceDurationInMSec = "InstanceStartSilenceDurationInMSec";

			public const string InstanceStartHoldupDurationMaxAllowedStarts = "InstanceStartHoldupDurationMaxAllowedStarts";

			public const string InstanceStartHoldUpDurationInMSec = "InstanceStartHoldUpDurationInMSec";

			public const string InstanceMemoryCommitSizeLimitInMb = "InstanceMemoryCommitSizeLimitInMb";

			public const string IsUseHttpTransportForInstanceCommunication = "IsUseHttpTransportForInstanceCommunication";

			public const string IsUseHttpTransportForClientCommunication = "IsUseHttpTransportForClientCommunication";

			public const string IsUseBinarySerializerForClientCommunication = "IsUseBinarySerializerForClientCommunication";

			public const string IsUseEncryption = "IsUseEncryption";

			public const string StartupDelayInMSec = "StartupDelayInMSec";
		}
	}
}
