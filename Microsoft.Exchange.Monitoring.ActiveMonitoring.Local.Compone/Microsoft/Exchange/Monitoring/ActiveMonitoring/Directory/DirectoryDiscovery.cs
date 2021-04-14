using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory
{
	public sealed class DirectoryDiscovery : MaintenanceWorkItem
	{
		internal DumpMode DirectoryServiceRestartDumpMode { get; private set; }

		internal int ADConnectivityProbeThreshold { get; private set; }

		internal bool KDCStartOnProvisionDCEnabled { get; private set; }

		internal bool KDCStopOnMMDCEnabled { get; private set; }

		internal int ServiceStartStopRetryCount { get; private set; }

		internal int LiveIdProbeLatencyThreshold { get; private set; }

		internal int ReplicationThresholdInMins { get; private set; }

		internal double PercentageOfDCsThresholdExcludedForADHealth { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				string targetName = new ServerIdParameter().ToString();
				this.ReadAttribute("ServiceRestartDumpMode", DirectoryDiscovery.DefaultServiceRestartDump);
				try
				{
					this.DirectoryServiceRestartDumpMode = (DumpMode)Enum.Parse(typeof(DumpMode), this.ReadAttribute("ServiceRestartDumpMode", DirectoryDiscovery.DefaultServiceRestartDump), true);
					if (!Enum.IsDefined(typeof(DumpMode), this.DirectoryServiceRestartDumpMode))
					{
						this.DirectoryServiceRestartDumpMode = DumpMode.None;
					}
				}
				catch (ArgumentException)
				{
					this.DirectoryServiceRestartDumpMode = DumpMode.None;
				}
				int readADConnectivityThreshold = this.ReadAttribute("ADConnectivityProbeThreshold", 1000);
				this.ADConnectivityProbeThreshold = DirectoryUtils.GetADConnectivityProbeThresholdByEnviornment(readADConnectivityThreshold);
				this.KDCStartOnProvisionDCEnabled = this.ReadAttribute("KDCStartOnProvisionDCEnabled", true);
				this.KDCStopOnMMDCEnabled = this.ReadAttribute("KDCStopOnMMDCEnabled", true);
				this.ServiceStartStopRetryCount = this.ReadAttribute("ServiceStartStopRetryCount", 3);
				this.LiveIdProbeLatencyThreshold = this.ReadAttribute("LiveIdProbeLatencyThreshold", 15000);
				this.ReplicationThresholdInMins = this.ReadAttribute("ReplicationThresholdInMins", 30);
				this.PercentageOfDCsThresholdExcludedForADHealth = this.ReadAttribute("PercentageOfDCsThresholdExcludedForADHealth", 0.1);
				if (instance.ExchangeServerRoleEndpoint != null && (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled || instance.ExchangeServerRoleEndpoint.IsCentralAdminDatabaseRoleInstalled || instance.ExchangeServerRoleEndpoint.IsCentralAdminRoleInstalled || instance.ExchangeServerRoleEndpoint.IsClientAccessRoleInstalled || instance.ExchangeServerRoleEndpoint.IsFrontendTransportRoleInstalled || instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled || instance.ExchangeServerRoleEndpoint.IsUnifiedMessagingRoleInstalled))
				{
					this.CreateContext(DirectoryMonitoringStrings.MSExchangeADTopologyNotRunning, "MSExchangeADTopology", typeof(GenericServiceProbe), DirectoryUtils.ResponderChainType.Default, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.TopologyServiceConnectivity, "MSExchangeADTopology", typeof(TopologyServiceAvailabilityProbe), DirectoryUtils.ResponderChainType.Default, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.ActiveDirectoryConnectivity, targetName, typeof(ActiveDirectoryConnectivityProbe), new MonitoringPattern(300, 1000, 3, 290), DirectoryUtils.ResponderChainType.EscalateOnly, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.ActiveDirectoryConnectivityConfigDC, targetName, typeof(ActiveDirectoryConnectivityConfigDCProbe), new MonitoringPattern(300, 1000, 3, 290), DirectoryUtils.ResponderChainType.EscalateOnly, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.MSExchangeInformationStoreCannotContactAD, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(120, 1320, 10, 120), DirectoryUtils.ResponderChainType.Default, "Application", "MSExchangeIS*", "1121", "");
					this.CreateContext(DirectoryMonitoringStrings.TopoDiscoveryFailedAllServers, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(60, 660, 10, 120), DirectoryUtils.ResponderChainType.Default, "Application", "MSExchange ADAccess", "2102", "");
					this.CreateContext(DirectoryMonitoringStrings.DirectorySettingOverride, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "Application", "MSExchange ADAccess", "4122", "");
				}
				if (instance.WindowsServerRoleEndpoint != null && instance.WindowsServerRoleEndpoint.IsDirectoryServiceRoleInstalled)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "DirectoryDiscovery.DoWork: local server has directory service configured. Creating related performance counter monitors", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\DirectoryDiscovery.cs", 225);
					this.CreatePerformanceCounterMonitorsAndResponders(DirectoryDiscovery.DraPendingReplicationMonitorTrigger, DirectoryMonitoringStrings.ReplicationDraPendingReps, DirectoryUtils.ResponderChainType.NonUrgentEscalate);
					this.CreatePerformanceCounterMonitorsAndResponders(DirectoryDiscovery.DsNotificationQueueMonitorTrigger, DirectoryMonitoringStrings.DsNotificationQueue, DirectoryUtils.ResponderChainType.TraceAndEscalate);
					this.CreatePerformanceCounterMonitorsAndResponders(DirectoryDiscovery.OutStandingATQRequestsMonitorTrigger, DirectoryMonitoringStrings.OutStandingATQRequests, DirectoryUtils.ResponderChainType.TraceAndPutInMM);
					this.CreatePerformanceCounterMonitorsAndResponders(DirectoryDiscovery.LogicalDiskFreeMegabytesMonigorTrigger, DirectoryMonitoringStrings.LogicalDiskFreeMegabytes, DirectoryUtils.ResponderChainType.TraceAndEscalate);
					this.CreateContext(DirectoryMonitoringStrings.NTDSNotRunning, "NTDS", typeof(GenericServiceProbe), DirectoryUtils.ResponderChainType.Default, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.KDCServiceStatusTest, "KDC", typeof(TestKDCServiceProbe), new MonitoringPattern(1200, 3600, 3, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.SyntheticReplicationTransactionAll, targetName, typeof(SyntheticReplicationTransactionProbe), new MonitoringPattern(60, 180, 3, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.PassiveReplicationMonitorAll, targetName, typeof(PassiveReplicationMonitorProbe), new MonitoringPattern(300, 1560, 5, 120), DirectoryUtils.ResponderChainType.DomainController, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.PassiveADReplicationMonitorAll, targetName, typeof(PassiveADReplicationMonitorProbe), new MonitoringPattern(300, 660, 2, 180), DirectoryUtils.ResponderChainType.Scheduled, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.PassiveReplicationPerformanceCounterProbeAll, targetName, typeof(PassiveReplicationPerformanceCounterProbe), new MonitoringPattern(60, 3700, 60, 120), DirectoryUtils.ResponderChainType.None, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.TrustMonitorProbeAll, targetName, typeof(TrustMonitorProbe), new MonitoringPattern(3600, 10950, 3, 3000), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.ActiveDirectoryConnectivityLocal, targetName, typeof(ActiveDirectoryConnectivityOnADProbe), new MonitoringPattern(120, 780, 6, 120), DirectoryUtils.ResponderChainType.DomainController, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.ADCannotBoot, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS General", "2062", "");
					this.CreateContext(DirectoryMonitoringStrings.ADFailedToUpgradeIndex, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS Database", "2045", "");
					this.CreateContext(DirectoryMonitoringStrings.ADDatabaseCorrupt, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS General", "2062", "");
					this.CreateContext(DirectoryMonitoringStrings.ADReinstallServer, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS Replication", "1021", "");
					this.CreateContext(DirectoryMonitoringStrings.ADCannotFunctionNormally, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS General", "2011", "");
					this.CreateContext(DirectoryMonitoringStrings.ADCannotRecover, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "*ActiveDirectory_DomainService", "1003", "");
					this.CreateContext(DirectoryMonitoringStrings.ADSchemaPartitionFailed, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "*ActiveDirectory_DomainService", "1135", "");
					this.CreateContext(DirectoryMonitoringStrings.ADCannotRebuildIndex, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS Database", "2016", "");
					this.CreateContext(DirectoryMonitoringStrings.ADContentsUnpredictable, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS Backup", "1919", "");
					this.CreateContext(DirectoryMonitoringStrings.ADNoNTDSObject, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "*", "1038", "");
					this.CreateContext(DirectoryMonitoringStrings.ADVersionStore623, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS ISAM", "623", "");
					this.CreateContext(DirectoryMonitoringStrings.ADVersionStore2008, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS SDPROP", "2008", "");
					this.CreateContext(DirectoryMonitoringStrings.ADVersionStore1479, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS Replication", "1479", "");
					this.CreateContext(DirectoryMonitoringStrings.ADCheckSum, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS ISAM", "474", "");
					this.CreateContext(DirectoryMonitoringStrings.DeviceDegraded, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "Adaptec Storage Manager Agent", "301", "");
					this.CreateContext(DirectoryMonitoringStrings.RaidDegraded, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "Adaptec Storage Manager Agent", "338", "");
					this.CreateContext(DirectoryMonitoringStrings.ReplicationDisabled, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "*ActiveDirectory_DomainService", "1008", "");
					this.CreateContext(DirectoryMonitoringStrings.ReplicationUnableToCompleteTopology, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS KCC", "1130", "");
					this.CreateContext(DirectoryMonitoringStrings.ReplicationKccInsufficientInfo, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS KCC", "1311", "");
					this.CreateContext(DirectoryMonitoringStrings.ReplicationBridgehead, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "*ActiveDirectory_DomainService", "1567", "");
					this.CreateContext(DirectoryMonitoringStrings.ReplicationIncompatibleVector, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS KCC", "1910", "");
					this.CreateContext(DirectoryMonitoringStrings.ReplicationSlowADWrites, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS ISAM", "508", "");
					this.CreateContext(DirectoryMonitoringStrings.RidPoolRequestFailed, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS ISAM", "16645,16651", "");
					this.CreateContext(DirectoryMonitoringStrings.DatabaseCorruption, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.RenameNTDSPowerOff, "Directory Service", "NTDS ISAM", "447", "");
					this.CreateContext(DirectoryMonitoringStrings.ADDataIssue, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.RenameNTDSPowerOff, "Directory Service", "NTDS ISAM", "448", "");
					this.CreateContext(DirectoryMonitoringStrings.NTDSCorruption, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.RenameNTDSPowerOff, "Directory Service", "NTDS ISAM", "467", "");
					this.CreateContext(DirectoryMonitoringStrings.ADDatabaseCorruption, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.RenameNTDSPowerOff, "Directory Service", "NTDS ISAM", "478", "");
					this.CreateContext(DirectoryMonitoringStrings.ADDatabaseCorruption1017, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "NTDS General", "1017", "");
					this.CreateContext(DirectoryMonitoringStrings.ReplicationOutdatedObjectsFailed, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.RenameNTDSPowerOff, "Directory Service", "*", "1988", "");
					this.CreateContext(DirectoryMonitoringStrings.ReplicationFailures, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 1900, 3, 120), DirectoryUtils.ResponderChainType.DomainController, "Directory Service", "*", "2108", "");
					this.CreateContext(DirectoryMonitoringStrings.RidConsumption, targetName, typeof(RidMonitorProbe), DirectoryUtils.ResponderChainType.EscalateOnly, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.RidSetValidation, targetName, typeof(RidSetReferencesMonitorProbe), DirectoryUtils.ResponderChainType.EscalateOnly, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.RemoteDomainControllerState, targetName, typeof(RemoteDomainControllerStateProbe), new MonitoringPattern(900, 2700, 3, 870), DirectoryUtils.ResponderChainType.PutMultipleDCInMM, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.SyntheticReplicationMonitorRID, targetName, typeof(SyntheticReplicationMonitorProbe), new MonitoringPattern(900, 2800, 3, 120), DirectoryUtils.ResponderChainType.Scheduled, "", "", "", "");
				}
				if (LocalEndpointManager.IsDataCenter && instance.ExchangeServerRoleEndpoint != null && (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled || instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled))
				{
					this.CreateContext(DirectoryMonitoringStrings.MSExchangeProtectedServiceHostNotRunning, "MSExchangeProtectedServiceHost", typeof(GenericServiceProbe), DirectoryUtils.ResponderChainType.Default, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.LiveIdAuthenticationConnectivity, "MSExchangeProtectedServiceHost", typeof(LiveIdAuthenticationProbe), DirectoryUtils.ResponderChainType.LiveId, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.GlsConnectivity, targetName, typeof(GLSConnectivityProbe), new MonitoringPattern(180, 1900, 10, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.OfflineGLS, targetName, typeof(OfflineGLSProbe), new MonitoringPattern(900, 7200, 8, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.PermanentExceptionInRelocationService, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(120, 120, 1, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "Application", "MSExchange ADAccess", "4032", "");
					this.CreateContext(DirectoryMonitoringStrings.MSExchangeProtectedServiceHostCrashing, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(900, 3600, 3, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "Application", "MSExchangeProtectedServiceHost", "4999", "2004");
					this.CreateContext(DirectoryMonitoringStrings.InocrrectSCTStateException, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(43200, 43200, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Shared Configuration Tenant State Monitor", "1001", "");
					this.CreateContext(DirectoryMonitoringStrings.SCTStateMonitoringScriptException, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(43200, 43200, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Shared Configuration Tenant State Monitor", "1005", "");
					this.CreateContext(DirectoryMonitoringStrings.NtlmConnectivity, targetName, typeof(NtlmConnectivityProbe), DirectoryUtils.ResponderChainType.Default, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.KerbAuthFailure, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(120, 2160, 15, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "System", "Microsoft-Windows-Security-Kerberos", "4", "");
					this.CreateContext(DirectoryMonitoringStrings.PACValidationFailure, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(120, 2160, 15, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "System", "Microsoft-Windows-Security-Kerberos", "7", "");
				}
				if (LocalEndpointManager.IsDataCenter && instance.ExchangeServerRoleEndpoint != null && instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled && DirectoryUtils.IsPrimaryActiveManager())
				{
					this.CreateContext(DirectoryMonitoringStrings.TenantRelocationErrorsFound, targetName, typeof(TenantRelocationErrorProbe), new MonitoringPattern(3600, 3600, 1, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "", "", "", "");
					this.CreateContext(DirectoryMonitoringStrings.SCTNotFoundForAllVersions, targetName, typeof(SharedConfigurationTenantProbe), new MonitoringPattern(43200, 43200, 1, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "", "", "", "");
				}
				if (LocalEndpointManager.IsDataCenter && instance != null && instance.ExchangeServerRoleEndpoint.IsCentralAdminRoleInstalled)
				{
					this.CreateContext(DirectoryMonitoringStrings.SCTNotFoundForAllVersions, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(300, 300, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Shared Configuration Tenant Recovery", "1001", "");
					this.CreateContext(DirectoryMonitoringStrings.SCTMonitoringScriptException, targetName, typeof(GenericEventLogProbe), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Shared Configuration Tenant Recovery", "1005", "");
					this.CreateContext(DirectoryMonitoringStrings.DivergenceBetweenCAAndAD1003, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Domain Controller Maintenance Mode", "1003", "");
					this.CreateContext(DirectoryMonitoringStrings.CheckDCMMDivergenceScriptException, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Domain Controller Maintenance Mode", "1004", "");
					this.CreateContext(DirectoryMonitoringStrings.DivergenceInDefinition, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Domain Controller Maintenance Mode", "1005", "");
					this.CreateContext(DirectoryMonitoringStrings.DivergenceBetweenCAAndAD1006, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Domain Controller Maintenance Mode", "1006", "");
					this.CreateContext(DirectoryMonitoringStrings.DivergenceInSiteName, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Domain Controller Maintenance Mode", "1007", "");
					this.CreateContext(DirectoryMonitoringStrings.ProvisionedDCBelowMinimum, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Monitoring Provisioned DCs", "1001", "");
					this.CreateContext(DirectoryMonitoringStrings.CheckProvisionedDCException, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Monitoring Provisioned DCs", "1002", "");
					this.CreateContext(DirectoryMonitoringStrings.FSMODCNotProvisioned, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.EscalateOnly, "Application", "MSExchange FSMO Roles", "1001", "1003");
					this.CreateContext(DirectoryMonitoringStrings.CheckFsmoRolesScriptException, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange FSMO Roles", "1002", "");
					this.CreateContext(DirectoryMonitoringStrings.DirectoryConfigDiscrepancy, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "BPA", "6003,6004,6005,6006,6007,6008,6009,6010,6011,6012", "");
					this.CreateContext(DirectoryMonitoringStrings.CheckZombieDCScriptException, targetName, typeof(GenericEventLogProbe), new MonitoringPattern(600, 600, 1, 120), DirectoryUtils.ResponderChainType.NonUrgentEscalate, "Application", "MSExchange Monitoring ZombieDCs", "1009", "");
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "DirectoryDiscovery.DoWork: EndpointManagerEndpointUninitializedException is caught.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\DirectoryDiscovery.cs", 327);
			}
		}

		private void CreatePerformanceCounterMonitorsAndResponders(string triggerName, DirectoryMonitoringScenario scenario, DirectoryUtils.ResponderChainType responderChainType = DirectoryUtils.ResponderChainType.EscalateOnly)
		{
			string text = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, triggerName, null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(scenario.MonitorName, text, "Directory Service", ExchangeComponent.AD, 1, true, 900);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate AD service health is not impacted by performance issues";
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, (int)DirectoryDiscovery.DefaultDegradedTransitionSpan.TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)DirectoryDiscovery.DefaultUnhealthyTransitionSpan.TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)DirectoryDiscovery.DefaultUnrecoverableTransitionSpan.TotalSeconds)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string text2 = Strings.EscalationMessageUnhealthy(string.Format("Directory Perf Monitor {0} detected perf counter {1} is above the threshold. Oncall please investigate", scenario.MonitorName, text));
			if (responderChainType == DirectoryUtils.ResponderChainType.NonUrgentEscalate)
			{
				ResponderDefinition definition = EscalateResponder.CreateDefinition(scenario.EscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), DirectoryGeneralUtils.GetLocalFQDN(), ServiceHealthStatus.Degraded, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text2, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
				return;
			}
			switch (responderChainType)
			{
			case DirectoryUtils.ResponderChainType.TraceAndEscalate:
			{
				TraceLogResponder.TraceAttributes traceAttributes = new TraceLogResponder.TraceAttributes(scenario.ADDiagTraceResponderName, ExchangeComponent.AD.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), ExchangeComponent.AD.Name, ServiceHealthStatus.Degraded, DirectoryDiscovery.ADDiagTraceGuid, string.Empty, string.Empty, 30, string.Empty, PerformanceCounterNotificationItem.GenerateResultName(text), true);
				ResponderDefinition definition2 = TraceLogResponder.CreateDefinition(traceAttributes);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
				ResponderDefinition definition3 = EscalateResponder.CreateDefinition(scenario.EscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.Degraded, ExchangeComponent.AD.EscalationTeam, Strings.EscalationSubjectUnhealthy, text2, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition3, base.TraceContext);
				return;
			}
			case DirectoryUtils.ResponderChainType.TraceAndPutInMM:
			{
				TraceLogResponder.TraceAttributes traceAttributes2 = new TraceLogResponder.TraceAttributes(scenario.ADDiagTraceResponderName, ExchangeComponent.AD.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), ExchangeComponent.AD.Name, ServiceHealthStatus.Degraded, DirectoryDiscovery.ADDiagTraceGuid, string.Empty, string.Empty, 30, string.Empty, PerformanceCounterNotificationItem.GenerateResultName(text), true);
				ResponderDefinition definition4 = TraceLogResponder.CreateDefinition(traceAttributes2);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition4, base.TraceContext);
				ResponderDefinition definition5 = PutDCInMMResponder.CreateDefinition(scenario.PutDCInMMResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), DirectoryGeneralUtils.GetLocalFQDN(), ServiceHealthStatus.Unhealthy, true, "DomainController");
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition5, base.TraceContext);
				ResponderDefinition definition6 = CheckDCInMMEscalateResponder.CreateDefinition(scenario.CheckDCInMMEscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), DirectoryGeneralUtils.GetLocalFQDN(), ServiceHealthStatus.Unrecoverable, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text2);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition6, base.TraceContext);
				return;
			}
			default:
				return;
			}
		}

		private void CreateContext(DirectoryMonitoringScenario scenario, string targetName, Type probeType, DirectoryUtils.ResponderChainType responderChainType = DirectoryUtils.ResponderChainType.Default, string logName = "", string providerName = "", string redEvent = "", string greenEvent = "")
		{
			MonitoringPattern monitoringPattern = new MonitoringPattern(120, 540, 4, 120);
			this.CreateContext(scenario, targetName, probeType, monitoringPattern, responderChainType, logName, providerName, redEvent, greenEvent);
		}

		private void CreateContext(DirectoryMonitoringScenario scenario, string targetName, Type probeType, MonitoringPattern monitoringPattern, DirectoryUtils.ResponderChainType responderChainType = DirectoryUtils.ResponderChainType.Default, string logName = "", string providerName = "", string redEvent = "", string greenEvent = "")
		{
			ProbeDefinition probeDefinition = this.CreateProbe(targetName, probeType, scenario.ProbeName, monitoringPattern.RecurrenceInSeconds, monitoringPattern.TimeoutInSeconds, logName, providerName, redEvent, greenEvent);
			if (probeDefinition == null)
			{
				return;
			}
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = DirectoryMonitor.CreateMonitor(targetName, scenario.MonitorName, monitoringPattern.RecurrenceInSeconds, monitoringPattern.MonitoringIntervalInSeconds, monitoringPattern.MonitoringThreshold, scenario.ProbeName, DirectoryDiscovery.AssemblyPath, 3, base.TraceContext);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, (int)DirectoryDiscovery.DefaultDegradedTransitionSpan.TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, (int)DirectoryDiscovery.DefaultUnhealthyTransitionSpan.TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)DirectoryDiscovery.DefaultUnrecoverableTransitionSpan.TotalSeconds)
			};
			if (scenario.AllowCorrelationToMonitoring)
			{
				monitorDefinition.AllowCorrelationToMonitor = true;
			}
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = string.Format("Validate AD health is not impacted by {0} issues", scenario.Scenario);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string text = Strings.EscalationMessageUnhealthy("{Probe.ResultName} Failed with Error message: {Probe.Error}") + " Exception Details: {Probe.Exception}";
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				int minimumRequiredServers = -1;
				string text2 = string.Empty;
				if (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
				{
					text2 = "Cafe";
					CafeUtils.ConfigureResponderForCafeMinimumValues(null, null, delegate(int minRequired)
					{
						minimumRequiredServers = minRequired;
					}, base.TraceContext);
				}
				else if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					text2 = "Dag";
				}
				else if (instance.ExchangeServerRoleEndpoint.IsCentralAdminRoleInstalled)
				{
					text2 = "CentralAdmin";
				}
				else if (instance.WindowsServerRoleEndpoint.IsDirectoryServiceRoleInstalled)
				{
					text2 = "DomainController";
				}
				switch (responderChainType)
				{
				case DirectoryUtils.ResponderChainType.Default:
				{
					string restartResponderName = scenario.RestartResponderName;
					string monitorName = scenario.MonitorName;
					ServiceHealthStatus responderTargetState = ServiceHealthStatus.Degraded;
					string name = ExchangeComponent.AD.Name;
					ResponderDefinition definition = RestartServiceResponder.CreateDefinition(restartResponderName, monitorName, targetName, responderTargetState, 15, 120, 0, false, this.DirectoryServiceRestartDumpMode, null, 20.0, 30, name, null, true, true, null, false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
					string restartServerResponderName = scenario.RestartServerResponderName;
					string monitorName2 = scenario.MonitorName;
					ServiceHealthStatus responderTargetState2 = ServiceHealthStatus.Unhealthy;
					string name2 = ExchangeComponent.AD.Name;
					string throttleGroupName = text2;
					ResponderDefinition definition2 = ForceRebootServerResponder.CreateDefinition(restartServerResponderName, monitorName2, responderTargetState2, null, minimumRequiredServers, "", "", "Datacenter, Stamp", "RecoveryData", "ArbitrationOnly", name2, true, throttleGroupName, true);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
					ResponderDefinition definition3 = EscalateResponder.CreateDefinition(scenario.EscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition3, base.TraceContext);
					break;
				}
				case DirectoryUtils.ResponderChainType.LiveId:
				{
					string restartResponderName2 = scenario.RestartResponderName;
					string monitorName3 = scenario.MonitorName;
					ServiceHealthStatus responderTargetState3 = ServiceHealthStatus.Degraded;
					string name3 = ExchangeComponent.AD.Name;
					ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition(restartResponderName2, monitorName3, targetName, responderTargetState3, 15, 120, 0, false, this.DirectoryServiceRestartDumpMode, null, 20.0, 30, name3, null, true, true, null, false);
					responderDefinition.AssemblyPath = typeof(RestartServiceForSpecificExceptionResponder).Assembly.Location;
					responderDefinition.TypeName = typeof(RestartServiceForSpecificExceptionResponder).FullName;
					responderDefinition.Attributes["ExceptionType"] = DirectoryUtils.ExceptionType.ProtectedServiceHostIssue.ToString();
					base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
					string restartServerResponderName2 = scenario.RestartServerResponderName;
					string monitorName4 = scenario.MonitorName;
					ServiceHealthStatus responderTargetState4 = ServiceHealthStatus.Unhealthy;
					string name4 = ExchangeComponent.AD.Name;
					string throttleGroupName2 = text2;
					ResponderDefinition responderDefinition2 = ForceRebootServerResponder.CreateDefinition(restartServerResponderName2, monitorName4, responderTargetState4, null, minimumRequiredServers, "", "", "Datacenter, Stamp", "RecoveryData", "ArbitrationOnly", name4, true, throttleGroupName2, true);
					responderDefinition2.AssemblyPath = typeof(RestartServerForSpecificExceptionResponder).Assembly.Location;
					responderDefinition2.TypeName = typeof(RestartServerForSpecificExceptionResponder).FullName;
					responderDefinition2.Attributes["ExceptionType"] = DirectoryUtils.ExceptionType.ProtectedServiceHostIssue.ToString();
					base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
					ResponderDefinition definition4 = EscalateResponder.CreateDefinition(scenario.EscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition4, base.TraceContext);
					break;
				}
				case DirectoryUtils.ResponderChainType.DomainController:
				{
					ResponderDefinition definition5 = PutDCInMMResponder.CreateDefinition(scenario.PutDCInMMResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Degraded, true, "DomainController");
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition5, base.TraceContext);
					ResponderDefinition definition6 = CheckDCInMMEscalateResponder.CreateDefinition(scenario.CheckDCInMMEscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition6, base.TraceContext);
					break;
				}
				case DirectoryUtils.ResponderChainType.EscalateOnly:
				{
					ResponderDefinition definition7 = EscalateResponder.CreateDefinition(scenario.EscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Degraded, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition7, base.TraceContext);
					break;
				}
				case DirectoryUtils.ResponderChainType.NonUrgentEscalate:
				{
					ResponderDefinition definition8 = EscalateResponder.CreateDefinition(scenario.EscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Degraded, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition8, base.TraceContext);
					break;
				}
				case DirectoryUtils.ResponderChainType.Scheduled:
				{
					ResponderDefinition definition9 = EscalateResponder.CreateDefinition(scenario.EscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Degraded, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition9, base.TraceContext);
					break;
				}
				case DirectoryUtils.ResponderChainType.PutMultipleDCInMM:
				{
					ResponderDefinition definition10 = PutMultipleDCInMMResponder.CreateDefinition(scenario.PutMultipleDCInMMResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Degraded, "DomainController");
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition10, base.TraceContext);
					ResponderDefinition definition11 = CheckMultipleDCInMMEscalateResponder.CreateDefinition(scenario.CheckMultipleDCInMMEscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text, NotificationServiceClass.UrgentInTraining);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition11, base.TraceContext);
					break;
				}
				case DirectoryUtils.ResponderChainType.RenameNTDSPowerOff:
				{
					ResponderDefinition definition12 = RenameNTDSPowerOffResponder.CreateDefinition(scenario.RenameNTDSPowerOffResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), ServiceHealthStatus.Degraded, "DomainController");
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition12, base.TraceContext);
					ResponderDefinition definition13 = EscalateResponder.CreateDefinition(scenario.EscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition13, base.TraceContext);
					break;
				}
				case DirectoryUtils.ResponderChainType.DoMT:
				{
					string restartResponderName3 = scenario.RestartResponderName;
					string monitorName5 = scenario.MonitorName;
					ServiceHealthStatus responderTargetState5 = ServiceHealthStatus.Degraded;
					string name5 = ExchangeComponent.AD.Name;
					ResponderDefinition definition14 = RestartServiceResponder.CreateDefinition(restartResponderName3, monitorName5, targetName, responderTargetState5, 15, 120, 0, false, this.DirectoryServiceRestartDumpMode, null, 20.0, 30, name5, null, true, true, null, false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition14, base.TraceContext);
					ResponderDefinition definition15 = EscalateResponder.CreateDefinition(scenario.EscalateResponderName, ExchangeComponent.AD.Name, scenario.MonitorName, monitorDefinition.ConstructWorkItemResultName(), targetName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.AD.EscalationTeam, scenario.EscalationMessageSubject, text, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition15, base.TraceContext);
					break;
				}
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "DirectoryDiscovery.CreateContext: EndpointManagerEndpointUninitializedException is caught.", null, "CreateContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\DirectoryDiscovery.cs", 839);
			}
		}

		private ProbeDefinition CreateProbe(string targetResource, Type probeType, string probeName, int recurrenceInterval, int timeout, string logName = "", string providerName = "", string redEvent = "", string greenEvent = "")
		{
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "DirectoryDiscovery.CreateProbe: Creating {0} for {1}", probeName, targetResource, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\DirectoryDiscovery.cs", 867);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = probeType.Assembly.Location;
			probeDefinition.TypeName = probeType.FullName;
			probeDefinition.Name = probeName;
			probeDefinition.ServiceName = ExchangeComponent.AD.Name;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			probeDefinition.TimeoutSeconds = timeout;
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.TargetResource = targetResource;
			probeDefinition.Attributes.Add("RidsLeftThreshold", 25000.ToString());
			probeDefinition.Attributes.Add("RidsLeftLimit", 395000000.ToString());
			probeDefinition.Attributes.Add("RidsLeftLimitLowValue", 5000000.ToString());
			probeDefinition.Attributes.Add("RidsLeftLimitSDF", 1000000.ToString());
			probeDefinition.Attributes.Add("ADConnectivityThreshold", this.ADConnectivityProbeThreshold.ToString());
			probeDefinition.Attributes.Add("KDCStartOnProvisionDCEnabled", this.KDCStartOnProvisionDCEnabled.ToString());
			probeDefinition.Attributes.Add("KDCStopOnMMDCEnabled", this.KDCStopOnMMDCEnabled.ToString());
			probeDefinition.Attributes.Add("ServiceStartStopRetryCount", this.ServiceStartStopRetryCount.ToString());
			probeDefinition.Attributes.Add("LiveIdProbeLatencyThreshold", this.LiveIdProbeLatencyThreshold.ToString());
			probeDefinition.Attributes.Add("ReplicationThresholdInMins", this.ReplicationThresholdInMins.ToString());
			probeDefinition.Attributes.Add("PercentageOfDCsThresholdExcludedForADHealth", this.PercentageOfDCsThresholdExcludedForADHealth.ToString());
			if (!string.IsNullOrEmpty(logName))
			{
				probeDefinition.Attributes[GenericEventLogProbe.LogNameAttrName] = logName;
				probeDefinition.Attributes[GenericEventLogProbe.ProviderNameAttrName] = providerName;
				probeDefinition.Attributes[GenericEventLogProbe.RedEventsAttrName] = redEvent;
				probeDefinition.Attributes[GenericEventLogProbe.GreenEventsAttrName] = greenEvent;
			}
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "DirectoryDiscovery.CreateProbe: Created {0} for {1}", probeName, targetResource, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\DirectoryDiscovery.cs", 904);
			return probeDefinition;
		}

		private const int MaxRetryAttempt = 3;

		private const int RidLeftThreshold = 25000;

		private const int RidsLeftLimit = 395000000;

		private const int RidsLeftLimitLowValue = 5000000;

		private const int RidsLeftLimitSDF = 1000000;

		private const int DefaultMinimumFreeDiskPercent = 20;

		private const int DefaultMaximumDumpDurationInSeconds = 30;

		private const int DefaultADConnectivityProdThreshold = 1000;

		private const int AdDiagTraceDurationSeconds = 30;

		private const int DefaultServiceStartStopRetryCount = 3;

		private const int DefaultLiveIdProbeLatencyThreshold = 15000;

		private const int DefaultReplicationThresholdInMins = 30;

		private const double DefaultPercentageOfDCsThresholdExcludedForADHealth = 0.1;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string DraPendingReplicationMonitorTrigger = "DraPendingReplicationTrigger_Error";

		private static readonly string DsNotificationQueueMonitorTrigger = "DsNotificationQueueTrigger_Error";

		private static readonly string OutStandingATQRequestsMonitorTrigger = "OutStandingATQRequestsTrigger_Error";

		private static readonly string LogicalDiskFreeMegabytesMonigorTrigger = "LogicalDiskFreeMegabytesTrigger_Error";

		private static readonly string DefaultServiceRestartDump = "None";

		private static readonly string ADDiagTraceGuid = "1C83B2FC-C04F-11D1-8AFC-00C04FC21914,8E598056-8993-11D2-819E-0000F875A064,BBA3ADD2-C229-4CDB-AE2B-57EB6966B0C4,24DB8964-E6BC-11D1-916A-0000F8045B04,CC85922F-DB41-11D2-9244-006008269001,C92CF544-91B3-4DC0-8E11-C580339A0BF8";

		private static readonly TimeSpan DefaultDegradedTransitionSpan = TimeSpan.FromMinutes(0.0);

		private static readonly TimeSpan DefaultUnhealthyTransitionSpan = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan DefaultUnrecoverableTransitionSpan = TimeSpan.FromMinutes(15.0);
	}
}
