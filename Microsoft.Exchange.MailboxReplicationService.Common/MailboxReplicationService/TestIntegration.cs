using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class TestIntegration : TestIntegrationBase
	{
		public TestIntegration(bool autoRefresh = false) : base("SOFTWARE\\Microsoft\\Exchange_Test\\v15\\Migration", autoRefresh)
		{
		}

		public static TestIntegration Instance
		{
			get
			{
				return TestIntegration.instance;
			}
		}

		public bool DisableRetriesOnTransientFailures
		{
			get
			{
				return base.GetFlagValue("DisableRetriesOnTransientFailures");
			}
		}

		public bool UseRemoteForSource
		{
			get
			{
				return base.GetFlagValue("UseRemoteForSource");
			}
		}

		public bool UseRemoteForDestination
		{
			get
			{
				return base.GetFlagValue("UseRemoteForDestination");
			}
		}

		public bool UseTcpForRemoteMoves
		{
			get
			{
				return base.GetFlagValue("UseTcpForRemoteMoves");
			}
		}

		public bool UseHttpsForLocalMoves
		{
			get
			{
				return base.GetFlagValue("UseHttpsForLocalMoves");
			}
		}

		public bool SkipWordBreaking
		{
			get
			{
				return base.GetFlagValue("SkipWordBreaking");
			}
		}

		public bool ForcePreFinalSyncDataProcessing
		{
			get
			{
				return base.GetFlagValue("ForcePreFinalSyncDataProcessing");
			}
		}

		public bool DisableDataGuaranteeCheckPeriod
		{
			get
			{
				return base.GetFlagValue("DisableDataGuaranteeCheckPeriod");
			}
		}

		public bool AllowRemoteArchivesInEnt
		{
			get
			{
				return base.GetFlagValue("AllowRemoteArchivesInEnt");
			}
		}

		public bool MicroDelayEnabled
		{
			get
			{
				return base.GetFlagValue("MicroDelayEnabled", true);
			}
		}

		public bool BypassResourceReservation
		{
			get
			{
				return base.GetFlagValue("BypassResourceReservation");
			}
		}

		public int MaxTombstoneRetries
		{
			get
			{
				return base.GetIntValue("MaxTombstoneRetries", 180, 0, 1000);
			}
		}

		public int MaxReportEntryCount
		{
			get
			{
				return base.GetIntValue("MaxReportEntryCount", 10000, -1, int.MaxValue);
			}
		}

		public int MaxOpenConnectionsPerPublicFolderMigration
		{
			get
			{
				return base.GetIntValue("MaxOpenConnectionsPerPublicFolderMigration", 500, 1, 1000);
			}
		}

		public int LargeDataLossThreshold
		{
			get
			{
				return base.GetIntValue("LargeDataLossThreshold", 50, 0, int.MaxValue);
			}
		}

		public bool UseLegacyCheckForHaCiHealthQuery
		{
			get
			{
				return base.GetFlagValue("UseLegacyCheckForHaCiHealthQuery");
			}
		}

		public bool DoNotAutomaticallyMarkRehome
		{
			get
			{
				return base.GetFlagValue("DoNotAutomaticallyMarkRehome");
			}
		}

		public int InjectMissingItems
		{
			get
			{
				return base.GetIntValue("InjectMissingItems", 0, 0, int.MaxValue);
			}
		}

		public bool ClassifyBadItemFaults
		{
			get
			{
				return base.GetFlagValue("ClassifyBadItemFaults");
			}
		}

		public string RoutingCookie
		{
			get
			{
				return base.GetStrValue("RoutingCookieName");
			}
		}

		public bool AllowRemoteLegacyMovesWithE15
		{
			get
			{
				return base.GetFlagValue("AllowRemoteLegacyMovesWithE15");
			}
		}

		public bool DisableRemoteHostNameBlacklisting
		{
			get
			{
				return base.GetFlagValue("DisableRemoteHostNameBlacklisting");
			}
		}

		public TimeSpan RemoteMailboxConnectionTimeout
		{
			get
			{
				return TimeSpan.FromSeconds((double)base.GetIntValue("RemoteMailboxConnectionTimeoutSecs", 22, 0, int.MaxValue));
			}
		}

		public TimeSpan RemoteMailboxCallTimeout
		{
			get
			{
				return TimeSpan.FromSeconds((double)base.GetIntValue("RemoteMailboxCallTimeoutSecs", 7200, 0, int.MaxValue));
			}
		}

		public TimeSpan LocalMailboxConnectionTimeout
		{
			get
			{
				return TimeSpan.FromSeconds((double)base.GetIntValue("LocalMailboxConnectionTimeoutSecs", 15, 0, int.MaxValue));
			}
		}

		public TimeSpan LocalMailboxCallTimeout
		{
			get
			{
				return TimeSpan.FromSeconds((double)base.GetIntValue("LocalMailboxCallTimeoutSecs", 7200, 0, int.MaxValue));
			}
		}

		public TimeSpan ProxyClientPingInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)base.GetIntValue("ProxyClientPingInterval", 180, 0, int.MaxValue));
			}
		}

		public bool SkipMrsProxyValidation
		{
			get
			{
				return base.GetFlagValue("SkipMrsProxyValidation");
			}
		}

		public int UpgradeSourceUserWhileOnboarding
		{
			get
			{
				return base.GetIntValue("UpgradeSourceUserWhileOnboarding", 0, -1, 1);
			}
		}

		public int FolderBatchSize
		{
			get
			{
				return base.GetIntValue("FolderBatchSize", 100, 1, int.MaxValue);
			}
		}

		public bool DoE15HaCiHealthCheckForJobPickup
		{
			get
			{
				return base.GetFlagValue("DoE15HaCiHealthCheckForJobPickup");
			}
		}

		public bool InjectCrash
		{
			get
			{
				return base.GetFlagValue("InjectCrash");
			}
		}

		public bool InjectCorruptSyncState
		{
			get
			{
				return base.GetFlagValue("InjectCorruptSyncState");
			}
		}

		public bool InjectTransientExceptionAfterFolderDataCopy
		{
			get
			{
				return base.GetFlagValue("InjectTransientExceptionAfterFolderDataCopy");
			}
		}

		public bool LogContentDetails
		{
			get
			{
				return base.GetFlagValue("LogContentDetails");
			}
		}

		public bool DoNotUnlockTargetMailbox
		{
			get
			{
				return base.GetFlagValue("DoNotUnlockTargetMailbox");
			}
		}

		public bool UpdateMoveRequestFailsAfterStampingHomeMdb
		{
			get
			{
				return base.GetFlagValue("UpdateMoveRequestFailsAfterStampingHomeMdb");
			}
		}

		public bool AssumeWLMUnhealthyForReservations
		{
			get
			{
				return base.GetFlagValue("AssumeWLMUnhealthyForReservations");
			}
		}

		public bool DisableFolderCreationBlockFeature
		{
			get
			{
				return base.GetFlagValue("DisableFolderCreationBlockFeature");
			}
		}

		public bool AbortConnectionDuringFX
		{
			get
			{
				return base.GetFlagValue("AbortConnectionDuringFX");
			}
		}

		public Guid RemoteExchangeGuidOverride
		{
			get
			{
				return base.GetGuidValue("RemoteExchangeGuidOverride");
			}
		}

		public Guid RemoteArchiveGuidOverride
		{
			get
			{
				return base.GetGuidValue("RemoteArchiveGuidOverride");
			}
		}

		public Unlimited<EnhancedTimeSpan> GetCompletedRequestAgeLimit(TimeSpan defaultValue)
		{
			int intValue = base.GetIntValue("DefaultCompletedRequestAgeLimitOverride", (int)defaultValue.TotalHours, -1, int.MaxValue);
			if (intValue < 0)
			{
				return Unlimited<EnhancedTimeSpan>.UnlimitedValue;
			}
			return new Unlimited<EnhancedTimeSpan>(TimeSpan.FromHours((double)intValue));
		}

		public int IntroduceFailureAfterCopyingHighWatermarkNTimes
		{
			get
			{
				return base.GetIntValue("IntroduceFailureAfterCopyingHighWatermarkNTimes", 0, 0, 100);
			}
		}

		public bool CheckInitialProvisioningForMoves
		{
			get
			{
				return base.GetFlagValue("CheckInitialProvisioningForMoves", ConfigBase<MRSConfigSchema>.GetConfig<bool>("CheckInitialProvisioningForMoves"));
			}
		}

		public bool InjectUmmEndProcessingFailure
		{
			get
			{
				return base.GetFlagValue("InjectUmmEndProcessingFailure");
			}
		}

		public string EasAutodiscoverUrlOverride
		{
			get
			{
				return base.GetStrValue("EasAutodiscoverUrlOverride");
			}
		}

		public bool ProtocolTest
		{
			get
			{
				return base.GetFlagValue("ProtocolTest");
			}
		}

		public const string RegKeyName = "SOFTWARE\\Microsoft\\Exchange_Test\\v15\\Migration";

		public const string PostponeEnumerateFolderMessagesName = "PostponeEnumerateFolderMessages";

		public const string PostponeSyncName = "PostponeSync";

		public const string PostponeFinalSyncName = "PostponeFinalSync";

		public const string PostponeResumeAccessToMailboxName = "PostponeResumeAccessToMailbox";

		public const string PostponeWriteMessagesName = "PostponeWriteMessages";

		public const string PostponeCleanupName = "PostponeCleanup";

		public const string BreakpointBeforeUMM = "BreakpointBeforeUMM";

		public const string BreakpointAfterUMM = "BreakpointAfterUMM";

		public const string BreakpointRelinquish = "BreakpointRelinquish";

		public const string BreakpointJobStalledDueToHACIHealth = "BreakpointStalledDueToHACIHealth";

		public const string BreakpointBeforeConnect = "BreakpointBeforeConnect";

		public const string DontPickupJobsName = "DontPickupJobs";

		public const string DisableRetriesOnTransientFailuresName = "DisableRetriesOnTransientFailures";

		public const string UseRemoteForSourceName = "UseRemoteForSource";

		public const string UseRemoteForDestinationName = "UseRemoteForDestination";

		public const string UseTcpForRemoteMovesName = "UseTcpForRemoteMoves";

		public const string UseHttpsForLocalMovesName = "UseHttpsForLocalMoves";

		public const string SkipWordBreakingName = "SkipWordBreaking";

		public const string ForcePreFinalSyncDataProcessingName = "ForcePreFinalSyncDataProcessing";

		public const string ReplayRpcConfigOverrideFrequency = "ReplayRpcConfigOverrideFrequency";

		public const string DisableDataGuaranteeCheckPeriodName = "DisableDataGuaranteeCheckPeriod";

		public const string DataGuaranteeTimeoutOverrideSecsName = "DataGuaranteeTimeoutOverrideSecs";

		public const string AllowRemoteArchivesInEntName = "AllowRemoteArchivesInEnt";

		public const string MaxReportEntryCountName = "MaxReportEntryCount";

		public const string DefaultCompletedRequestAgeLimitOverrideName = "DefaultCompletedRequestAgeLimitOverride";

		public const string MaxTombstoneRetriesName = "MaxTombstoneRetries";

		public const string MaxOpenConnectionsPerPublicFolderMigrationName = "MaxOpenConnectionsPerPublicFolderMigration";

		public const string LargeDataLossThresholdName = "LargeDataLossThreshold";

		public const string UseLegacyCheckForHaCiHealthQueryName = "UseLegacyCheckForHaCiHealthQuery";

		public const string DoNotAutomaticallyMarkRehomeName = "DoNotAutomaticallyMarkRehome";

		public const string RoutingCookieName = "RoutingCookieName";

		public const string AllowRemoteLegacyMovesWithE15Name = "AllowRemoteLegacyMovesWithE15";

		public const string DisableRemoteHostNameBlacklistingName = "DisableRemoteHostNameBlacklisting";

		public const string RemoteMailboxConnectionTimeoutName = "RemoteMailboxConnectionTimeoutSecs";

		public const string RemoteMailboxCallTimeoutName = "RemoteMailboxCallTimeoutSecs";

		public const string LocalMailboxConnectionTimeoutName = "LocalMailboxConnectionTimeoutSecs";

		public const string LocalMailboxCallTimeoutName = "LocalMailboxCallTimeoutSecs";

		public const string ProxyClientPingIntervalName = "ProxyClientPingInterval";

		public const string InjectMissingItemsName = "InjectMissingItems";

		public const string ClassifyBadItemFaultsName = "ClassifyBadItemFaults";

		public const string MicroDelayEnabledName = "MicroDelayEnabled";

		public const string BypassResourceReservationName = "BypassResourceReservation";

		public const string SkipMrsProxyValidationName = "SkipMrsProxyValidation";

		public const string FolderBatchSizeName = "FolderBatchSize";

		public const string UpgradeSourceUserWhileOnboardingName = "UpgradeSourceUserWhileOnboarding";

		public const string DoE15HaCiHealthCheckForJobPickupName = "DoE15HaCiHealthCheckForJobPickup";

		public const string InjectCrashName = "InjectCrash";

		public const string InjectCorruptSyncStateName = "InjectCorruptSyncState";

		public const string InjectTransientExceptionAfterFolderDataCopyName = "InjectTransientExceptionAfterFolderDataCopy";

		public const string FolderNameToInjectTransientException = "FolderToInjectTransientException";

		public const string InjectNFaultsPostMoveUpdateSourceMailboxName = "InjectNFaultsPostMoveUpdateSourceMailbox";

		public const string LogContentDetailsName = "LogContentDetails";

		public const string DoNotUnlockTargetMailboxName = "DoNotUnlockTargetMailbox";

		public const string UpdateMoveRequestFailsAfterStampingHomeMdbName = "UpdateMoveRequestFailsAfterStampingHomeMdb";

		public const string AssumeWLMUnhealthyForReservationsName = "AssumeWLMUnhealthyForReservations";

		public const string DisableFolderCreationBlockFeatureName = "DisableFolderCreationBlockFeature";

		public const string AbortConnectionDuringFXName = "AbortConnectionDuringFX";

		public const string IntroduceFailureAfterCopyingHighWatermarkNTimesName = "IntroduceFailureAfterCopyingHighWatermarkNTimes";

		public const string PostponeCompleteName = "PostponeComplete";

		public const string CheckInitialProvisioningForMovesName = "CheckInitialProvisioningForMoves";

		public const string InjectUmmEndProcessingFailureName = "InjectUmmEndProcessingFailure";

		public const string SimulatePushMoveName = "SimulatePushMove";

		public const string RemoteExchangeGuidOverrideName = "RemoteExchangeGuidOverride";

		public const string RemoteArchiveGuidOverrideName = "RemoteArchiveGuidOverride";

		public const string InjectNFaultsTargetConnectivityVerificationName = "InjectNFaultsTargetConnectivityVerification";

		public const string EasAutodiscoverUrlOverrideName = "EasAutodiscoverUrlOverride";

		public const string ProtocolTestName = "ProtocolTest";

		public const int DefaultMaxOpenConnectionsPerPublicFolderMigration = 500;

		public const int DefaultLargeDataLossThreshold = 50;

		public const int DefaultRemoteMailboxConnectionTimeout = 22;

		public const int DefaultRemoteMailboxCallTimeout = 7200;

		public const int DefaultLocalMailboxConnectionTimeout = 15;

		public const int DefaultLocalMailboxCallTimeout = 7200;

		public const int DefaultProxyClientPingInterval = 180;

		public const int DefaultFolderBatchSize = 100;

		public static readonly LocalizedString FaultInjectionExceptionMessage = new LocalizedString("Injecting Fault for Testing...");

		private static readonly TestIntegration instance = new TestIntegration(true);
	}
}
