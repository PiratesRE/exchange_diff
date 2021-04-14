using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class TenantRelocationConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "TenantRelocation";
			}
		}

		public override string SectionName
		{
			get
			{
				return "TenantRelocationConfiguration";
			}
		}

		[ConfigurationProperty("IsAllRelocationActivitiesHalted", DefaultValue = true)]
		public bool IsAllRelocationActivitiesHalted
		{
			get
			{
				return (bool)base["IsAllRelocationActivitiesHalted"];
			}
			set
			{
				base["IsAllRelocationActivitiesHalted"] = value;
			}
		}

		[ConfigurationProperty("IsProcessingBrokerEnabled", DefaultValue = false)]
		public bool IsProcessingBrokerEnabled
		{
			get
			{
				return (bool)base["IsProcessingBrokerEnabled"];
			}
			set
			{
				base["IsProcessingBrokerEnabled"] = value;
			}
		}

		[ConfigurationProperty("IsRollbackBrokerEnabled", DefaultValue = false)]
		public bool IsRollbackBrokerEnabled
		{
			get
			{
				return (bool)base["IsRollbackBrokerEnabled"];
			}
			set
			{
				base["IsRollbackBrokerEnabled"] = value;
			}
		}

		[ConfigurationProperty("IsCleanupBrokerEnabled", DefaultValue = false)]
		public bool IsCleanupBrokerEnabled
		{
			get
			{
				return (bool)base["IsCleanupBrokerEnabled"];
			}
			set
			{
				base["IsCleanupBrokerEnabled"] = value;
			}
		}

		[ConfigurationProperty("IsOrchestratorEnabled", DefaultValue = false)]
		public bool IsOrchestratorEnabled
		{
			get
			{
				return (bool)base["IsOrchestratorEnabled"];
			}
			set
			{
				base["IsOrchestratorEnabled"] = value;
			}
		}

		[ConfigurationProperty("DataSyncObjectsPerPageLimit", DefaultValue = "1000")]
		public uint DataSyncObjectsPerPageLimit
		{
			get
			{
				return (uint)base["DataSyncObjectsPerPageLimit"];
			}
			set
			{
				base["DataSyncObjectsPerPageLimit"] = value;
			}
		}

		[ConfigurationProperty("DataSyncLinksPerPageLimit", DefaultValue = "1500")]
		public uint DataSyncLinksPerPageLimit
		{
			get
			{
				return (uint)base["DataSyncLinksPerPageLimit"];
			}
			set
			{
				base["DataSyncLinksPerPageLimit"] = value;
			}
		}

		[ConfigurationProperty("DataSyncInitialLinkReadSize", DefaultValue = "1500")]
		public uint DataSyncInitialLinkReadSize
		{
			get
			{
				return (uint)base["DataSyncInitialLinkReadSize"];
			}
			set
			{
				base["DataSyncInitialLinkReadSize"] = value;
			}
		}

		[ConfigurationProperty("DataSyncFailoverTimeoutInMinutes", DefaultValue = "30")]
		public uint DataSyncFailoverTimeoutInMinutes
		{
			get
			{
				return (uint)base["DataSyncFailoverTimeoutInMinutes"];
			}
			set
			{
				base["DataSyncFailoverTimeoutInMinutes"] = value;
			}
		}

		[ConfigurationProperty("DataSyncLinksOverldapSize", DefaultValue = "100")]
		public uint DataSyncLinksOverldapSize
		{
			get
			{
				return (uint)base["DataSyncLinksOverldapSize"];
			}
			set
			{
				base["DataSyncLinksOverldapSize"] = value;
			}
		}

		[ConfigurationProperty("DeltaSyncUsnRangeLimit", DefaultValue = "1000000")]
		public long DeltaSyncUsnRangeLimit
		{
			get
			{
				return (long)base["DeltaSyncUsnRangeLimit"];
			}
			set
			{
				base["DeltaSyncUsnRangeLimit"] = value;
			}
		}

		[ConfigurationProperty("MaxConcurrentProcessingThreadsPerServer", DefaultValue = "20")]
		public uint MaxConcurrentProcessingThreadsPerServer
		{
			get
			{
				return (uint)base["MaxConcurrentProcessingThreadsPerServer"];
			}
			set
			{
				base["MaxConcurrentProcessingThreadsPerServer"] = value;
			}
		}

		[ConfigurationProperty("MaxConcurrentRollbackThreadsPerServer", DefaultValue = "1")]
		public uint MaxConcurrentRollbackThreadsPerServer
		{
			get
			{
				return (uint)base["MaxConcurrentRollbackThreadsPerServer"];
			}
			set
			{
				base["MaxConcurrentRollbackThreadsPerServer"] = value;
			}
		}

		[ConfigurationProperty("MaxConcurrentCleanupThreadsPerServer", DefaultValue = "1")]
		public uint MaxConcurrentCleanupThreadsPerServer
		{
			get
			{
				return (uint)base["MaxConcurrentCleanupThreadsPerServer"];
			}
			set
			{
				base["MaxConcurrentCleanupThreadsPerServer"] = value;
			}
		}

		[ConfigurationProperty("ProcessingBrokerPollIntervalInMinutes", DefaultValue = "5")]
		public uint ProcessingBrokerPollIntervalInMinutes
		{
			get
			{
				return (uint)base["ProcessingBrokerPollIntervalInMinutes"];
			}
			set
			{
				base["ProcessingBrokerPollIntervalInMinutes"] = value;
			}
		}

		[ConfigurationProperty("RollbackBrokerPollIntervalInMinutes", DefaultValue = "5")]
		public uint RollbackBrokerPollIntervalInMinutes
		{
			get
			{
				return (uint)base["RollbackBrokerPollIntervalInMinutes"];
			}
			set
			{
				base["RollbackBrokerPollIntervalInMinutes"] = value;
			}
		}

		[ConfigurationProperty("IsUserExperienceTestEnabled", DefaultValue = false)]
		public bool IsUserExperienceTestEnabled
		{
			get
			{
				return (bool)base["IsUserExperienceTestEnabled"];
			}
			set
			{
				base["IsUserExperienceTestEnabled"] = value;
			}
		}

		[ConfigurationProperty("DisabledUXProbes", DefaultValue = "")]
		public string DisabledUXProbes
		{
			get
			{
				return (string)base["DisabledUXProbes"];
			}
			set
			{
				base["DisabledUXProbes"] = value;
			}
		}

		[ConfigurationProperty("UXProbeRecurrenceIntervalSeconds", DefaultValue = "150")]
		public uint UXProbeRecurrenceIntervalSeconds
		{
			get
			{
				return (uint)base["UXProbeRecurrenceIntervalSeconds"];
			}
			set
			{
				base["UXProbeRecurrenceIntervalSeconds"] = value;
			}
		}

		[ConfigurationProperty("UXMonitorConsecutiveProbeFailureCount", DefaultValue = "2")]
		public uint UXMonitorConsecutiveProbeFailureCount
		{
			get
			{
				return (uint)base["UXMonitorConsecutiveProbeFailureCount"];
			}
			set
			{
				base["UXMonitorConsecutiveProbeFailureCount"] = value;
			}
		}

		[ConfigurationProperty("UXMonitorAccountExpiredDays", DefaultValue = "15")]
		public uint UXMonitorAccountExpiredDays
		{
			get
			{
				return (uint)base["UXMonitorAccountExpiredDays"];
			}
			set
			{
				base["UXMonitorAccountExpiredDays"] = value;
			}
		}

		[ConfigurationProperty("RemoveUXMonitorAccountWaitReplicationMinutes", DefaultValue = "5")]
		public uint RemoveUXMonitorAccountWaitReplicationMinutes
		{
			get
			{
				return (uint)base["RemoveUXMonitorAccountWaitReplicationMinutes"];
			}
			set
			{
				base["RemoveUXMonitorAccountWaitReplicationMinutes"] = value;
			}
		}

		[ConfigurationProperty("WaitUXFailureResultSeconds", DefaultValue = "30")]
		public uint WaitUXFailureResultSeconds
		{
			get
			{
				return (uint)base["WaitUXFailureResultSeconds"];
			}
			set
			{
				base["WaitUXFailureResultSeconds"] = value;
			}
		}

		[ConfigurationProperty("UXTransportProbeSmtpServers", DefaultValue = "")]
		public string UXTransportProbeSmtpServers
		{
			get
			{
				return (string)base["UXTransportProbeSmtpServers"];
			}
			set
			{
				base["UXTransportProbeSmtpServers"] = value;
			}
		}

		[ConfigurationProperty("UXTransportProbeSmtpPort", DefaultValue = "2525")]
		public uint UXTransportProbeSmtpPort
		{
			get
			{
				return (uint)base["UXTransportProbeSmtpPort"];
			}
			set
			{
				base["UXTransportProbeSmtpPort"] = value;
			}
		}

		[ConfigurationProperty("UXTransportProbeSenderAddress", DefaultValue = "")]
		public string UXTransportProbeSenderAddress
		{
			get
			{
				return (string)base["UXTransportProbeSenderAddress"];
			}
			set
			{
				base["UXTransportProbeSenderAddress"] = value;
			}
		}

		[ConfigurationProperty("UXTransportProbeSendMessageTimeout", DefaultValue = "15")]
		public uint UXTransportProbeSendMessageTimeout
		{
			get
			{
				return (uint)base["UXTransportProbeSendMessageTimeout"];
			}
			set
			{
				base["UXTransportProbeSendMessageTimeout"] = value;
			}
		}

		[ConfigurationProperty("UXTransportProbeWaitMessageTimeout", DefaultValue = "90")]
		public uint UXTransportProbeWaitMessageTimeout
		{
			get
			{
				return (uint)base["UXTransportProbeWaitMessageTimeout"];
			}
			set
			{
				base["UXTransportProbeWaitMessageTimeout"] = value;
			}
		}

		[ConfigurationProperty("CheckStaleRelocations", DefaultValue = true)]
		public bool CheckStaleRelocations
		{
			get
			{
				return (bool)base["CheckStaleRelocations"];
			}
			set
			{
				base["CheckStaleRelocations"] = value;
			}
		}

		[ConfigurationProperty("SafeScheduleWindow", DefaultValue = "DailyFrom1AMTo5AM")]
		public string SafeScheduleWindow
		{
			get
			{
				return (string)base["SafeScheduleWindow"];
			}
			set
			{
				base["SafeScheduleWindow"] = value;
			}
		}

		[ConfigurationProperty("MaxRelocationInNonCriticalStage", DefaultValue = "20")]
		public uint MaxRelocationInNonCriticalStage
		{
			get
			{
				return (uint)base["MaxRelocationInNonCriticalStage"];
			}
			set
			{
				base["MaxRelocationInNonCriticalStage"] = value;
			}
		}

		[ConfigurationProperty("MaxRelocationInCriticalStage", DefaultValue = "10")]
		public uint MaxRelocationInCriticalStage
		{
			get
			{
				return (uint)base["MaxRelocationInCriticalStage"];
			}
			set
			{
				base["MaxRelocationInCriticalStage"] = value;
			}
		}

		[ConfigurationProperty("MaxRelocationInCleanupStage", DefaultValue = "10")]
		public uint MaxRelocationInCleanupStage
		{
			get
			{
				return (uint)base["MaxRelocationInCleanupStage"];
			}
			set
			{
				base["MaxRelocationInCleanupStage"] = value;
			}
		}

		[ConfigurationProperty("OrchestratorSleepIntervalBetweenRetriesInMinutes", DefaultValue = "60")]
		public uint OrchestratorSleepIntervalBetweenRetriesInMinutes
		{
			get
			{
				return (uint)base["OrchestratorSleepIntervalBetweenRetriesInMinutes"];
			}
			set
			{
				base["OrchestratorSleepIntervalBetweenRetriesInMinutes"] = value;
			}
		}

		[ConfigurationProperty("ADDriverValidatorEnabled", DefaultValue = true)]
		public bool ADDriverValidatorEnabled
		{
			get
			{
				return (bool)base["ADDriverValidatorEnabled"];
			}
			set
			{
				base["ADDriverValidatorEnabled"] = value;
			}
		}

		[ConfigurationProperty("RemoveSourceForestLinkOnRetirement", DefaultValue = false)]
		public bool RemoveSourceForestLinkOnRetirement
		{
			get
			{
				return (bool)base["RemoveSourceForestLinkOnRetirement"];
			}
			set
			{
				base["RemoveSourceForestLinkOnRetirement"] = value;
			}
		}

		[ConfigurationProperty("RemoveSourceForestLinkOnCleanup", DefaultValue = true)]
		public bool RemoveSourceForestLinkOnCleanup
		{
			get
			{
				return (bool)base["RemoveSourceForestLinkOnCleanup"];
			}
			set
			{
				base["RemoveSourceForestLinkOnCleanup"] = value;
			}
		}

		[ConfigurationProperty("TranslateSupportedSharedConfigurations", DefaultValue = true)]
		public bool TranslateSupportedSharedConfigurations
		{
			get
			{
				return (bool)base["TranslateSupportedSharedConfigurations"];
			}
			set
			{
				base["TranslateSupportedSharedConfigurations"] = value;
			}
		}

		[ConfigurationProperty("IgnoreRelocationConstraintExpiration", DefaultValue = true)]
		public bool IgnoreRelocationConstraintExpiration
		{
			get
			{
				return (bool)base["IgnoreRelocationConstraintExpiration"];
			}
			set
			{
				base["IgnoreRelocationConstraintExpiration"] = value;
			}
		}

		[ConfigurationProperty("AutoSelectTargetPartition", DefaultValue = true)]
		public bool AutoSelectTargetPartition
		{
			get
			{
				return (bool)base["AutoSelectTargetPartition"];
			}
			set
			{
				base["AutoSelectTargetPartition"] = value;
			}
		}

		[ConfigurationProperty("DefaultRelocationCacheExpirationTimeInMinutes", DefaultValue = 60)]
		public int DefaultRelocationCacheExpirationTimeInMinutes
		{
			get
			{
				return (int)base["DefaultRelocationCacheExpirationTimeInMinutes"];
			}
			set
			{
				base["DefaultRelocationCacheExpirationTimeInMinutes"] = value;
			}
		}

		[ConfigurationProperty("ModerateRelocationCacheExpirationTimeInMinutes", DefaultValue = 10)]
		public int ModerateRelocationCacheExpirationTimeInMinutes
		{
			get
			{
				return (int)base["ModerateRelocationCacheExpirationTimeInMinutes"];
			}
			set
			{
				base["ModerateRelocationCacheExpirationTimeInMinutes"] = value;
			}
		}

		[ConfigurationProperty("AggressiveRelocationCacheExpirationTimeInMinutes", DefaultValue = 3)]
		public int AggressiveRelocationCacheExpirationTimeInMinutes
		{
			get
			{
				return (int)base["AggressiveRelocationCacheExpirationTimeInMinutes"];
			}
			set
			{
				base["AggressiveRelocationCacheExpirationTimeInMinutes"] = value;
			}
		}

		[ConfigurationProperty("DedicatedOrchestrator", DefaultValue = "")]
		public string DedicatedOrchestrator
		{
			get
			{
				return (string)base["DedicatedOrchestrator"];
			}
			set
			{
				base["DedicatedOrchestrator"] = value;
			}
		}

		[ConfigurationProperty("WaitForGlsCacheUpdateMinutes", DefaultValue = 5)]
		public int WaitForGlsCacheUpdateMinutes
		{
			get
			{
				return (int)base["WaitForGlsCacheUpdateMinutes"];
			}
			set
			{
				base["WaitForGlsCacheUpdateMinutes"] = value;
			}
		}

		[ConfigurationProperty("GlsReadRetries", DefaultValue = 6)]
		public int GlsReadRetries
		{
			get
			{
				return (int)base["GlsReadRetries"];
			}
			set
			{
				base["GlsReadRetries"] = value;
			}
		}

		[ConfigurationProperty("MaxAllowedReplicationLatencyInMinutes", DefaultValue = 5)]
		public int MaxAllowedReplicationLatencyInMinutes
		{
			get
			{
				return (int)base["MaxAllowedReplicationLatencyInMinutes"];
			}
			set
			{
				base["MaxAllowedReplicationLatencyInMinutes"] = value;
			}
		}

		[ConfigurationProperty("MaxTenantLockDownTimeInMinutes", DefaultValue = 60)]
		public int MaxTenantLockDownTimeInMinutes
		{
			get
			{
				return (int)base["MaxTenantLockDownTimeInMinutes"];
			}
			set
			{
				base["MaxTenantLockDownTimeInMinutes"] = value;
			}
		}

		[ConfigurationProperty("DoValidationAfterFullSyncEnabled", DefaultValue = true)]
		public bool ValidationAfterFullSyncEnabled
		{
			get
			{
				return (bool)base["DoValidationAfterFullSyncEnabled"];
			}
			set
			{
				base["DoValidationAfterFullSyncEnabled"] = value;
			}
		}

		[ConfigurationProperty("MaxNumberOfTransitions", DefaultValue = 30)]
		public int MaxNumberOfTransitions
		{
			get
			{
				return (int)base["MaxNumberOfTransitions"];
			}
			set
			{
				base["MaxNumberOfTransitions"] = value;
			}
		}

		[ConfigurationProperty("ValidateDomainRecordsInGls", DefaultValue = true)]
		public bool ValidateDomainRecordsInGls
		{
			get
			{
				return (bool)base["ValidateDomainRecordsInGls"];
			}
			set
			{
				base["ValidateDomainRecordsInGls"] = value;
			}
		}

		[ConfigurationProperty("ValidateDomainRecordsInMServ", DefaultValue = true)]
		public bool ValidateDomainRecordsInMServ
		{
			get
			{
				return (bool)base["ValidateDomainRecordsInMServ"];
			}
			set
			{
				base["ValidateDomainRecordsInMServ"] = value;
			}
		}

		[ConfigurationProperty("ValidateMXRecordsInDNS", DefaultValue = true)]
		public bool ValidateMXRecordsInDNS
		{
			get
			{
				return (bool)base["ValidateMXRecordsInDNS"];
			}
			set
			{
				base["ValidateMXRecordsInDNS"] = value;
			}
		}

		[ConfigurationProperty("MaxNumberOfRelocationsInRelocationPipeline", DefaultValue = 1000)]
		public int MaxNumberOfRelocationsInRelocationPipeline
		{
			get
			{
				return (int)base["MaxNumberOfRelocationsInRelocationPipeline"];
			}
			set
			{
				base["MaxNumberOfRelocationsInRelocationPipeline"] = value;
			}
		}

		[ConfigurationProperty("DoValidationAfterDeltaSyncEnabled", DefaultValue = true)]
		public bool DoValidationAfterDeltaSyncEnabled
		{
			get
			{
				return (bool)base["DoValidationAfterDeltaSyncEnabled"];
			}
			set
			{
				base["DoValidationAfterDeltaSyncEnabled"] = value;
			}
		}

		[ConfigurationProperty("CleanupSchedule", DefaultValue = "From9AMTo5PMAtWeekDays")]
		public string CleanupSchedule
		{
			get
			{
				return (string)base["CleanupSchedule"];
			}
			set
			{
				base["CleanupSchedule"] = value;
			}
		}

		[ConfigurationProperty("ADHealthSamplerPollIntervalInMinutes", DefaultValue = 15)]
		public int ADHealthSamplerPollIntervalInMinutes
		{
			get
			{
				return (int)base["ADHealthSamplerPollIntervalInMinutes"];
			}
			set
			{
				base["ADHealthSamplerPollIntervalInMinutes"] = value;
			}
		}

		[ConfigurationProperty("ADReplicationHealthSamplerEnabled", DefaultValue = true)]
		public bool ADReplicationHealthSamplerEnabled
		{
			get
			{
				return (bool)base["ADReplicationHealthSamplerEnabled"];
			}
			set
			{
				base["ADReplicationHealthSamplerEnabled"] = value;
			}
		}

		[ConfigurationProperty("LoadStateNoDelayMs", DefaultValue = 0)]
		public int LoadStateNoDelayMs
		{
			get
			{
				return (int)base["LoadStateNoDelayMs"];
			}
			set
			{
				base["LoadStateNoDelayMs"] = value;
			}
		}

		[ConfigurationProperty("LoadStateDefaultDelayMs", DefaultValue = 100)]
		public int LoadStateDefaultDelayMs
		{
			get
			{
				return (int)base["LoadStateDefaultDelayMs"];
			}
			set
			{
				base["LoadStateDefaultDelayMs"] = value;
			}
		}

		[ConfigurationProperty("LoadStateOverloadedDelayMs", DefaultValue = 500)]
		public int LoadStateOverloadedDelayMs
		{
			get
			{
				return (int)base["LoadStateOverloadedDelayMs"];
			}
			set
			{
				base["LoadStateOverloadedDelayMs"] = value;
			}
		}

		[ConfigurationProperty("LoadStateCriticalDelayMs", DefaultValue = 1000)]
		public int LoadStateCriticalDelayMs
		{
			get
			{
				return (int)base["LoadStateCriticalDelayMs"];
			}
			set
			{
				base["LoadStateCriticalDelayMs"] = value;
			}
		}

		[ConfigurationProperty("CleanupDryRunEnabled", DefaultValue = false)]
		public bool CleanupDryRunEnabled
		{
			get
			{
				return (bool)base["CleanupDryRunEnabled"];
			}
			set
			{
				base["CleanupDryRunEnabled"] = value;
			}
		}

		[ConfigurationProperty("SuspendGlsCache", DefaultValue = true)]
		public bool SuspendGlsCache
		{
			get
			{
				return (bool)base["SuspendGlsCache"];
			}
			set
			{
				base["SuspendGlsCache"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>(0L, "Unrecognized configuration attribute {0}={1}", name, value);
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}

		public static class Setting
		{
			public const string IsAllRelocationActivitiesHalted = "IsAllRelocationActivitiesHalted";

			public const string IsProcessingBrokerEnabled = "IsProcessingBrokerEnabled";

			public const string IsRollbackBrokerEnabled = "IsRollbackBrokerEnabled";

			public const string IsCleanupBrokerEnabled = "IsCleanupBrokerEnabled";

			public const string IsOrchestratorEnabled = "IsOrchestratorEnabled";

			public const string DataSyncObjectsPerPageLimit = "DataSyncObjectsPerPageLimit";

			public const string DataSyncLinksPerPageLimit = "DataSyncLinksPerPageLimit";

			public const string DataSyncInitialLinkReadSize = "DataSyncInitialLinkReadSize";

			public const string DataSyncFailoverTimeoutInMinutes = "DataSyncFailoverTimeoutInMinutes";

			public const string DataSyncLinksOverldapSize = "DataSyncLinksOverldapSize";

			public const string DeltaSyncUsnRangeLimit = "DeltaSyncUsnRangeLimit";

			public const string MaxAllowedReplicationLatencyInMinutes = "MaxAllowedReplicationLatencyInMinutes";

			public const string MaxConcurrentProcessingThreadsPerServer = "MaxConcurrentProcessingThreadsPerServer";

			public const string MaxConcurrentRollbackThreadsPerServer = "MaxConcurrentRollbackThreadsPerServer";

			public const string MaxConcurrentCleanupThreadsPerServer = "MaxConcurrentCleanupThreadsPerServer";

			public const string ProcessingBrokerPollIntervalInMinutes = "ProcessingBrokerPollIntervalInMinutes";

			public const string RollbackBrokerPollIntervalInMinutes = "RollbackBrokerPollIntervalInMinutes";

			public const string IsUserExperienceTestEnabled = "IsUserExperienceTestEnabled";

			public const string DisabledUXProbes = "DisabledUXProbes";

			public const string UXProbeRecurrenceIntervalSeconds = "UXProbeRecurrenceIntervalSeconds";

			public const string UXMonitorConsecutiveProbeFailureCount = "UXMonitorConsecutiveProbeFailureCount";

			public const string UXMonitorAccountExpiredDays = "UXMonitorAccountExpiredDays";

			public const string RemoveUXMonitorAccountWaitReplicationMinutes = "RemoveUXMonitorAccountWaitReplicationMinutes";

			public const string WaitUXFailureResultSeconds = "WaitUXFailureResultSeconds";

			public const string UXTransportProbeSmtpServers = "UXTransportProbeSmtpServers";

			public const string UXTransportProbeSmtpPort = "UXTransportProbeSmtpPort";

			public const string UXTransportProbeSenderAddress = "UXTransportProbeSenderAddress";

			public const string UXTransportProbeSendMessageTimeout = "UXTransportProbeSendMessageTimeout";

			public const string UXTransportProbeWaitMessageTimeout = "UXTransportProbeWaitMessageTimeout";

			public const string CheckStaleRelocations = "CheckStaleRelocations";

			public const string SafeScheduleWindow = "SafeScheduleWindow";

			public const string MaxRelocationInNonCriticalStage = "MaxRelocationInNonCriticalStage";

			public const string MaxRelocationInCriticalStage = "MaxRelocationInCriticalStage";

			public const string MaxRelocationInCleanupStage = "MaxRelocationInCleanupStage";

			public const string OrchestratorSleepIntervalBetweenRetriesInMinutes = "OrchestratorSleepIntervalBetweenRetriesInMinutes";

			public const string ADDriverValidatorEnabled = "ADDriverValidatorEnabled";

			public const string RemoveSourceForestLinkOnRetirement = "RemoveSourceForestLinkOnRetirement";

			public const string RemoveSourceForestLinkOnCleanup = "RemoveSourceForestLinkOnCleanup";

			public const string TranslateSupportedSharedConfigurations = "TranslateSupportedSharedConfigurations";

			public const string IgnoreRelocationConstraintExpiration = "IgnoreRelocationConstraintExpiration";

			public const string AutoSelectTargetPartition = "AutoSelectTargetPartition";

			public const string DefaultRelocationCacheExpirationTimeInMinutes = "DefaultRelocationCacheExpirationTimeInMinutes";

			public const string ModerateRelocationCacheExpirationTimeInMinutes = "ModerateRelocationCacheExpirationTimeInMinutes";

			public const string AggressiveRelocationCacheExpirationTimeInMinutes = "AggressiveRelocationCacheExpirationTimeInMinutes";

			public const string DedicatedOrchestrator = "DedicatedOrchestrator";

			public const string WaitForGlsCacheUpdateMinutes = "WaitForGlsCacheUpdateMinutes";

			public const string GlsReadRetries = "GlsReadRetries";

			public const string MaxTenantLockDownTimeInMinutes = "MaxTenantLockDownTimeInMinutes";

			public const string DoValidationAfterFullSyncEnabled = "DoValidationAfterFullSyncEnabled";

			public const string MaxNumberOfTransitions = "MaxNumberOfTransitions";

			public const string ValidateDomainRecordsInGls = "ValidateDomainRecordsInGls";

			public const string ValidateDomainRecordsInMServ = "ValidateDomainRecordsInMServ";

			public const string ValidateMXRecordsInDNS = "ValidateMXRecordsInDNS";

			public const string MaxNumberOfRelocationsInRelocationPipeline = "MaxNumberOfRelocationsInRelocationPipeline";

			public const string DoValidationAfterDeltaSyncEnabled = "DoValidationAfterDeltaSyncEnabled";

			public const string CleanupSchedule = "CleanupSchedule";

			public const string ADHealthSamplerPollIntervalInMinutes = "ADHealthSamplerPollIntervalInMinutes";

			public const string ADReplicationHealthSamplerEnabled = "ADReplicationHealthSamplerEnabled";

			public const string LoadStateNoDelayMs = "LoadStateNoDelayMs";

			public const string LoadStateDefaultDelayMs = "LoadStateDefaultDelayMs";

			public const string LoadStateOverloadedDelayMs = "LoadStateOverloadedDelayMs";

			public const string LoadStateCriticalDelayMs = "LoadStateCriticalDelayMs";

			public const string CleanupDryRunEnabled = "CleanupDryRunEnabled";

			public const string SuspendGlsCache = "SuspendGlsCache";
		}
	}
}
