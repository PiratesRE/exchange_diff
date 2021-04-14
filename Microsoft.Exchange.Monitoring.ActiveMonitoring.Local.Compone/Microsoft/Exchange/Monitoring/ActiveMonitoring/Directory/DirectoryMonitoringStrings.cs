using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory
{
	public static class DirectoryMonitoringStrings
	{
		public const string NTDSServiceName = "NTDS";

		public const string MSExchangeADTopologyServiceName = "MSExchangeADTopology";

		public const string MSExchangeProtectedServiceHostServiceName = "MSExchangeProtectedServiceHost";

		public const string KDCServiceName = "KDC";

		public const string MSExchangeRPCServiceName = "MSExchangeRPC";

		public static DirectoryMonitoringScenario NTDSNotRunning = new DirectoryMonitoringScenario("NTDSServiceNotRunning", Strings.ServiceNotRunningEscalationMessage("NTDS"));

		public static DirectoryMonitoringScenario MSExchangeADTopologyNotRunning = new DirectoryMonitoringScenario("MSExchangeADTopologyNotRunning", Strings.ServiceNotRunningEscalationMessage("MSExchangeADTopology"));

		public static DirectoryMonitoringScenario MSExchangeProtectedServiceHostNotRunning = new DirectoryMonitoringScenario("MSExchangeProtectedServiceHostNotRunning", Strings.ServiceNotRunningEscalationMessage("MSExchangeProtectedServiceHost"));

		public static DirectoryMonitoringScenario MSExchangeProtectedServiceHostCrashing = new DirectoryMonitoringScenario("MSExchangeProtectedServiceHostCrashing", Strings.MSExchangeProtectedServiceHostCrashingMessage);

		public static DirectoryMonitoringScenario KDCServiceStatusTest = new DirectoryMonitoringScenario("KDCServiceStatusTest", Strings.KDCServiceStatusTestMessage);

		public static DirectoryMonitoringScenario KDCNotRunning = new DirectoryMonitoringScenario("KDCServiceNotRunning", Strings.ServiceNotRunningEscalationMessage("KDC"));

		public static DirectoryMonitoringScenario ActiveDirectoryConnectivity = new DirectoryMonitoringScenario("ActiveDirectoryConnectivity", Strings.ActiveDirectoryConnectivityEscalationMessage, true);

		public static DirectoryMonitoringScenario ActiveDirectoryConnectivityLocal = new DirectoryMonitoringScenario("ActiveDirectoryConnectivityLocal", Strings.ActiveDirectoryConnectivityLocalEscalationMessage);

		public static DirectoryMonitoringScenario ActiveDirectoryConnectivityConfigDC = new DirectoryMonitoringScenario("ActiveDirectoryConnectivityConfigDC", Strings.ActiveDirectoryConnectivityConfigDCEscalationMessage, true);

		public static DirectoryMonitoringScenario NtlmConnectivity = new DirectoryMonitoringScenario("NtlmConnectivity", Strings.NtlmConnectivityEscalationMessage, true);

		public static DirectoryMonitoringScenario TopologyServiceConnectivity = new DirectoryMonitoringScenario("TopologyServiceConnectivity", Strings.TopologyServiceConnectivityEscalationMessage);

		public static DirectoryMonitoringScenario LiveIdAuthenticationConnectivity = new DirectoryMonitoringScenario("LiveIdAuthenticationConnectivity", Strings.LiveIdAuthenticationEscalationMesage, true);

		public static DirectoryMonitoringScenario GlsConnectivity = new DirectoryMonitoringScenario("GlsConnectivity", Strings.GLSEscalationMessage, true);

		public static DirectoryMonitoringScenario OfflineGLS = new DirectoryMonitoringScenario("OfflineGls", Strings.OfflineGLSEscalationMessage, true);

		public static DirectoryMonitoringScenario MSExchangeInformationStoreCannotContactAD = new DirectoryMonitoringScenario("MSExchangeInformationStoreCannotContactAD", Strings.MSExchangeInformationStoreCannotContactADEscalationMessage, true);

		public static DirectoryMonitoringScenario TopoDiscoveryFailedAllServers = new DirectoryMonitoringScenario("TopoDiscoveryFailedAllServers", Strings.TopoDiscoveryFailedAllServersEscalationMessage, true);

		public static DirectoryMonitoringScenario RidConsumption = new DirectoryMonitoringScenario("RidConsumption", Strings.RidMonitorEscalationMessage);

		public static DirectoryMonitoringScenario RidSetValidation = new DirectoryMonitoringScenario("RidSetValidation", Strings.RidSetMonitorEscalationMessage);

		public static DirectoryMonitoringScenario RidPoolRequestFailed = new DirectoryMonitoringScenario("RidPoolRequestFailed", Strings.RequestForNewRidPoolFailedEscalationMessage);

		public static DirectoryMonitoringScenario KerbAuthFailure = new DirectoryMonitoringScenario("KerbAuthFailure", Strings.KerbAuthFailureEscalationMessage);

		public static DirectoryMonitoringScenario PACValidationFailure = new DirectoryMonitoringScenario("PACValidationFailure", Strings.KerbAuthFailureEscalationMessagPAC);

		public static DirectoryMonitoringScenario SyntheticReplicationTransactionAll = new DirectoryMonitoringScenario("SyntheticReplicationTransactionAll", Strings.SyntheticReplicationTransactionEscalationMessage);

		public static DirectoryMonitoringScenario PassiveReplicationMonitorAll = new DirectoryMonitoringScenario("PassiveReplicationMonitorAll", Strings.PassiveReplicationMonitorEscalationMessage);

		public static DirectoryMonitoringScenario PassiveReplicationPerformanceCounterProbeAll = new DirectoryMonitoringScenario("PassiveReplicationPerformanceCounterProbeAll", Strings.PassiveReplicationPerformanceCounterProbeEscalationMessage);

		public static DirectoryMonitoringScenario PassiveADReplicationMonitorAll = new DirectoryMonitoringScenario("PassiveADReplicationMonitorAll", Strings.PassiveADReplicationMonitorEscalationMessage);

		public static DirectoryMonitoringScenario TrustMonitorProbeAll = new DirectoryMonitoringScenario("TrustMonitorProbeAll", Strings.TrustMonitorProbeEscalationMessage);

		public static DirectoryMonitoringScenario RemoteDomainControllerState = new DirectoryMonitoringScenario("RemoteDomainControllerState", Strings.RemoteDomainControllerStateEscalationMessage);

		public static DirectoryMonitoringScenario SyntheticReplicationMonitorRID = new DirectoryMonitoringScenario("SyntheticReplicationMonitorRID", Strings.SyntheticReplicationMonitorEscalationMessage);

		public static DirectoryMonitoringScenario ReplicationDisabled = new DirectoryMonitoringScenario("ReplicationDisabled", Strings.ReplicationDisabledEscalationMessage);

		public static DirectoryMonitoringScenario ReplicationKccInsufficientInfo = new DirectoryMonitoringScenario("ReplicationKccInsufficientInfo", Strings.InsufficientInformationKCCEscalationMessage);

		public static DirectoryMonitoringScenario ReplicationBridgehead = new DirectoryMonitoringScenario("ReplicationBridgehead", Strings.BridgeHeadReplicationEscalationMessage);

		public static DirectoryMonitoringScenario ReplicationIncompatibleVector = new DirectoryMonitoringScenario("ReplicationIncompatibleVector", Strings.IncompatibleVectorEscalationMessage);

		public static DirectoryMonitoringScenario ReplicationSlowADWrites = new DirectoryMonitoringScenario("ReplicationSlowADWrites", Strings.SlowADWritesEscalationMessage);

		public static DirectoryMonitoringScenario ReplicationUnableToCompleteTopology = new DirectoryMonitoringScenario("ReplicationUnableToCompleteTopology", Strings.UnableToCompleteTopologyEscalationMessage);

		public static DirectoryMonitoringScenario ReplicationDraPendingReps = new DirectoryMonitoringScenario("ReplicationDraPendingReps", Strings.DRAPendingReplication5MinutesEscalationMessage);

		public static DirectoryMonitoringScenario OutStandingATQRequests = new DirectoryMonitoringScenario("OutStandingATQRequests", Strings.OutStandingATQRequests15MinutesEscalationMessage);

		public static DirectoryMonitoringScenario HighProcessor = new DirectoryMonitoringScenario("HighProcessor", Strings.HighProcessor15MinutesEscalationMessage);

		public static DirectoryMonitoringScenario ReplicationOutdatedObjectsFailed = new DirectoryMonitoringScenario("ReplicationOutdatedObjectsFailed", Strings.ReplicationOutdatedObjectsFailedEscalationMessage);

		public static DirectoryMonitoringScenario ReplicationFailures = new DirectoryMonitoringScenario("ReplicationFailures", Strings.ReplicationFailuresEscalationMessage);

		public static DirectoryMonitoringScenario ADCannotBoot = new DirectoryMonitoringScenario("ADCannotBoot", Strings.CannotBootEscalationMessage);

		public static DirectoryMonitoringScenario ADFailedToUpgradeIndex = new DirectoryMonitoringScenario("ADFailedToUpgradeIndex", Strings.FailedToUpgradeIndexEscalationMessage);

		public static DirectoryMonitoringScenario ADReinstallServer = new DirectoryMonitoringScenario("ADReinstallServer", Strings.ReinstallServerEscalationMessage);

		public static DirectoryMonitoringScenario ADCannotFunctionNormally = new DirectoryMonitoringScenario("ADCannotFunctionNormally", Strings.CannotFunctionNormallyEscalationMessage);

		public static DirectoryMonitoringScenario ADCannotRecover = new DirectoryMonitoringScenario("ADCannotRecover", Strings.CannotRecoverEscalationMessage);

		public static DirectoryMonitoringScenario ADSchemaPartitionFailed = new DirectoryMonitoringScenario("ADSchemaPartitionFailed", Strings.SchemaPartitionFailedEscalationMessage);

		public static DirectoryMonitoringScenario ADCannotRebuildIndex = new DirectoryMonitoringScenario("ADCannotRebuildIndex", Strings.CannotRebuildIndexEscalationMessage);

		public static DirectoryMonitoringScenario ADContentsUnpredictable = new DirectoryMonitoringScenario("ADContentsUnpredictable", Strings.ContentsUnpredictableEscalationMessage);

		public static DirectoryMonitoringScenario ADNoNTDSObject = new DirectoryMonitoringScenario("ADNoNTDSObject", Strings.NoNTDSObjectEscalationMessage);

		public static DirectoryMonitoringScenario ADVersionStore623 = new DirectoryMonitoringScenario("ADVersionStore623", Strings.VersionStore623EscalationMessage);

		public static DirectoryMonitoringScenario ADVersionStore2008 = new DirectoryMonitoringScenario("ADVersionStore2008", Strings.VersionStore2008EscalationMessage);

		public static DirectoryMonitoringScenario ADVersionStore1479 = new DirectoryMonitoringScenario("ADVersionStore1479", Strings.VersionStore1479EscalationMessage);

		public static DirectoryMonitoringScenario DsNotificationQueue = new DirectoryMonitoringScenario("DsNotificationQueue", Strings.DSNotifyQueueHigh15MinutesEscalationMessage);

		public static DirectoryMonitoringScenario ADDatabaseCorruption1017 = new DirectoryMonitoringScenario("ADDatabaseCorruption1017", Strings.ADDatabaseCorruption1017EscalationMessage);

		public static DirectoryMonitoringScenario LogicalDiskFreeMegabytes = new DirectoryMonitoringScenario("LogicalDiskFreeMegabytes", Strings.LogicalDiskFreeMegabytesEscalationMessage);

		public static DirectoryMonitoringScenario ADDatabaseCorrupt = new DirectoryMonitoringScenario("ADDatabaseCorrupt", Strings.DatabaseCorruptEscalationMessage);

		public static DirectoryMonitoringScenario NTDSCorruption = new DirectoryMonitoringScenario("NTDSCorruption", Strings.NTDSCorruptionEscalationMessage);

		public static DirectoryMonitoringScenario ADDatabaseCorruption = new DirectoryMonitoringScenario("ADDatabaseCorruption", Strings.ADDatabaseCorruptionEscalationMessage);

		public static DirectoryMonitoringScenario ADCheckSum = new DirectoryMonitoringScenario("ADChecksum", Strings.CheckSumEscalationMessage);

		public static DirectoryMonitoringScenario ADDataIssue = new DirectoryMonitoringScenario("ADDataIssue", Strings.DataIssueEscalationMessage);

		public static DirectoryMonitoringScenario DatabaseCorruption = new DirectoryMonitoringScenario("DatabaseCorruption", Strings.DatabaseCorruptionEscalationMessage);

		public static DirectoryMonitoringScenario RaidDegraded = new DirectoryMonitoringScenario("RaidDegraded", Strings.RaidDegradedEscalationMessage);

		public static DirectoryMonitoringScenario DeviceDegraded = new DirectoryMonitoringScenario("DeviceDegraded", Strings.DeviceDegradedEscalationMessage);

		public static DirectoryMonitoringScenario PermanentExceptionInRelocationService = new DirectoryMonitoringScenario("PermanentExceptionInRelocationService", Strings.RelocationServicePermanentExceptionMessage);

		public static DirectoryMonitoringScenario TenantRelocationErrorsFound = new DirectoryMonitoringScenario("TenantRelocationErrorsFound", Strings.TenantRelocationErrorsFoundExceptionMessage);

		public static DirectoryMonitoringScenario SCTNotFoundForAllVersions = new DirectoryMonitoringScenario("SCTNotFoundForAllVersions", Strings.SCTNotFoundForAllVersionsExceptionMessage);

		public static DirectoryMonitoringScenario SCTMonitoringScriptException = new DirectoryMonitoringScenario("SCTMonitoringScriptException", Strings.SCTMonitoringScriptExceptionMessage);

		public static DirectoryMonitoringScenario InocrrectSCTStateException = new DirectoryMonitoringScenario("InocrrectSCTStateException", Strings.InocrrectSCTStateExceptionMessage);

		public static DirectoryMonitoringScenario SCTStateMonitoringScriptException = new DirectoryMonitoringScenario("SCTStateMonitoringScriptException", Strings.SCTStateMonitoringScriptExceptionMessage);

		public static DirectoryMonitoringScenario DivergenceBetweenCAAndAD1003 = new DirectoryMonitoringScenario("DivergenceBetweenCAAndAD1003", Strings.DivergenceBetweenCAAndAD1003EscalationMessage);

		public static DirectoryMonitoringScenario CheckDCMMDivergenceScriptException = new DirectoryMonitoringScenario("CheckDCMMDivergenceScriptException", Strings.CheckDCMMDivergenceScriptExceptionMessage);

		public static DirectoryMonitoringScenario DivergenceInDefinition = new DirectoryMonitoringScenario("DivergenceInDefinition", Strings.DivergenceInDefinitionEscalationMessage);

		public static DirectoryMonitoringScenario DivergenceBetweenCAAndAD1006 = new DirectoryMonitoringScenario("DivergenceBetweenCAAndAD1006", Strings.DivergenceBetweenCAAndAD1006EscalationMessage);

		public static DirectoryMonitoringScenario DivergenceInSiteName = new DirectoryMonitoringScenario("DivergenceInSiteName", Strings.DivergenceInSiteNameEscalationMessage);

		public static DirectoryMonitoringScenario ProvisionedDCBelowMinimum = new DirectoryMonitoringScenario("ProvisionedDCBelowMinimum", Strings.ProvisionedDCBelowMinimumEscalationMessage);

		public static DirectoryMonitoringScenario CheckProvisionedDCException = new DirectoryMonitoringScenario("CheckProvisionedDCException", Strings.CheckProvisionedDCExceptionMessage);

		public static DirectoryMonitoringScenario FSMODCNotProvisioned = new DirectoryMonitoringScenario("FSMODCNotProvisioned", Strings.FSMODCNotProvisionedEscalationMessage);

		public static DirectoryMonitoringScenario CheckFsmoRolesScriptException = new DirectoryMonitoringScenario("CheckFsmoRolesScriptException", Strings.CheckFsmoRolesScriptExceptionMessage);

		public static DirectoryMonitoringScenario DirectoryConfigDiscrepancy = new DirectoryMonitoringScenario("DirectoryConfigDiscrepancy", Strings.DirectoryConfigDiscrepancyEscalationMessage);

		public static DirectoryMonitoringScenario DirectorySettingOverride = new DirectoryMonitoringScenario("DirectorySettingOverride", "A directory setting override validation failed. Please see event message for details");

		public static DirectoryMonitoringScenario CheckZombieDCScriptException = new DirectoryMonitoringScenario("CheckZombieDCScriptException", Strings.CheckZombieDCEscalateMessage);

		public static DirectoryMonitoringScenario DoMTConnectivity = new DirectoryMonitoringScenario("DoMTConnectivity", Strings.DoMTConnectivityEscalateMessage);
	}
}
