using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Responders;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store
{
	public sealed class StoreDiscovery : MaintenanceWorkItem
	{
		internal static void PopulateProbeDefinition(ProbeDefinition probeDefinition, string targetResource, Type probeType, string probeName, TimeSpan recurrenceInterval, TimeSpan timeoutInterval)
		{
			probeDefinition.AssemblyPath = probeType.Assembly.Location;
			probeDefinition.TypeName = probeType.FullName;
			probeDefinition.Name = probeName;
			probeDefinition.ServiceName = ExchangeComponent.Store.Name;
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
					WTFDiagnostics.TraceInformation(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: Mailbox role is not installed on this server, no need to create store related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreDiscovery.cs", 729);
				}
				else
				{
					StoreDiscovery.isDatacenter = LocalEndpointManager.IsDataCenter;
					this.CreateStoreServiceContext();
					this.CreateProcessCrashingContext("M.E.Store.Service");
					this.CreateProcessCrashingContext("M.E.Store.Worker");
					this.CreateStoreAdminRPCInterfaceContext();
					if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: No mailbox database found on this server", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreDiscovery.cs", 749);
					}
					else
					{
						foreach (MailboxDatabaseInfo dbInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
						{
							this.CreateActiveDatabaseAvailabilityContext(dbInfo);
							this.CreatePassiveDatabaseAvailabilityContext(dbInfo);
							this.CreatePercentRPCRequestsContext(dbInfo);
							this.CreateDatabaseRPCLatencyContext(dbInfo);
							this.QuarantinedMailboxContext(dbInfo);
							this.NumberOfActiveBackgroundTasksContext(dbInfo);
							foreach (string resourceType in StoreDiscovery.RequiredMaintenanceResourceTypes)
							{
								this.StoreMaintenanceAssistantContext(dbInfo, resourceType);
							}
							this.DatabaseRepeatedMountsContext(dbInfo);
							this.VersionBucketsAllocatedContext(dbInfo);
							this.DatabaseDiskReadLatencyContext(dbInfo);
							if (StoreDiscovery.isDatacenter)
							{
								this.DatabaseSchemaVersionCheckContext(dbInfo);
							}
						}
						this.CreateActiveDatabaseAvailabilityEscalateWorkitems();
						this.CreatePassiveDatabaseAvailabilityEscalateWorkitems();
						this.CreateStoreMaintenanceAssistantConsolidatedWorkitems();
						this.CreateVersionBucketsAllocatedEscalateWorkitems();
						this.CreateDatabaseDiskReadLatencyEscalateWorkitems();
						if (StoreDiscovery.isDatacenter)
						{
							this.CreateDatabaseSchemaVersionCheckEscalateWorkitems();
						}
					}
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: EndpointManagerEndpointUninitializedException is caught.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreDiscovery.cs", 819);
			}
		}

		private void CreateStoreServiceContext()
		{
			ProbeDefinition probeDefinition = this.CreateProbe("MSExchangeIS", StoreDiscovery.StoreServiceProbeType, "StoreServiceProbe", StoreDiscovery.storeServiceRecurrence);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitor("MSExchangeIS", StoreDiscovery.OverallXFailuresMonitorType, "StoreServiceMonitor", StoreDiscovery.storeServiceRecurrence, StoreDiscovery.storeServiceMonitoringInterval, new int?(2), probeDefinition.ConstructWorkItemResultName());
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, StoreDiscovery.storeServiceStartTimeout + StoreDiscovery.storeServiceRecurrence),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.storeServiceEscalationInterval)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition("StoreServiceRestart", monitorDefinition.Name, "MSExchangeIS", ServiceHealthStatus.Degraded, 15, (int)StoreDiscovery.storeServiceStartTimeout.TotalSeconds, 0, false, DumpMode.None, null, 15.0, 0, ExchangeComponent.Store.Name, null, true, true, "Dag", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			responderDefinition.AlertMask = monitorDefinition.ConstructWorkItemResultName();
			responderDefinition.AlertTypeId = monitorDefinition.Name;
			responderDefinition.TargetResource = "MSExchangeIS";
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = DagForceRebootServerResponder.CreateDefinition("StoreServiceKillServer", ExchangeComponent.Store.Name, monitorDefinition.Name, ServiceHealthStatus.Unhealthy);
			responderDefinition2.TargetResource = "MSExchangeIS";
			responderDefinition2.AlertMask = monitorDefinition.ConstructWorkItemResultName();
			responderDefinition2.AlertTypeId = monitorDefinition.Name;
			responderDefinition2.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			string escalationMessageUnhealthy;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.ServiceNotRunningEscalationMessageDc("MSExchangeIS");
			}
			else
			{
				escalationMessageUnhealthy = Strings.ServiceNotRunningEscalationMessageEnt("MSExchangeIS");
			}
			ResponderDefinition responderDefinition3 = EscalateResponder.CreateDefinition("StoreServiceEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), "MSExchangeIS", ServiceHealthStatus.Unrecoverable, "Store", Strings.ServiceNotRunningEscalationSubject("MSExchangeIS"), escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition3.RecurrenceIntervalSeconds = 0;
			responderDefinition3.TimeoutSeconds = (int)StoreDiscovery.storeServiceRecurrence.TotalSeconds;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition3, base.TraceContext);
		}

		private void CreateStoreAdminRPCInterfaceContext()
		{
			ProbeDefinition probeDefinition = this.CreateProbe("MSExchangeIS", StoreDiscovery.StoreAdminRPCInterfaceProbeType, "StoreAdminRPCInterfaceProbe", StoreDiscovery.storeAdminRPCInterfaceRecurrence);
			probeDefinition.Attributes["ValidationTimeoutSeconds"] = StoreDiscovery.validationTimeout.ToString();
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitor("MSExchangeIS", StoreDiscovery.OverallXFailuresMonitorType, "StoreAdminRPCInterfaceMonitor", StoreDiscovery.storeAdminRPCInterfaceRecurrence, StoreDiscovery.storeAdminRPCInterfaceMonitoringInterval, new int?(4), probeDefinition.ConstructWorkItemResultName());
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, StoreDiscovery.storeAdminRPCInterfaceServiceStartTimeout + StoreDiscovery.storeAdminRPCInterfaceRecurrence),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.storeAdminRPCInterfaceEscalationInterval)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition("StoreAdminRPCInterfaceRestart", monitorDefinition.Name, "MSExchangeIS", ServiceHealthStatus.Degraded, 15, (int)StoreDiscovery.storeAdminRPCInterfaceServiceStartTimeout.TotalSeconds, 0, false, DumpMode.None, null, 15.0, 0, ExchangeComponent.Store.Name, null, true, true, "Dag", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			responderDefinition.AlertMask = monitorDefinition.ConstructWorkItemResultName();
			responderDefinition.AlertTypeId = monitorDefinition.Name;
			responderDefinition.Enabled = false;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = DagForceRebootServerResponder.CreateDefinition("StoreAdminRPCInterfaceKillServer", ExchangeComponent.Store.Name, monitorDefinition.Name, ServiceHealthStatus.Unhealthy);
			responderDefinition2.TypeName = StoreDiscovery.StoreKillServerByExceptionTypeResponderType.FullName;
			responderDefinition2.AssemblyPath = StoreDiscovery.StoreKillServerByExceptionTypeResponderType.Assembly.Location;
			responderDefinition2.AlertMask = monitorDefinition.ConstructWorkItemResultName();
			responderDefinition2.AlertTypeId = monitorDefinition.Name;
			responderDefinition2.RecurrenceIntervalSeconds = 0;
			responderDefinition2.Enabled = false;
			responderDefinition2.Attributes[StoreKillServerByExceptionTypeResponder.ExceptionTypePropertyKeyString] = "StateAttribute1";
			responderDefinition2.Attributes[typeof(TimeoutException).FullName] = bool.TrueString;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			TimeSpan duration = StoreDiscovery.storeAdminRPCInterfaceEscalationInterval;
			string escalationMessage;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessage = Strings.StoreAdminRPCInterfaceEscalationEscalationMessageDc(duration);
			}
			else
			{
				escalationMessage = Strings.StoreAdminRPCInterfaceEscalationEscalationMessageEnt(duration);
			}
			ResponderDefinition responderDefinition3 = ConditionalEscalateResponder.CreateDefinition("StoreAdminRPCInterfaceEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), "MSExchangeIS", ServiceHealthStatus.Unrecoverable, "Store", Strings.StoreAdminRPCInterfaceEscalationSubject(duration), escalationMessage, "StateAttribute1", NotificationServiceClass.Scheduled, true, 0);
			responderDefinition3.Attributes[typeof(MapiExceptionNetworkError).FullName] = bool.TrueString;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition3, base.TraceContext);
		}

		private void CreateActiveDatabaseAvailabilityContext(MailboxDatabaseInfo dbInfo)
		{
			string mailboxDatabaseName = dbInfo.MailboxDatabaseName;
			ProbeDefinition probeDefinition = this.CreateProbe(mailboxDatabaseName, StoreDiscovery.ActiveDBAvailabilityProbeType, "ActiveDatabaseAvailabilityProbe", StoreDiscovery.activeDBAvailabilityRecurrence, StoreDiscovery.activeDBAvailabilityTimeout);
			probeDefinition.TargetExtension = dbInfo.MailboxDatabaseGuid.ToString();
			probeDefinition.Attributes["SystemMailboxGuid"] = dbInfo.SystemMailboxGuid.ToString();
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitor(mailboxDatabaseName, StoreDiscovery.OverallXFailuresMonitorType, "ActiveDatabaseAvailabilityMonitor", StoreDiscovery.activeDBAvailabilityRecurrence, StoreDiscovery.activeDBAvailabilityMonitoringInterval, new int?(3), probeDefinition.ConstructWorkItemResultName());
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.activeDBAvailabilityEscalationNotificationInterval),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable1, StoreDiscovery.activeDBAvailabilityBackupEscalationNotificationInterval + StoreDiscovery.activeDBAvailabilityEscalationNotificationInterval)
			};
			monitorDefinition.AllowCorrelationToMonitor = true;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by active database availability issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = this.CreateResponder(mailboxDatabaseName, StoreDiscovery.DatabaseAvailabilityEscalationNotificationResponderType, "ActiveDatabaseAvailabilityEscalationNotification", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, StoreDiscovery.activeDBAvailabilityEscalationNotificationWaitInterval, ServiceHealthStatus.Unrecoverable);
			responderDefinition.ActionOnCorrelatedMonitors = CorrelatedMonitorAction.GenerateException;
			responderDefinition.CorrelatedMonitors = new CorrelatedMonitorInfo[]
			{
				new CorrelatedMonitorInfo(string.Format("{0}\\{1}\\*", ExchangeComponent.AD.Name, DirectoryMonitoringStrings.ActiveDirectoryConnectivityConfigDC.MonitorName), StoreMonitoringHelpers.GetRegExMatchFromException(new string[]
				{
					typeof(MapiExceptionMdbOffline).FullName,
					typeof(MapiExceptionADUnavailable).FullName
				}), CorrelatedMonitorInfo.MatchMode.RegEx)
			};
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = this.CreateResponder(mailboxDatabaseName, StoreDiscovery.DatabaseAvailabilityEscalationNotificationResponderType, "ActiveDatabaseAvailabilityEscalationNotification", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, StoreDiscovery.activeDBAvailabilityEscalationNotificationWaitInterval, ServiceHealthStatus.Unrecoverable1);
			responderDefinition2.TargetPartition = "ActiveDatabaseAvailabilityBackup";
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
		}

		private void CreatePassiveDatabaseAvailabilityContext(MailboxDatabaseInfo dbInfo)
		{
			string mailboxDatabaseName = dbInfo.MailboxDatabaseName;
			ProbeDefinition probeDefinition = this.CreateProbe(mailboxDatabaseName, StoreDiscovery.PassiveDatabaseAvailabilityProbeType, "PassiveDatabaseAvailabilityProbe", StoreDiscovery.passiveDBAvailabilityRecurrence);
			probeDefinition.TargetExtension = dbInfo.MailboxDatabaseGuid.ToString();
			probeDefinition.Attributes["SystemMailboxGuid"] = dbInfo.SystemMailboxGuid.ToString();
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitor(mailboxDatabaseName, StoreDiscovery.OverallXFailuresMonitorType, "PassiveDatabaseAvailabilityMonitor", StoreDiscovery.passiveDBAvailabilityRecurrence, StoreDiscovery.passiveDBAvailabilityMonitoringInterval, new int?(3), probeDefinition.ConstructWorkItemResultName());
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.passiveDBAvailabilityEscalationNotificationInterval)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by passive database availability issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition = this.CreateResponder(mailboxDatabaseName, StoreDiscovery.DatabaseAvailabilityEscalationNotificationResponderType, "PassiveDatabaseAvailabilityEscalationNotification", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, StoreDiscovery.passiveDBAvailabilityEscalationNotificationWaitInterval, ServiceHealthStatus.Unrecoverable);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateProcessCrashingContext(string processName)
		{
			TimeSpan timeSpan;
			int num;
			string probeName;
			string monitorName;
			string name;
			if (string.Equals(processName, "M.E.Store.Service", StringComparison.OrdinalIgnoreCase))
			{
				timeSpan = StoreDiscovery.serviceCrashMonitoringInterval;
				num = 6;
				probeName = "StoreServiceProcessRepeatedlyCrashingProbe";
				monitorName = "StoreServiceProcessRepeatedlyCrashingMonitor";
				name = "StoreServiceProcessRepeatedlyCrashingEscalate";
			}
			else
			{
				if (!string.Equals(processName, "M.E.Store.Worker", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException("Invalid process name for creating active monitoring workitems", processName);
				}
				timeSpan = StoreDiscovery.workerCrashMonitoringInterval;
				num = 10;
				probeName = "StoreWorkerProcessRepeatedlyCrashingProbe";
				monitorName = "StoreWorkerProcessRepeatedlyCrashingMonitor";
				name = "StoreWorkerProcessRepeatedlyCrashingEscalate";
			}
			ProbeDefinition probeDefinition = this.CreateProbe(processName, typeof(GenericProcessCrashDetectionProbe), probeName, StoreDiscovery.processCrashRecurrence);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitor(processName, StoreDiscovery.OverallXFailuresMonitorType, monitorName, StoreDiscovery.processCrashRecurrence, timeSpan, new int?(num), probeDefinition.ConstructWorkItemResultName());
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by process crash issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessageUnhealthy;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.StoreProcessRepeatedlyCrashingEscalationMessageDc(processName, num, timeSpan);
			}
			else
			{
				escalationMessageUnhealthy = Strings.StoreProcessRepeatedlyCrashingEscalationMessageEnt(processName, num, timeSpan);
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), processName, ServiceHealthStatus.Unhealthy, "Store", Strings.ProcessRepeatedlyCrashingEscalationSubject(processName), escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = (int)StoreDiscovery.processCrashRecurrence.TotalSeconds;
			responderDefinition.TimeoutSeconds = (int)StoreDiscovery.processCrashRecurrence.TotalSeconds;
			if (string.Equals(processName, "M.E.Store.Worker", StringComparison.OrdinalIgnoreCase))
			{
				responderDefinition.TypeName = StoreDiscovery.ProcessCrashConditionalEscalationResponderType.FullName;
				responderDefinition.Attributes[ConditionalEscalateResponder.ConditionalPropertyString] = "StateAttribute4";
				responderDefinition.Attributes["S.IO.FileLoadException"] = bool.TrueString;
			}
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreatePercentRPCRequestsContext(MailboxDatabaseInfo dbInfo)
		{
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "StorePercentRpcRequestsTrigger_Error", dbInfo.MailboxDatabaseName);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("DatabasePercentRPCRequestsMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, 1, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.IsHaImpacting = true;
			monitorDefinition.SetHaScope(HaScopeEnum.Database);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, StoreDiscovery.percentRPCRequestsDatabaseFailoverInterval),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.percentRPCRequestsEscalationInterval)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by RPC issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			if (StoreDiscovery.isDatacenter)
			{
				ResponderDefinition responderDefinition = StoreWatsonResponder.CreateDefinition("DatabasePercentRPCRequestsWatsonResponder", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), "Microsoft.Exchange.Store.Worker.exe", dbInfo.MailboxDatabaseGuid.ToString(), ServiceHealthStatus.Degraded, string.Format("{0}Exception", "DatabasePercentRPCRequestsWatsonResponder"), "E12");
				responderDefinition.Enabled = false;
				responderDefinition.RecurrenceIntervalSeconds = 0;
				responderDefinition.Attributes["CopyQueueLengthThreshold"] = 10.ToString();
				responderDefinition.Attributes["ReplayQueueLengthThreshold"] = 100.ToString();
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
			ResponderDefinition responderDefinition2 = DatabaseFailoverResponder.CreateDefinition("DatabasePercentRPCRequestsDatabaseFailover", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, ExchangeComponent.Store.Name, dbInfo.MailboxDatabaseGuid, ServiceHealthStatus.Unhealthy);
			responderDefinition2.RecurrenceIntervalSeconds = (int)StoreDiscovery.percentRPCRequestsRecurrence.TotalSeconds;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			string escalationMessage;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessage = Strings.DatabasePercentRPCRequestsEscalationMessageDc(dbInfo.MailboxDatabaseName, 90, new TimeSpan(StoreDiscovery.percentRPCRequestsRecurrence.Ticks * 2L + StoreDiscovery.percentRPCRequestsEscalationInterval.Ticks));
			}
			else
			{
				escalationMessage = Strings.DatabasePercentRPCRequestsEscalationMessageEnt(dbInfo.MailboxDatabaseName, 90, new TimeSpan(StoreDiscovery.percentRPCRequestsRecurrence.Ticks * 2L + StoreDiscovery.percentRPCRequestsEscalationInterval.Ticks));
			}
			ResponderDefinition responderDefinition3 = EscalateByDatabaseHealthResponder.CreateDefinition("DatabasePercentRPCRequestsEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, dbInfo.MailboxDatabaseGuid.ToString(), ServiceHealthStatus.Unrecoverable, "Store", Strings.DatabasePercentRPCRequestsEscalationSubject(dbInfo.MailboxDatabaseName, 90), escalationMessage, NotificationServiceClass.Urgent, true, (int)StoreDiscovery.percentRPCRequestsRecurrence.TotalSeconds);
			responderDefinition3.Attributes[CopyStatusEnum.Healthy.ToString()] = NotificationServiceClass.Scheduled.ToString();
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition3, base.TraceContext);
		}

		private void CreateDatabaseRPCLatencyContext(MailboxDatabaseInfo dbInfo)
		{
			string edsTriggerMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "StoreRpcAverageLatencyTrigger_Warning", dbInfo.MailboxDatabaseName);
			string edsTriggerMask2 = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "StoreRpcAverageLatencyTrigger_Error", dbInfo.MailboxDatabaseName);
			this.CreateDatabaseRPCLatencyContext(dbInfo, "DatabaseRPCLatencyWarningMonitor", "DatabaseRPCLatencyWarningRootCause", "DatabaseRPCLatencyWarningEscalate", edsTriggerMask, 70, NotificationServiceClass.Scheduled);
			this.CreateDatabaseRPCLatencyContext(dbInfo, "DatabaseRPCLatencyErrorMonitor", "DatabaseRPCLatencyErrorRootCause", "DatabaseRPCLatencyErrorEscalate", edsTriggerMask2, 150, NotificationServiceClass.Urgent);
		}

		private void CreateDatabaseRPCLatencyContext(MailboxDatabaseInfo dbInfo, string monitorName, string latencyresponderName, string escalateResponderName, string edsTriggerMask, int rpcAverageLatencyThreshold, NotificationServiceClass alertNotificationType)
		{
			TimeSpan recurrenceInterval = TimeSpan.FromMinutes(5.0);
			TimeSpan timeoutInterval = TimeSpan.FromMinutes(5.0);
			TimeSpan waitInterval = TimeSpan.FromSeconds(15.0);
			TimeSpan transitionTimeout = TimeSpan.FromMinutes(5.0);
			TimeSpan timeSpan = TimeSpan.FromMinutes(15.0);
			MonitorDefinition monitorDefinition = this.CreateMonitor(dbInfo.MailboxDatabaseName, StoreDiscovery.DatabaseRPCLatencyMonitorType, monitorName, TimeSpan.Zero, timeSpan, new int?(rpcAverageLatencyThreshold), timeoutInterval, edsTriggerMask);
			monitorDefinition.TargetExtension = dbInfo.MailboxDatabaseGuid.ToString();
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, transitionTimeout)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by RPC latency issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = this.CreateResponder(dbInfo.MailboxDatabaseName, StoreDiscovery.DatabaseRPCLatencyResponderType, latencyresponderName, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, recurrenceInterval, waitInterval, timeoutInterval, ServiceHealthStatus.Degraded);
			if (rpcAverageLatencyThreshold == 70)
			{
				responderDefinition.Attributes["RPCAverageLatencyWarningThreshold"] = rpcAverageLatencyThreshold.ToString();
			}
			else
			{
				responderDefinition.Attributes["RPCAverageLatencyErrorThreshold"] = rpcAverageLatencyThreshold.ToString();
			}
			responderDefinition.Attributes["AverageTimeInServerThreshold"] = 60000.ToString();
			responderDefinition.Attributes["RopLatencyThreshold"] = 15.ToString();
			responderDefinition.Attributes["PercentSampleBelowThresholdToAlert"] = 80.ToString();
			responderDefinition.Attributes["MinimumStoreUsageStatisticsSampleCount"] = 125.ToString();
			responderDefinition.TargetExtension = dbInfo.MailboxDatabaseGuid.ToString();
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			string escalationMessage;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessage = Strings.DatabaseRPCLatencyEscalationMessageDc(dbInfo.MailboxDatabaseName, rpcAverageLatencyThreshold, timeSpan);
			}
			else
			{
				escalationMessage = Strings.DatabaseRPCLatencyEscalationMessageEnt(dbInfo.MailboxDatabaseName, rpcAverageLatencyThreshold, timeSpan);
			}
			ResponderDefinition responderDefinition2 = AlertNotificationTypeByDatabaseCopyStateResponder.CreateDefinition(escalateResponderName, ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, dbInfo.MailboxDatabaseGuid.ToString(), ServiceHealthStatus.Unrecoverable, "Store", Strings.DatabaseRPCLatencyEscalationSubject(dbInfo.MailboxDatabaseName, rpcAverageLatencyThreshold, timeSpan), escalationMessage, alertNotificationType, recurrenceInterval, true);
			responderDefinition2.ActionOnCorrelatedMonitors = CorrelatedMonitorAction.GenerateException;
			responderDefinition2.CorrelatedMonitors = new CorrelatedMonitorInfo[]
			{
				new CorrelatedMonitorInfo(string.Format("{0}\\{1}\\{2}", ExchangeComponent.Store.Name, "DatabaseDiskReadLatencyMonitor", dbInfo.MailboxDatabaseName), null, CorrelatedMonitorInfo.MatchMode.Wildcard)
			};
			responderDefinition2.Attributes["EscalationNotificationTypeForPassive"] = NotificationServiceClass.Scheduled.ToString();
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
		}

		private void QuarantinedMailboxContext(MailboxDatabaseInfo dbInfo)
		{
			int failureCount = 1;
			TimeSpan timeSpan = TimeSpan.FromMinutes(15.0);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "StoreQuarantinedMailboxCountTrigger_Error", dbInfo.MailboxDatabaseName);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("MailboxQuarantinedMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, failureCount, true, (int)timeSpan.TotalSeconds);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by quarantined mailboxes issue";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessage;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessage = Strings.QuarantinedMailboxEscalationMessageDc(dbInfo.MailboxDatabaseName);
			}
			else
			{
				escalationMessage = Strings.QuarantinedMailboxEscalationMessageEnt(dbInfo.MailboxDatabaseName);
			}
			ResponderDefinition responderDefinition = this.CreateResponder(dbInfo.MailboxDatabaseName, StoreDiscovery.MailboxQuarantinedEscalateResponderType, "MailboxQuarantinedEscalate", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, TimeSpan.FromSeconds(15.0), ServiceHealthStatus.Unhealthy);
			responderDefinition.TargetExtension = dbInfo.MailboxDatabaseGuid.ToString();
			responderDefinition.Attributes["SuppressQuarantineAlertDuration"] = StoreDiscovery.suppressQuarantineAlertDuration.TotalSeconds.ToString();
			responderDefinition.NotificationServiceClass = NotificationServiceClass.Scheduled;
			responderDefinition.EscalationTeam = "Store";
			responderDefinition.EscalationSubject = Strings.QuarantinedMailboxEscalationSubject(dbInfo.MailboxDatabaseName);
			responderDefinition.EscalationMessage = escalationMessage;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition definition = SetMonitorStateRepairingResponder.CreateDefinition("SetQuarantinedMailboxMonitorStateRepairing", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, monitorDefinition.Name, ServiceHealthStatus.Unhealthy, true, 0);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void NumberOfActiveBackgroundTasksContext(MailboxDatabaseInfo dbInfo)
		{
			int failureCount = 1;
			TimeSpan duration = TimeSpan.FromMinutes(15.0);
			TimeSpan timeSpan = TimeSpan.FromMinutes(20.0);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "StoreNumberOfActiveBackgroundTasksTrigger_Error", dbInfo.MailboxDatabaseName);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("NumberOfActiveBackgroundTasksMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, failureCount, true, (int)timeSpan.TotalSeconds);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.SecondaryMonitoringThreshold = 15.0;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Valdiate Store health is not impacted by active background tasks issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessageUnhealthy;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.NumberOfActiveBackgroundTasksEscalationMessageDc(dbInfo.MailboxDatabaseName, 15, duration);
			}
			else
			{
				escalationMessageUnhealthy = Strings.NumberOfActiveBackgroundTasksEscalationMessageEnt(dbInfo.MailboxDatabaseName, 15, duration);
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("NumberOfActiveBackgroundTasksEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, ServiceHealthStatus.Unhealthy, "Store", Strings.NumberOfActiveBackgroundTasksEscalationSubject(dbInfo.MailboxDatabaseName, 15, duration), escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void StoreMaintenanceAssistantContext(MailboxDatabaseInfo dbInfo, string resourceType)
		{
			string notificationMask = NotificationItem.GenerateResultName(ExchangeComponent.Store.Name, string.Format("{0}.{1}", "StoreMaintenanceHandler", resourceType), dbInfo.MailboxDatabaseName);
			MonitorDefinition monitorDefinition = NotificationHeartbeatMonitor.CreateDefinition(string.Format("{0}{1}", resourceType, "MaintenanceAssistantMonitor"), ExchangeComponent.Store.Name, ExchangeComponent.Store, notificationMask, (int)StoreDiscovery.maintenanceAssistantRecurrence.TotalSeconds, (int)StoreDiscovery.maintenanceAssistantHeartbeatSLA.TotalSeconds, true);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.TargetExtension = dbInfo.MailboxDatabaseGuid.ToString();
			monitorDefinition.TypeName = StoreDiscovery.StoreMaintenanceAssistantMonitorType.FullName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.maintenanceAssistantEscalationNotificationInterval)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by maintenance assistance issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition("MaintenanceAssistantRestart", monitorDefinition.Name, "MSExchangeMailboxAssistants", ServiceHealthStatus.Unhealthy, 15, (int)StoreDiscovery.maintenanceAssistantServiceStartTimeout.TotalSeconds, 0, false, DumpMode.None, null, 15.0, 0, ExchangeComponent.Store.Name, null, true, true, "Dag", false);
			responderDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			responderDefinition.TargetPartition = resourceType;
			responderDefinition.AlertMask = monitorDefinition.ConstructWorkItemResultName();
			responderDefinition.AlertTypeId = monitorDefinition.Name;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = this.CreateResponder(dbInfo.MailboxDatabaseName, StoreDiscovery.EscalationNotificationResponderType, "MaintenanceAssistantEscalationNotification", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, StoreDiscovery.maintenanceAssistantEscalationNotificationWaitInterval, ServiceHealthStatus.Unrecoverable);
			responderDefinition2.TargetPartition = resourceType;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
		}

		private void DatabaseRepeatedMountsContext(MailboxDatabaseInfo dbInfo)
		{
			int value = 13;
			TimeSpan recurrenceInterval = TimeSpan.FromMinutes(5.0);
			TimeSpan timeSpan = TimeSpan.FromHours(1.5);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Store.Name, "DatabaseMountingTrigger", dbInfo.MailboxDatabaseName);
			MonitorDefinition monitorDefinition = this.CreateMonitor(dbInfo.MailboxDatabaseName, StoreDiscovery.DatabaseRepeatedMountsMonitorType, "DatabaseRepeatedMountsMonitor", recurrenceInterval, timeSpan, new int?(value), sampleMask);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.SecondaryMonitoringThreshold = (double)((int)TimeSpan.FromSeconds(60.0).TotalSeconds);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by database mounts and unmount issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("DatabaseRepeatedMountsEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, ServiceHealthStatus.Unhealthy, ExchangeComponent.DataProtection.EscalationTeam, Strings.DatabaseRepeatedMountsEscalationSubject(dbInfo.MailboxDatabaseName, timeSpan), Strings.DatabaseRepeatedMountsEscalationMessage(dbInfo.MailboxDatabaseName, timeSpan), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void VersionBucketsAllocatedContext(MailboxDatabaseInfo dbInfo)
		{
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "StoreVersionBucketsAllocatedTrigger_Error", string.Format("information store - {0}/_total", dbInfo.MailboxDatabaseName));
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("VersionBucketsAllocatedMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, 1, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, StoreDiscovery.versionBucketsAllocatedEscalationNotificationInterval)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by version buckets issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			if (StoreDiscovery.isDatacenter)
			{
				ResponderDefinition responderDefinition = StoreWatsonResponder.CreateDefinition("VersionBucketsAllocatedWatsonResponder", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), "Microsoft.Exchange.Store.Worker.exe", dbInfo.MailboxDatabaseGuid.ToString(), ServiceHealthStatus.Degraded, string.Format("{0}Exception", "VersionBucketsAllocatedWatsonResponder"), "E12");
				responderDefinition.RecurrenceIntervalSeconds = 0;
				responderDefinition.Attributes["CopyQueueLengthThreshold"] = 10.ToString();
				responderDefinition.Attributes["ReplayQueueLengthThreshold"] = 100.ToString();
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
			ResponderDefinition definition = this.CreateResponder(dbInfo.MailboxDatabaseName, StoreDiscovery.EscalationNotificationResponderType, "VersionBucketsAllocatedEscalationNotification", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, StoreDiscovery.versionBucketsAllocatedEscalationNotificationWaitInterval, ServiceHealthStatus.Unhealthy);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void DatabaseDiskReadLatencyContext(MailboxDatabaseInfo dbInfo)
		{
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "DatabaseDiskReadLatencyTrigger_Error", string.Format("information store - {0}/_Total", dbInfo.MailboxDatabaseName));
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("DatabaseDiskReadLatencyMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, 1, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.MonitoringIntervalSeconds = (int)StoreDiscovery.databaseDiskReadLatencyRecurrence.TotalSeconds;
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.IsHaImpacting = true;
			monitorDefinition.SetHaScope(HaScopeEnum.Database);
			monitorDefinition.AllowCorrelationToMonitor = true;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by database disk read latency";
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, new TimeSpan(StoreDiscovery.databaseDiskReadLatencyRecurrence.Ticks * 3L)),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.databaseDiskReadLatencyEscalationInterval)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			if (StoreDiscovery.isDatacenter)
			{
				ResponderDefinition responderDefinition = GenericEventWriteResponder.CreateDefinition("DatabaseDiskReadLatencyRepairboxEventLoggingResponder", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, ServiceHealthStatus.Unhealthy, "Microsoft-Exchange-HighAvailability/Monitoring", "Microsoft-Exchange-HighAvailability", 389L, string.Format("{0},HighDatabaseDiskReadLatency", dbInfo.MailboxDatabaseName), EventLogEntryType.Error, 30, null);
				responderDefinition.RecurrenceIntervalSeconds = 0;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
			ResponderDefinition responderDefinition2 = DatabaseFailoverResponder.CreateDefinition("DatabaseDiskReadLatencyDatabaseFailoverResponder", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, ExchangeComponent.Store.Name, dbInfo.MailboxDatabaseGuid, ServiceHealthStatus.Unhealthy);
			responderDefinition2.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			ResponderDefinition definition = this.CreateResponder(dbInfo.MailboxDatabaseName, StoreDiscovery.EscalationNotificationResponderType, "DatabaseDiskReadLatencyEscalationNotification", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, StoreDiscovery.databaseDiskReadLatencyWaitInterval, ServiceHealthStatus.Unrecoverable);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void DatabaseSchemaVersionCheckContext(MailboxDatabaseInfo dbInfo)
		{
			ProbeDefinition probeDefinition = this.CreateProbe(dbInfo.MailboxDatabaseName, StoreDiscovery.DatabaseSchemaVersionCheckProbeType, "DatabaseSchemaVersionCheckProbe", StoreDiscovery.databaseSchemaVersionCheckRecurrence, StoreDiscovery.databaseSchemaVersionCheckTimeoutInterval);
			probeDefinition.TargetExtension = dbInfo.MailboxDatabaseGuid.ToString();
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitor(dbInfo.MailboxDatabaseName, StoreDiscovery.DatabaseSchemaVersionCheckMonitorType, "DatabaseSchemaVersionCheckMonitor", TimeSpan.Zero, StoreDiscovery.databaseSchemaVersionCheckMonitoringInterval, null, StoreDiscovery.databaseSchemaVersionCheckTimeoutInterval, probeDefinition.ConstructWorkItemResultName());
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.databaseSchemaVersionCheckEscalationNotificationInterval)
			};
			monitorDefinition.Attributes["MinimumRequiredDatabaseSchemaVersion"] = 126.ToString();
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by schema version";
			if (LocalEndpointManager.IsDataCenterDedicated)
			{
				monitorDefinition.Enabled = false;
			}
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = this.CreateResponder(dbInfo.MailboxDatabaseName, StoreDiscovery.DatabaseProvisioningResponderType, "DatabaseSchemaVersionCheckProvisioning", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, StoreDiscovery.databaseSchemaVersionCheckWaitInterval, ServiceHealthStatus.Unhealthy);
			responderDefinition.TargetExtension = dbInfo.MailboxDatabaseGuid.ToString();
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = this.CreateResponder(dbInfo.MailboxDatabaseName, StoreDiscovery.DatabaseSchemaVersionCheckEscalationNotificationResponderType, "DatabaseSchemaVersionCheckEscalationNotification", monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.Name, TimeSpan.Zero, StoreDiscovery.databaseSchemaVersionCheckWaitInterval, ServiceHealthStatus.Unrecoverable);
			responderDefinition2.ActionOnCorrelatedMonitors = CorrelatedMonitorAction.GenerateException;
			responderDefinition2.CorrelatedMonitors = new CorrelatedMonitorInfo[]
			{
				StoreMonitoringHelpers.GetStoreCorrelation(dbInfo.MailboxDatabaseName)
			};
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
		}

		private ProbeDefinition CreateProbe(string targetResource, Type probeType, string probeName, TimeSpan recurrenceInterval)
		{
			return this.CreateProbe(targetResource, probeType, probeName, recurrenceInterval, TimeSpan.FromMinutes(1.0));
		}

		private ProbeDefinition CreateProbe(string targetResource, Type probeType, string probeName, TimeSpan recurrenceInterval, TimeSpan timeoutInterval)
		{
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: Creating {0} for {1}", probeName, targetResource, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreDiscovery.cs", 2248);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			StoreDiscovery.PopulateProbeDefinition(probeDefinition, targetResource, probeType, probeName, recurrenceInterval, timeoutInterval);
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: Created {0} for {1}", probeName, targetResource, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreDiscovery.cs", 2257);
			return probeDefinition;
		}

		private MonitorDefinition CreateMonitor(string targetResource, Type monitorType, string monitorName, TimeSpan recurrenceInterval, TimeSpan monitoringInterval, int? monitoringThreshold, string sampleMask)
		{
			return this.CreateMonitor(targetResource, monitorType, monitorName, recurrenceInterval, monitoringInterval, monitoringThreshold, new TimeSpan(recurrenceInterval.Ticks / 2L), sampleMask);
		}

		private MonitorDefinition CreateMonitor(string targetResource, Type monitorType, string monitorName, TimeSpan recurrenceInterval, TimeSpan monitoringInterval, int? monitoringThreshold, TimeSpan timeoutInterval, string sampleMask)
		{
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: Creating {0} for {1}", monitorName, targetResource, null, "CreateMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreDiscovery.cs", 2320);
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = monitorType.Assembly.Location;
			monitorDefinition.TypeName = monitorType.FullName;
			monitorDefinition.Name = monitorName;
			monitorDefinition.ServiceName = ExchangeComponent.Store.Name;
			monitorDefinition.RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			monitorDefinition.TimeoutSeconds = (int)timeoutInterval.TotalSeconds;
			monitorDefinition.MaxRetryAttempts = 3;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.MonitoringIntervalSeconds = (int)monitoringInterval.TotalSeconds;
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.Component = ExchangeComponent.Store;
			if (monitoringThreshold != null)
			{
				monitorDefinition.MonitoringThreshold = (double)monitoringThreshold.Value;
			}
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: Created {0} for {1}", monitorName, targetResource, null, "CreateMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreDiscovery.cs", 2345);
			return monitorDefinition;
		}

		private ResponderDefinition CreateResponder(string targetResource, Type responderType, string responderName, string alertMask, string alertTypeId, TimeSpan recurrenceInterval, TimeSpan waitIntervalSeconds, ServiceHealthStatus targetHealthState)
		{
			return this.CreateResponder(targetResource, responderType, responderName, alertMask, alertTypeId, recurrenceInterval, waitIntervalSeconds, TimeSpan.FromSeconds(60.0), targetHealthState);
		}

		private ResponderDefinition CreateResponder(string targetResource, Type responderType, string responderName, string alertMask, string alertTypeId, TimeSpan recurrenceInterval, TimeSpan waitInterval, TimeSpan timeoutInterval, ServiceHealthStatus targetHealthState)
		{
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: Creating {0} for {1}", responderName, targetResource, null, "CreateResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreDiscovery.cs", 2413);
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = responderType.Assembly.Location;
			responderDefinition.TypeName = responderType.FullName;
			responderDefinition.Name = responderName;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds;
			responderDefinition.TimeoutSeconds = (int)timeoutInterval.TotalSeconds;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.ServiceName = ExchangeComponent.Store.Name;
			responderDefinition.WaitIntervalSeconds = (int)waitInterval.TotalSeconds;
			responderDefinition.TargetHealthState = targetHealthState;
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.StoreTracer, base.TraceContext, "StoreDiscovery.DoWork: Created {0} for {1}", responderName, targetResource, null, "CreateResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreDiscovery.cs", 2435);
			return responderDefinition;
		}

		private void CreateActiveDatabaseAvailabilityEscalateWorkitems()
		{
			TimeSpan timeSpan = new TimeSpan(StoreDiscovery.activeDBAvailabilityRecurrence.Ticks + StoreDiscovery.activeDBAvailabilityRecurrence.Ticks / 2L);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Store.Name, "ActiveDatabaseAvailabilityEscalationNotification", null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("ActiveDatabaseAvailabilityEscalationProcessingMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, 1, true, (int)timeSpan.TotalSeconds);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, new TimeSpan(2L * StoreDiscovery.activeDBAvailabilityRecurrence.Ticks))
			};
			monitorDefinition.RecurrenceIntervalSeconds = (int)StoreDiscovery.activeDBAvailabilityRecurrence.TotalSeconds;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by escalation causing issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = DagForceRebootServerResponder.CreateDefinition("ActiveDatabaseAvailabilityKillServer", ExchangeComponent.Store.Name, monitorDefinition.Name, ServiceHealthStatus.Unhealthy);
			responderDefinition.TypeName = StoreDiscovery.StoreKillServerByExceptionTypeResponderType.FullName;
			responderDefinition.AssemblyPath = StoreDiscovery.StoreKillServerByExceptionTypeResponderType.Assembly.Location;
			responderDefinition.AlertMask = monitorDefinition.ConstructWorkItemResultName();
			responderDefinition.AlertTypeId = monitorDefinition.Name;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			responderDefinition.Attributes[StoreKillServerByExceptionTypeResponder.ExceptionTypePropertyKeyString] = "StateAttribute4";
			responderDefinition.Attributes[typeof(TimeoutException).FullName] = bool.FalseString;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			string escalationMessageUnhealthy;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.ActiveDatabaseAvailabilityEscalationMessageDc(string.Format("Invoke-MonitoringProbe -Identity '{0}\\{1}\\{2}' -Server {3}", new object[]
				{
					ExchangeComponent.Store.Name,
					"ActiveDatabaseAvailabilityProbe",
					"{Probe.StateAttribute1}",
					Environment.MachineName
				}), string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "ActiveDatabaseAvailabilityMonitor"));
			}
			else
			{
				escalationMessageUnhealthy = Strings.ActiveDatabaseAvailabilityEscalationMessageEnt(string.Format("Invoke-MonitoringProbe -Identity '{0}\\{1}\\{2}' -Server {3}", new object[]
				{
					ExchangeComponent.Store.Name,
					"ActiveDatabaseAvailabilityProbe",
					"{Probe.StateAttribute1}",
					Environment.MachineName
				}), string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "ActiveDatabaseAvailabilityMonitor"));
			}
			ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition("ActiveDatabaseAvailabilityEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), null, ServiceHealthStatus.Unrecoverable, "Store", Strings.ActiveDatabaseAvailabilityEscalationSubject, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition2.TypeName = StoreDiscovery.DBAvailabilityEscalateResponderType.FullName;
			responderDefinition2.RecurrenceIntervalSeconds = 0;
			responderDefinition2.Attributes[typeof(MapiExceptionMdbOffline).FullName] = ExchangeComponent.DataProtection.EscalationTeam;
			responderDefinition2.Attributes[typeof(MapiExceptionMailboxInTransit).FullName] = ExchangeComponent.DataProtection.EscalationTeam;
			responderDefinition2.Attributes[typeof(UnableToFindServerForDatabaseException).FullName] = ExchangeComponent.DataProtection.EscalationTeam;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
		}

		private void CreatePassiveDatabaseAvailabilityEscalateWorkitems()
		{
			TimeSpan timeSpan = new TimeSpan(StoreDiscovery.passiveDBAvailabilityRecurrence.Ticks + StoreDiscovery.passiveDBAvailabilityRecurrence.Ticks / 2L);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Store.Name, "PassiveDatabaseAvailabilityEscalationNotification", null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("PassiveDatabaseAvailabilityEscalationProcessingMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, 1, true, (int)timeSpan.TotalSeconds);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.passiveDBAvailabilityRecurrence)
			};
			monitorDefinition.RecurrenceIntervalSeconds = (int)StoreDiscovery.passiveDBAvailabilityRecurrence.TotalSeconds;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by escalation causing issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessageUnhealthy;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.PassiveDatabaseAvailabilityEscalationMessageDc(string.Format("Invoke-MonitoringProbe -Identity '{0}\\{1}\\{2}' -Server {3}", new object[]
				{
					ExchangeComponent.Store.Name,
					"PassiveDatabaseAvailabilityProbe",
					"{Probe.StateAttribute1}",
					Environment.MachineName
				}), string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "PassiveDatabaseAvailabilityMonitor"));
			}
			else
			{
				escalationMessageUnhealthy = Strings.PassiveDatabaseAvailabilityEscalationMessageEnt(string.Format("Invoke-MonitoringProbe -Identity '{0}\\{1}\\{2}' -Server {3}", new object[]
				{
					ExchangeComponent.Store.Name,
					"PassiveDatabaseAvailabilityProbe",
					"{Probe.StateAttribute1}",
					Environment.MachineName
				}), string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "PassiveDatabaseAvailabilityMonitor"));
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("PassiveDatabaseAvailabilityEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), null, ServiceHealthStatus.Unrecoverable, "Store", Strings.PassiveDatabaseAvailabilityEscalationSubject, escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.TypeName = StoreDiscovery.DBAvailabilityEscalateResponderType.FullName;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateStoreMaintenanceAssistantConsolidatedWorkitems()
		{
			TimeSpan timeSpan = new TimeSpan(StoreDiscovery.maintenanceAssistantRecurrence.Ticks + StoreDiscovery.maintenanceAssistantRecurrence.Ticks / 2L);
			TimeSpan duration = StoreDiscovery.maintenanceAssistantHeartbeatSLA + StoreDiscovery.maintenanceAssistantEscalationNotificationInterval;
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Store.Name, "MaintenanceAssistantEscalationNotification", null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("MaintenanceAssistantEscalationProcessingMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, 1, true, (int)timeSpan.TotalSeconds);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.maintenanceAssistantRecurrence)
			};
			monitorDefinition.RecurrenceIntervalSeconds = (int)StoreDiscovery.maintenanceAssistantRecurrence.TotalSeconds;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by maintenance assistant issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessageUnhealthy;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.StoreMaintenanceAssistantEscalationMessageDc(duration, string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "MaintenanceAssistantMonitor"));
			}
			else
			{
				escalationMessageUnhealthy = Strings.StoreMaintenanceAssistantEscalationMessageEnt(duration, string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "MaintenanceAssistantMonitor"));
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("MaintenanceAssistantEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), null, ServiceHealthStatus.Unrecoverable, "Store", Strings.StoreMaintenanceAssistantEscalationSubject(duration), escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateVersionBucketsAllocatedEscalateWorkitems()
		{
			TimeSpan timeSpan = new TimeSpan(StoreDiscovery.versionBucketsAllocatedRecurrence.Ticks + StoreDiscovery.versionBucketsAllocatedRecurrence.Ticks / 2L);
			TimeSpan duration = new TimeSpan(StoreDiscovery.versionBucketsAllocatedRecurrence.Ticks * 2L);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Store.Name, "VersionBucketsAllocatedEscalationNotification", null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("VersionBucketsAllocatedEscalationProcessingMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, 1, true, (int)timeSpan.TotalSeconds);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.versionBucketsAllocatedRecurrence)
			};
			monitorDefinition.RecurrenceIntervalSeconds = (int)StoreDiscovery.versionBucketsAllocatedRecurrence.TotalSeconds;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by escalations causing issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessageUnhealthy;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.VersionBucketsAllocatedEscalationEscalationMessageDc(duration, string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "VersionBucketsAllocatedMonitor"));
			}
			else
			{
				escalationMessageUnhealthy = Strings.VersionBucketsAllocatedEscalationEscalationMessageEnt(duration, string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "VersionBucketsAllocatedMonitor"));
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("VersionBucketsAllocatedEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), null, ServiceHealthStatus.Unrecoverable, "Store", Strings.VersionBucketsAllocatedEscalationSubject(duration), escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateDatabaseDiskReadLatencyEscalateWorkitems()
		{
			TimeSpan duration = new TimeSpan(StoreDiscovery.databaseDiskReadLatencyRecurrence.Ticks * 2L + StoreDiscovery.databaseDiskReadLatencyEscalationInterval.Ticks);
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Store.Name, "DatabaseDiskReadLatencyEscalationNotification", null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("DatabaseDiskReadLatencyEscalationProcessingMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, 1, true, (int)StoreDiscovery.databaseDiskReadLatencyMonitoringInterval.TotalSeconds);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.databaseDiskReadLatencyRecurrence)
			};
			monitorDefinition.RecurrenceIntervalSeconds = (int)StoreDiscovery.databaseDiskReadLatencyRecurrence.TotalSeconds;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by database disk read latency";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessageUnhealthy;
			if (StoreDiscovery.isDatacenter)
			{
				escalationMessageUnhealthy = Strings.DatabaseDiskReadLatencyEscalationMessageDc(duration, string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "DatabaseDiskReadLatencyMonitor"));
			}
			else
			{
				escalationMessageUnhealthy = Strings.DatabaseDiskReadLatencyEscalationMessageEnt(duration, string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "DatabaseDiskReadLatencyMonitor"));
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("DatabaseDiskReadLatencyEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), null, ServiceHealthStatus.Unrecoverable, ExchangeComponent.DataProtection.EscalationTeam, Strings.DatabaseDiskReadLatencyEscalationSubject(duration), escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateDatabaseSchemaVersionCheckEscalateWorkitems()
		{
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Store.Name, "DatabaseSchemaVersionCheckEscalationNotification", null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("DatabaseSchemaVersionCheckEscalationProcessingMonitor", sampleMask, ExchangeComponent.Store.Name, ExchangeComponent.Store, 1, true, (int)StoreDiscovery.databaseSchemaVersionCheckMonitoringInterval.TotalSeconds);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, StoreDiscovery.databaseSchemaVersionCheckRecurrence)
			};
			monitorDefinition.RecurrenceIntervalSeconds = (int)StoreDiscovery.databaseSchemaVersionCheckRecurrence.TotalSeconds;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Store health is not impacted by schema version";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessageUnhealthy = Strings.DatabaseSchemaVersionCheckEscalationMessageDc(string.Format("Invoke-MonitoringProbe -Identity '{0}\\{1}\\{2}' -Server {3}", new object[]
			{
				ExchangeComponent.Store.Name,
				"DatabaseSchemaVersionCheckProbe",
				"{Probe.StateAttribute1}",
				Environment.MachineName
			}), string.Format("Get-ServerHealth -Identity '{0}' -HealthSet '{1}' | ?{{$_.Name -match '{2}' -and $_.AlertValue -ne 'Healthy'}}", Environment.MachineName, ExchangeComponent.Store.Name, "DatabaseSchemaVersionCheckMonitor"));
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("DatabaseSchemaVersionCheckEscalate", ExchangeComponent.Store.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), null, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Store.EscalationTeam, Strings.DatabaseSchemaVersionCheckEscalationSubject, escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		internal const string StoreServiceName = "MSExchangeIS";

		internal const string StoreServiceProcessName = "M.E.Store.Service";

		internal const string StoreWorkerProcessName = "M.E.Store.Worker";

		internal const string SystemMailboxGuidString = "SystemMailboxGuid";

		internal const string RPCAverageLatencyWarningThresholdString = "RPCAverageLatencyWarningThreshold";

		internal const string RPCAverageLatencyErrorThresholdString = "RPCAverageLatencyErrorThreshold";

		internal const string AverageTimeInServerThresholdString = "AverageTimeInServerThreshold";

		internal const string RopLatencyThresholdString = "RopLatencyThreshold";

		internal const string PercentSampleBelowThresholdToAlertString = "PercentSampleBelowThresholdToAlert";

		internal const string MinimumStoreUsageStatisticsSampleCountString = "MinimumStoreUsageStatisticsSampleCount";

		internal const string StoreRpcAverageLatencyWarningTriggerString = "StoreRpcAverageLatencyTrigger_Warning";

		internal const string StoreRpcAverageLatencyErrorTriggerString = "StoreRpcAverageLatencyTrigger_Error";

		internal const string StorePercentRpcRequestsTriggerString = "StorePercentRpcRequestsTrigger_Error";

		internal const string StoreQuarantinedMailboxCountTriggerString = "StoreQuarantinedMailboxCountTrigger_Error";

		internal const string DatabaseDiskReadLatencyTriggerString = "DatabaseDiskReadLatencyTrigger_Error";

		internal const string SuppressQuarantineAlertDurationString = "SuppressQuarantineAlertDuration";

		internal const string ValidationTimeoutSecondsString = "ValidationTimeoutSeconds";

		internal const string MinimumRequiredDatabaseSchemaVersionString = "MinimumRequiredDatabaseSchemaVersion";

		internal const string MaintenanceAssistantTriggerString = "StoreMaintenanceHandler";

		internal const string DatabaseMountingTriggerString = "DatabaseMountingTrigger";

		internal const string StoreNumberOfActiveBackgroundTasksTriggerString = "StoreNumberOfActiveBackgroundTasksTrigger_Error";

		internal const string StoreVersionBucketsAllocatedTriggerString = "StoreVersionBucketsAllocatedTrigger_Error";

		internal const string DatabaseDiskLatencyTriggerString = "DatabaseDiskLatencyTrigger_Error";

		internal const string EscalationSuppressString = "Suppress";

		internal const string EscalationTypeString = "EscalationType";

		internal const string CopyQueueLengthThresholdString = "CopyQueueLengthThreshold";

		internal const string ReplayQueueLengthThresholdString = "ReplayQueueLengthThreshold";

		internal const string EscalationNotificationTypeForPassiveString = "EscalationNotificationTypeForPassive";

		private const int PercentRPCRequestsThreshold = 90;

		private const int PercentRPCRequestsMonitoringThreshold = 1;

		internal const int RPCAverageLatencyWarningThreshold = 70;

		internal const int RPCAverageLatencyErrorThreshold = 150;

		private const int RPCOperationsPerSecThreshold = 50;

		private const int AverageTimeInServerThreshold = 60000;

		private const int RopLatencyThreshold = 15;

		private const int PercentSampleBelowThresholdToAlert = 80;

		private const int MinimumStoreUsageStatisticsSampleCount = 125;

		private const double QuarantinedMailboxCountThreshold = 0.99;

		private const int NumberOfActiveBackgroundTasksThreshold = 15;

		private const int ActiveDBAvailabilityMonitoringThreshold = 3;

		private const int PassiveDBAvailabilityMonitoringThreshold = 3;

		private const int storeServiceMonitoringThreshold = 2;

		private const int ServiceCrashMonitoringThreshold = 6;

		private const int WorkerCrashMonitoringThreshold = 10;

		private const int VersionBucketsNumberOfSamplesAboveThreshold = 1;

		private const int storeAdminRPCInterfaceMonitoringThreshold = 4;

		internal const int CopyQueueLengthThreshold = 10;

		internal const int ReplayQueueLengthThreshold = 100;

		private const int DatabaseDiskReadLatencyNumberOfSamplesAboveThreshold = 1;

		internal const int MinimumRequiredDatabaseSchemaVersion = 126;

		private const int DatabaseSchemaVersionCheckNumberOfSamplesAboveThreshold = 1;

		private const int MaxRetryAttempt = 3;

		private const string EscalationTeam = "Store";

		private const string AssistantsServiceName = "MSExchangeMailboxAssistants";

		internal static readonly string[] RequiredMaintenanceResourceTypes = new string[]
		{
			"Store",
			"DirectoryServiceAndStore",
			"StoreUrgent",
			"StoreOnlineIntegrityCheck",
			"StoreScheduledIntegrityCheck"
		};

		private static TimeSpan percentRPCRequestsRecurrence = TimeSpan.FromMinutes(5.0);

		private static TimeSpan percentRPCRequestsDatabaseFailoverInterval = TimeSpan.FromMinutes(5.0);

		private static TimeSpan percentRPCRequestsEscalationInterval = TimeSpan.FromMinutes(15.0);

		internal static TimeSpan suppressQuarantineAlertDuration = TimeSpan.FromDays(1.0);

		internal static TimeSpan activeDBAvailabilityRecurrence = TimeSpan.FromMinutes(1.0);

		internal static TimeSpan activeDBAvailabilityTimeout = TimeSpan.FromSeconds(45.0);

		internal static TimeSpan activeDBAvailabilityMonitoringInterval = TimeSpan.FromMinutes(4.0);

		internal static TimeSpan activeDBAvailabilityEscalationNotificationInterval = TimeSpan.FromMinutes(17.0);

		internal static TimeSpan activeDBAvailabilityEscalationNotificationWaitInterval = TimeSpan.FromSeconds(1.0);

		internal static TimeSpan activeDBAvailabilityBackupEscalationNotificationInterval = TimeSpan.FromMinutes(30.0);

		internal static TimeSpan passiveDBAvailabilityRecurrence = TimeSpan.FromMinutes(5.0);

		internal static TimeSpan passiveDBAvailabilityMonitoringInterval = TimeSpan.FromMinutes(20.0);

		internal static TimeSpan passiveDBAvailabilityEscalationNotificationInterval = TimeSpan.FromMinutes(40.0);

		internal static TimeSpan passiveDBAvailabilityEscalationNotificationWaitInterval = TimeSpan.FromSeconds(1.0);

		private static TimeSpan storeServiceRecurrence = TimeSpan.FromMinutes(5.0);

		private static TimeSpan storeServiceStartTimeout = TimeSpan.FromMinutes(5.0);

		private static TimeSpan storeServiceMonitoringInterval = TimeSpan.FromMinutes(12.0);

		private static TimeSpan storeServiceEscalationInterval = TimeSpan.FromMinutes(30.0);

		private static TimeSpan serviceCrashMonitoringInterval = TimeSpan.FromHours(1.0);

		private static TimeSpan workerCrashMonitoringInterval = TimeSpan.FromHours(1.5);

		private static TimeSpan processCrashRecurrence = TimeSpan.FromMinutes(5.0);

		private static TimeSpan maintenanceAssistantEscalationNotificationInterval = TimeSpan.FromHours(8.0);

		private static TimeSpan maintenanceAssistantRecurrence = TimeSpan.FromMinutes(15.0);

		private static TimeSpan maintenanceAssistantServiceStartTimeout = TimeSpan.FromMinutes(5.0);

		private static TimeSpan maintenanceAssistantDurationBetweenServiceRestarts = TimeSpan.FromHours(4.0);

		private static TimeSpan maintenanceAssistantHeartbeatSLA = TimeSpan.FromHours(16.0);

		private static TimeSpan maintenanceAssistantEscalationNotificationWaitInterval = TimeSpan.FromSeconds(1.0);

		private static TimeSpan versionBucketsAllocatedRecurrence = TimeSpan.FromMinutes(5.0);

		private static TimeSpan versionBucketsAllocatedEscalationNotificationWaitInterval = TimeSpan.FromSeconds(1.0);

		private static TimeSpan versionBucketsAllocatedEscalationNotificationInterval = TimeSpan.FromMinutes(10.0);

		private static TimeSpan storeAdminRPCInterfaceRecurrence = TimeSpan.FromMinutes(1.0);

		private static TimeSpan storeAdminRPCInterfaceServiceStartTimeout = TimeSpan.FromMinutes(3.0);

		private static TimeSpan storeAdminRPCInterfaceMonitoringInterval = TimeSpan.FromMinutes(5.0);

		private static TimeSpan storeAdminRPCInterfaceEscalationInterval = TimeSpan.FromMinutes(30.0);

		private static TimeSpan validationTimeout = TimeSpan.FromSeconds(30.0);

		private static TimeSpan databaseDiskReadLatencyRecurrence = TimeSpan.FromMinutes(15.0);

		private static TimeSpan databaseDiskReadLatencyMonitoringInterval = new TimeSpan(StoreDiscovery.databaseDiskReadLatencyRecurrence.Ticks + StoreDiscovery.databaseDiskReadLatencyRecurrence.Ticks / 2L);

		private static TimeSpan databaseDiskReadLatencyEscalationInterval = TimeSpan.FromHours(4.0);

		private static TimeSpan databaseDiskReadLatencyWaitInterval = TimeSpan.FromSeconds(1.0);

		private static TimeSpan databaseSchemaVersionCheckRecurrence = TimeSpan.FromHours(1.0);

		private static TimeSpan databaseSchemaVersionCheckMonitoringInterval = new TimeSpan(StoreDiscovery.databaseSchemaVersionCheckRecurrence.Ticks + StoreDiscovery.databaseSchemaVersionCheckRecurrence.Ticks / 2L);

		private static TimeSpan databaseSchemaVersionCheckEscalationNotificationInterval = TimeSpan.FromHours(3.0);

		private static TimeSpan databaseSchemaVersionCheckTimeoutInterval = TimeSpan.FromMinutes(2.0);

		private static TimeSpan databaseSchemaVersionCheckWaitInterval = TimeSpan.FromSeconds(1.0);

		private static bool isDatacenter;

		private static readonly Type ActiveDBAvailabilityProbeType = typeof(ActiveDatabaseAvailabilityProbe);

		private static readonly Type PassiveDatabaseAvailabilityProbeType = typeof(PassiveDatabaseAvailabilityProbe);

		private static readonly Type DBAvailabilityEscalateResponderType = typeof(DatabaseAvailabilityEscalateResponder);

		private static readonly Type StoreServiceProbeType = typeof(GenericServiceProbe);

		private static readonly Type OverallXFailuresMonitorType = typeof(OverallXFailuresMonitor);

		private static readonly Type DatabaseRPCLatencyMonitorType = typeof(DatabaseRPCLatencyMonitor);

		private static readonly Type DatabaseRPCLatencyResponderType = typeof(DatabaseRPCLatencyResponder);

		private static readonly Type DatabaseRepeatedMountsMonitorType = typeof(ProbeResultGroupedByTimeMonitor);

		private static readonly Type StoreMaintenanceAssistantMonitorType = typeof(MaintenanceAssistantMonitor);

		private static readonly Type EscalationNotificationResponderType = typeof(EscalationNotificationResponder);

		private static readonly Type DatabaseAvailabilityEscalationNotificationResponderType = typeof(DatabaseAvailabilityEscalationNotificationResponder);

		private static readonly Type StoreKillServerByExceptionTypeResponderType = typeof(StoreKillServerByExceptionTypeResponder);

		private static readonly Type MailboxQuarantinedEscalateResponderType = typeof(MailboxQuarantinedEscalateResponder);

		private static readonly Type StoreAdminRPCInterfaceProbeType = typeof(StoreAdminRPCInterfaceProbe);

		private static readonly Type ProcessCrashConditionalEscalationResponderType = typeof(ConditionalEscalateResponder);

		private static readonly Type DatabaseSchemaVersionCheckProbeType = typeof(DatabaseSchemaVersionCheckProbe);

		private static readonly Type DatabaseSchemaVersionCheckMonitorType = typeof(DatabaseSchemaVersionCheckMonitor);

		private static readonly Type DatabaseProvisioningResponderType = typeof(DatabaseProvisioningResponder);

		private static readonly Type DatabaseSchemaVersionCheckEscalationNotificationResponderType = typeof(DatabaseSchemaVersionCheckEscalationNotificationResponder);
	}
}
