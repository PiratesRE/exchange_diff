using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace
{
	public sealed class MailboxSpaceDiscovery : MaintenanceWorkItem
	{
		internal static void PopulateProbeDefinition(ProbeDefinition probeDefinition, string targetResource, string probeTypeName, string probeName, TimeSpan recurrenceInterval, TimeSpan timeoutInterval)
		{
			probeDefinition.AssemblyPath = MailboxSpaceDiscovery.AssemblyPath;
			probeDefinition.TypeName = probeTypeName;
			probeDefinition.Name = probeName;
			probeDefinition.ServiceName = MailboxSpaceDiscovery.mailboxSpaceHealthsetName;
			probeDefinition.RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds;
			probeDefinition.TimeoutSeconds = (int)timeoutInterval.TotalSeconds;
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.TargetResource = targetResource;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.DoWork: Mailbox role is not installed on this server, no need to create database space related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 227);
				}
				else if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.StoreTracer, base.TraceContext, "MailboxSpaceDiscovery.DoWork: No mailbox database found on this server", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 238);
				}
				else
				{
					MailboxSpaceDiscovery.isTestTopology = ExEnvironment.IsTest;
					MailboxSpaceDiscovery.isDatacenter = LocalEndpointManager.IsDataCenter;
					MailboxSpaceDiscovery.isDatacenterDedicated = LocalEndpointManager.IsDataCenterDedicated;
					foreach (MailboxDatabaseInfo dbInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
					{
						this.CreateDatabaseSizeContext(dbInfo);
						this.CreateStorageSpaceNotificationMonitor(dbInfo);
					}
					this.CreateDatabaseSizeEscalateWorkitems();
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.DoWork: EndpointManagerEndpointUninitializedException is caught.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 273);
			}
		}

		private void CreateDatabaseSizeContext(MailboxDatabaseInfo dbInfo)
		{
			string mailboxDatabaseName = dbInfo.MailboxDatabaseName;
			string targetExtension = dbInfo.MailboxDatabaseGuid.ToString();
			TimeSpan monitoringInterval = new TimeSpan(2L * MailboxSpaceDiscovery.databaseSizeRecurrence.Ticks);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateDatabaseSizeContext: Starting creation of database size context", null, "CreateDatabaseSizeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 290);
			ProbeDefinition probeDefinition = this.CreateProbe(mailboxDatabaseName, MailboxSpaceDiscovery.DBSpaceProbeTypeName, "DatabaseSpaceProbe", MailboxSpaceDiscovery.databaseSizeRecurrence, MailboxSpaceDiscovery.databaseSizeTimeout);
			probeDefinition.TargetExtension = targetExtension;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitor(mailboxDatabaseName, MailboxSpaceDiscovery.DBSizeMonitorTypeName, "DatabaseSizeMonitor", MailboxSpaceDiscovery.databaseSizeRecurrence, monitoringInterval, null, MailboxSpaceDiscovery.databaseSizeTimeout, probeDefinition.ConstructWorkItemResultName());
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, new TimeSpan(6L * MailboxSpaceDiscovery.databaseSizeRecurrence.Ticks))
			};
			if ((MailboxSpaceDiscovery.isDatacenter || MailboxSpaceDiscovery.isDatacenterDedicated) && !MailboxSpaceDiscovery.isTestTopology)
			{
				monitorDefinition.Attributes["DatabaseBufferThreshold"] = MailboxSpaceDiscovery.DatabaseBufferThreshold.ToString();
				monitorDefinition.Attributes["DatabaseLogsThreshold"] = MailboxSpaceDiscovery.DatabaseLogsThreshold.ToString();
				monitorDefinition.Attributes["SearchSizeFactorThreshold"] = 0.2.ToString();
				monitorDefinition.Attributes["NumberOfDatabasesPerDisk"] = 4.ToString();
			}
			else
			{
				monitorDefinition.Attributes["DatabaseBufferThreshold"] = "1MB";
				monitorDefinition.Attributes["DatabaseLogsThreshold"] = "1MB";
				monitorDefinition.Attributes["SearchSizeFactorThreshold"] = 0.2.ToString();
				monitorDefinition.Attributes["NumberOfDatabasesPerDisk"] = "1";
			}
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate MailboxSpace health is not impacted by database health issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = this.CreateResponder(mailboxDatabaseName, MailboxSpaceDiscovery.DatabaseProvisioningResponderTypeName, "DatabaseSizeProvisioning", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, MailboxSpaceDiscovery.databaseSizeRecurrence, MailboxSpaceDiscovery.databaseSizeWaitInterval, MailboxSpaceDiscovery.databaseSizeTimeout, ServiceHealthStatus.Unhealthy);
			responderDefinition.TargetExtension = targetExtension;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = this.CreateResponder(mailboxDatabaseName, MailboxSpaceDiscovery.DatabaseSizeEscalationNotificationResponderTypeName, "DatabaseSizeEscalationNotification", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, MailboxSpaceDiscovery.databaseSizeWaitInterval, ServiceHealthStatus.Unrecoverable);
			responderDefinition2.ActionOnCorrelatedMonitors = CorrelatedMonitorAction.GenerateException;
			responderDefinition2.CorrelatedMonitors = new CorrelatedMonitorInfo[]
			{
				StoreMonitoringHelpers.GetStoreCorrelation(mailboxDatabaseName)
			};
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateDatabaseSizeContext: Finished creation of database size context", null, "CreateDatabaseSizeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 391);
		}

		private void CreateStorageSpaceNotificationMonitor(MailboxDatabaseInfo dbInfo)
		{
			string text = dbInfo.MailboxDatabaseName.ToUpper();
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("StorageLogicalDriveSpaceMonitor", NotificationItem.GenerateResultName("MSExchangeDagMgmt", "EdbAndLogVolSpace", text), ExchangeComponent.MailboxSpace.Name, ExchangeComponent.MailboxSpace, 1, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = text;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate MailboxSpace log and edb volume spaces are adequate.";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("StorageLogicalDriveSpaceEscalate", ExchangeComponent.MailboxSpace.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), text, ServiceHealthStatus.Unhealthy, "High Availability", Strings.LogVolumeSpaceEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.LogVolumeSpaceEscalationMessage(dbInfo.MailboxDatabaseName), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateLogVolumeSpaceNotificationMonitor: Finished creation of log volume space context", null, "CreateStorageSpaceNotificationMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 448);
		}

		private void CreateDatabaseLogicalPhysicalSizeRatioContext(MailboxDatabaseInfo dbInfo)
		{
			WTFDiagnostics.TraceDebug(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateDatabaseLogicalPhysicalSizeRatioContext: Starting creation of database logical physical size ratio context", null, "CreateDatabaseLogicalPhysicalSizeRatioContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 460);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "DatabaseLogicalPhysicalSizeRatioTrigger_Error", dbInfo.MailboxDatabaseName);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("DatabaseLogicalPhysicalSizeRatioMonitor", sampleMask, ExchangeComponent.MailboxSpace.Name, ExchangeComponent.MailboxSpace, 1, true, (int)MailboxSpaceDiscovery.databaseLogicalPhysicalSizeRatioMonitoringInterval.TotalSeconds);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate MailboxSpace health is not impacted by database health issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition = this.CreateResponder(dbInfo.MailboxDatabaseName, MailboxSpaceDiscovery.EscalationNotificationResponderTypeName, "DatabaseLogicalPhysicalSizeRatioEscalationNotification", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, MailboxSpaceDiscovery.databaseLogicalPhysicalSizeRatioWaitInterval, ServiceHealthStatus.Unhealthy);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			ResponderDefinition definition2 = SetMonitorStateRepairingResponder.CreateDefinition("SetDatabaseLogicalPhysicalSizeRatioMonitorStateRepairing", ExchangeComponent.MailboxSpace.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, monitorDefinition.Name, ServiceHealthStatus.Unhealthy, true, 0);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateDatabaseLogicalPhysicalSizeRatioContext: Finished creation of database logical physical size ratio context", null, "CreateDatabaseLogicalPhysicalSizeRatioContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 517);
		}

		private ProbeDefinition CreateProbe(string targetResource, string probeTypeName, string probeName, TimeSpan recurrenceInterval, TimeSpan timeoutInterval)
		{
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateProbe: Creating {0} for {1}", probeName, targetResource, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 534);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			MailboxSpaceDiscovery.PopulateProbeDefinition(probeDefinition, targetResource, probeTypeName, probeName, recurrenceInterval, timeoutInterval);
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateProbe: Created {0} for {1}", probeName, targetResource, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 543);
			return probeDefinition;
		}

		private MonitorDefinition CreateMonitor(string targetResource, string monitorTypeName, string monitorName, TimeSpan recurrenceInterval, TimeSpan monitoringInterval, int? monitoringThreshold, TimeSpan timeoutInterval, string sampleMask)
		{
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateMonitor: Creating {0} for {1}", monitorName, targetResource, null, "CreateMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 575);
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = MailboxSpaceDiscovery.AssemblyPath;
			monitorDefinition.TypeName = monitorTypeName;
			monitorDefinition.Name = monitorName;
			monitorDefinition.ServiceName = MailboxSpaceDiscovery.mailboxSpaceHealthsetName;
			monitorDefinition.RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			monitorDefinition.TimeoutSeconds = (int)timeoutInterval.TotalSeconds;
			monitorDefinition.MaxRetryAttempts = 3;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.MonitoringIntervalSeconds = (int)monitoringInterval.TotalSeconds;
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.Component = ExchangeComponent.MailboxSpace;
			if (monitoringThreshold != null)
			{
				monitorDefinition.MonitoringThreshold = (double)monitoringThreshold.Value;
			}
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateMonitor: Created {0} for {1}", monitorName, targetResource, null, "CreateMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 600);
			return monitorDefinition;
		}

		private ResponderDefinition CreateResponder(string targetResource, string responderTypeName, string responderName, string alertMask, string alertTypeId, TimeSpan recurrenceInterval, TimeSpan waitInterval, ServiceHealthStatus targetHealthState)
		{
			return this.CreateResponder(targetResource, responderTypeName, responderName, alertMask, alertTypeId, recurrenceInterval, waitInterval, TimeSpan.FromMinutes(2.0), targetHealthState);
		}

		private ResponderDefinition CreateResponder(string targetResource, string responderTypeName, string responderName, string alertMask, string alertTypeId, TimeSpan recurrenceInterval, TimeSpan waitInterval, TimeSpan timeoutInterval, ServiceHealthStatus targetHealthState)
		{
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateResponder: Creating {0} for {1}", responderName, targetResource, null, "CreateResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 668);
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = MailboxSpaceDiscovery.AssemblyPath;
			responderDefinition.TypeName = responderTypeName;
			responderDefinition.Name = responderName;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds;
			responderDefinition.TimeoutSeconds = (int)timeoutInterval.TotalSeconds;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.ServiceName = MailboxSpaceDiscovery.mailboxSpaceHealthsetName;
			responderDefinition.WaitIntervalSeconds = (int)waitInterval.TotalSeconds;
			responderDefinition.TargetHealthState = targetHealthState;
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "MailboxSpaceDiscovery.CreateResponder: Created {0} for {1}", responderName, targetResource, null, "CreateResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\MailboxSpaceDiscovery.cs", 690);
			return responderDefinition;
		}

		private void CreateDatabaseSizeEscalateWorkitems()
		{
			TimeSpan timeSpan = new TimeSpan(MailboxSpaceDiscovery.databaseSizeRecurrence.Ticks + MailboxSpaceDiscovery.databaseSizeRecurrence.Ticks / 2L);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.MailboxSpace.Name, "DatabaseSizeEscalationNotification", null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("DatabaseSizeEscalationProcessingMonitor", sampleMask, ExchangeComponent.MailboxSpace.Name, ExchangeComponent.MailboxSpace, 1, true, (int)timeSpan.TotalSeconds);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, MailboxSpaceDiscovery.databaseSizeRecurrence)
			};
			monitorDefinition.RecurrenceIntervalSeconds = (int)MailboxSpaceDiscovery.databaseSizeRecurrence.TotalSeconds;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate MailboxSpace health is not impacted by database health issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessageUnhealthy;
			if (MailboxSpaceDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.DatabaseSizeEscalationMessageDc(string.Format("Invoke-MonitoringProbe -Identity '{0}\\{1}\\{2}' -Server {3}", new object[]
				{
					MailboxSpaceDiscovery.mailboxSpaceHealthsetName,
					"DatabaseSpaceProbe",
					"{Probe.StateAttribute1}",
					Environment.MachineName
				}), string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.MailboxSpace.Name, "DatabaseSizeMonitor"));
			}
			else
			{
				escalationMessageUnhealthy = Strings.DatabaseSizeEscalationMessageEnt(string.Format("Invoke-MonitoringProbe -Identity '{0}\\{1}\\{2}' -Server {3}", new object[]
				{
					MailboxSpaceDiscovery.mailboxSpaceHealthsetName,
					"DatabaseSpaceProbe",
					"{Probe.StateAttribute1}",
					Environment.MachineName
				}), string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.MailboxSpace.Name, "DatabaseSizeMonitor"));
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("DatabaseSizeEscalate", MailboxSpaceDiscovery.mailboxSpaceHealthsetName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), null, ServiceHealthStatus.Unrecoverable, ExchangeComponent.MailboxSpace.EscalationTeam, Strings.DatabaseSizeEscalationSubject, escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateDatabaseLogicalPhysicalSizeRatioConsolidatedWorkitems()
		{
			TimeSpan duration = new TimeSpan(2L * MailboxSpaceDiscovery.databaseLogicalPhysicalSizeRatioRecurrence.Ticks);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.MailboxSpace.Name, "DatabaseLogicalPhysicalSizeRatioEscalationNotification", null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("DatabaseLogicalPhysicalSizeRatioEscalationProcessingMonitor", sampleMask, ExchangeComponent.MailboxSpace.Name, ExchangeComponent.MailboxSpace, 1, true, (int)MailboxSpaceDiscovery.databaseLogicalPhysicalSizeRatioMonitoringInterval.TotalSeconds);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, MailboxSpaceDiscovery.databaseLogicalPhysicalSizeRatioRecurrence)
			};
			monitorDefinition.RecurrenceIntervalSeconds = (int)MailboxSpaceDiscovery.databaseLogicalPhysicalSizeRatioRecurrence.TotalSeconds;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate MailboxSpace health is not impacted by database health issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessageUnhealthy;
			if (MailboxSpaceDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.DatabaseLogicalPhysicalSizeRatioEscalationMessageDc(0.9, duration, string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.MailboxSpace.Name, "DatabaseLogicalPhysicalSizeRatioMonitor"));
			}
			else
			{
				escalationMessageUnhealthy = Strings.DatabaseLogicalPhysicalSizeRatioEscalationMessageEnt(0.9, duration, string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.MailboxSpace.Name, "DatabaseLogicalPhysicalSizeRatioMonitor"));
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("DatabaseLogicalPhysicalSizeRatioEscalate", ExchangeComponent.MailboxSpace.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), null, ServiceHealthStatus.Unrecoverable, ExchangeComponent.MailboxSpace.EscalationTeam, Strings.DatabaseLogicalPhysicalSizeRatioEscalationSubject(duration), escalationMessageUnhealthy, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition definition = SetMonitorStateRepairingResponder.CreateDefinition("SetDatabaseLogicalPhysicalSizeRatioEscalationProcessingMonitorStateRepairing", ExchangeComponent.MailboxSpace.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), string.Empty, monitorDefinition.Name, ServiceHealthStatus.Unrecoverable, true, 0);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		internal const string DatabaseBufferThresholdString = "DatabaseBufferThreshold";

		internal const string DatabaseLogsThresholdString = "DatabaseLogsThreshold";

		internal const string SearchSizeFactorThresholdString = "SearchSizeFactorThreshold";

		internal const string NumberOfDatabasesPerDiskString = "NumberOfDatabasesPerDisk";

		internal const string DatabaseDriveSpaceTriggerString = "DatabaseDriveSpaceTrigger_Error";

		internal const string DatabaseLogicalPhysicalSizeRatioTriggerString = "DatabaseLogicalPhysicalSizeRatioTrigger_Error";

		internal const double SearchSizeFactorThreshold = 0.2;

		internal const int NumberOfDatabasesPerDisk = 4;

		private const int DriveSpaceMonitoringThreshold = 1;

		private const int DatabaseLogicalPhysicalSizeRatioNumberOfSamplesAboveThreshold = 1;

		private const double DatabaseLogicalPhysicalRatioThreshold = 0.9;

		private const int MaxRetryAttempt = 3;

		internal static ByteQuantifiedSize DatabaseBufferThreshold = ByteQuantifiedSize.FromGB(100UL);

		internal static ByteQuantifiedSize DatabaseLogsThreshold = ByteQuantifiedSize.FromGB(100UL);

		private static TimeSpan databaseSizeRecurrence = TimeSpan.FromMinutes(30.0);

		private static TimeSpan databaseSizeTimeout = TimeSpan.FromMinutes(2.0);

		private static TimeSpan databaseSizeWaitInterval = TimeSpan.FromSeconds(15.0);

		private static TimeSpan databaseLogicalPhysicalSizeRatioRecurrence = TimeSpan.FromMinutes(10.0);

		private static TimeSpan databaseLogicalPhysicalSizeRatioMonitoringInterval = new TimeSpan(MailboxSpaceDiscovery.databaseLogicalPhysicalSizeRatioRecurrence.Ticks + MailboxSpaceDiscovery.databaseLogicalPhysicalSizeRatioRecurrence.Ticks / 2L);

		private static TimeSpan databaseLogicalPhysicalSizeRatioWaitInterval = TimeSpan.FromSeconds(1.0);

		private static bool isTestTopology;

		private static bool isDatacenter;

		private static bool isDatacenterDedicated;

		private static string mailboxSpaceHealthsetName = ExchangeComponent.MailboxSpace.Name;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string DBSpaceProbeTypeName = typeof(DatabaseSpaceProbe).FullName;

		private static readonly string DBSizeMonitorTypeName = typeof(DatabaseSizeMonitor).FullName;

		private static readonly string DatabaseProvisioningResponderTypeName = typeof(DatabaseProvisioningResponder).FullName;

		private static readonly string DatabaseSizeEscalationNotificationResponderTypeName = typeof(DatabaseSizeEscalationNotificationResponder).FullName;

		private static readonly string EscalationNotificationResponderTypeName = typeof(EscalationNotificationResponder).FullName;
	}
}
