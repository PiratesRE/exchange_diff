using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal sealed class DatabaseCopyMonitoringContext : MonitoringContextBase
	{
		public DatabaseCopyMonitoringContext(IMaintenanceWorkBroker broker, LocalEndpointManager endpointManager, TracingContext traceContext) : base(broker, endpointManager, traceContext)
		{
		}

		public override void CreateContext()
		{
			bool isDataCenter = LocalEndpointManager.IsDataCenter;
			base.InvokeCatchAndLog(delegate
			{
				this.CreateTooManyDatabaseMountedMonitoringContext();
			});
			if (isDataCenter)
			{
				base.InvokeCatchAndLog(delegate
				{
					this.CreateControllerFailureContext();
				});
				base.InvokeCatchAndLog(delegate
				{
					this.CreateHighDiskLatencyMonitoringContext();
				});
			}
			using (IEnumerator<MailboxDatabaseInfo> enumerator = base.EndpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MailboxDatabaseInfo dbInfo = enumerator.Current;
					base.InvokeCatchAndLog(delegate
					{
						this.CreatePerfCounterMonitoringContext(dbInfo);
					});
					base.InvokeCatchAndLog(delegate
					{
						this.CreateSuspendedCopyContext(dbInfo);
					});
					base.InvokeCatchAndLog(delegate
					{
						this.CreateFailedAndSuspendedCopyContext(dbInfo);
					});
					base.InvokeCatchAndLog(delegate
					{
						this.CreateIoHardFailureContext(dbInfo);
					});
					base.InvokeCatchAndLog(delegate
					{
						this.CreateStalledCopyContext(dbInfo);
					});
					base.InvokeCatchAndLog(delegate
					{
						this.CreateNTFSCorruptionMonitoringContext(dbInfo);
					});
					if (isDataCenter)
					{
						base.InvokeCatchAndLog(delegate
						{
							this.CreateUnMonitoredDatabaseContext(dbInfo);
						});
						base.InvokeCatchAndLog(delegate
						{
							this.CreateCircLoggingContext(dbInfo);
						});
					}
				}
			}
		}

		private void CreatePerfCounterMonitoringContext(MailboxDatabaseInfo dbInfo)
		{
			string perfCounterName = string.Format("MSExchange Database ==> Instances\\Log Bytes Write/sec\\Information Store - {0}/_Total", dbInfo.MailboxDatabaseName);
			MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition("DatabaseHealthLogGenerationRateMonitor", PerformanceCounterNotificationItem.GenerateResultName(perfCounterName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 10240000.0, HighAvailabilityConstants.TransientSuppressedLoadDetectionWindow / 300, true);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by log perf issues";
			base.EnrollWorkItem<MonitorDefinition>(monitorDefinition);
			perfCounterName = string.Format("MSExchange Replication\\Log Copying is Not Keeping Up\\{0}", dbInfo.MailboxDatabaseName);
			MonitorDefinition monitorDefinition2 = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition("DatabaseHealthLogCopyQueueMonitor", PerformanceCounterNotificationItem.GenerateResultName(perfCounterName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 0.9, HighAvailabilityConstants.AdministrativelyDerivedFailureDetection / 300, true);
			monitorDefinition2.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition2.ServicePriority = 0;
			monitorDefinition2.ScenarioDescription = "Validate HA health is not impacted by log perf issues";
			base.EnrollWorkItem<MonitorDefinition>(monitorDefinition2);
			perfCounterName = string.Format("MSExchange Replication\\Log Replay is Not Keeping Up\\{0}", dbInfo.MailboxDatabaseName);
			MonitorDefinition monitorDefinition3 = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition("DatabaseHealthLogReplayQueueMonitor", PerformanceCounterNotificationItem.GenerateResultName(perfCounterName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 0.9, HighAvailabilityConstants.AdministrativelyDerivedFailureDetection / 300, true);
			monitorDefinition3.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition3.ServicePriority = 0;
			monitorDefinition3.ScenarioDescription = "Validate HA health is not impacted by log perf issues";
			base.EnrollWorkItem<MonitorDefinition>(monitorDefinition3);
		}

		private void CreateCircLoggingContext(MailboxDatabaseInfo dbInfo)
		{
			string name = "DatabaseHealthCircLoggingProbe";
			string name2 = "DatabaseHealthCircLoggingMonitor";
			string name3 = "DatabaseHealthCircLoggingEscalate";
			ProbeDefinition probeDefinition = CircularLoggingProbe.CreateDefinition(name, HighAvailabilityConstants.ServiceName, dbInfo, 300);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, HighAvailabilityConstants.TransientSuppressedLoadDetectionWindow / 300, true, 300);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "VValidate HA health is not impacted by circular logging issues";
			monitorDefinition.MonitoringIntervalSeconds = (HighAvailabilityConstants.TransientSuppressedLoadDetectionWindow / 300 + 1) * 300;
			monitorDefinition.RecurrenceIntervalSeconds = monitorDefinition.MonitoringIntervalSeconds / 2;
			int transitionTimeoutSeconds = 432000;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, transitionTimeoutSeconds),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.CircularLoggingDisabledEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.CircularLoggingDisabledEscalationMessage(dbInfo.MailboxDatabaseName), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateUnMonitoredDatabaseContext(MailboxDatabaseInfo dbInfo)
		{
			string name = "DatabaseHealthUnMonitoredDatabaseProbe";
			string name2 = "DatabaseHealthUnMonitoredDatabaseMonitor";
			string name3 = "EnableDatabaseMonitoringResponder";
			string name4 = "EnableDatabaseMonitoringEscalate";
			int num = 604800;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 214, "CreateUnMonitoredDatabaseContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\MonitoringContext\\DatabaseCopyMonitoringContext.cs");
			MailboxDatabase mailboxDatabase = topologyConfigurationSession.FindDatabaseByGuid<MailboxDatabase>(dbInfo.MailboxDatabaseGuid);
			if (mailboxDatabase == null)
			{
				throw new DatabaseNotFoundInADException(dbInfo.MailboxDatabaseGuid.ToString());
			}
			if (!mailboxDatabase.IsExcludedFromProvisioning)
			{
				num = 7200;
			}
			TimeSpan duration = TimeSpan.FromSeconds((double)num);
			ProbeDefinition probeDefinition = UnMonitoredDatabaseProbe.CreateDefinition(name, HighAvailabilityConstants.ServiceName, dbInfo, 300);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 300);
			monitorDefinition.MonitoringIntervalSeconds = 600;
			monitorDefinition.RecurrenceIntervalSeconds = 300;
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by unmonitored database issues";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					Responder = EnableDatabaseMonitoringResponder.CreateDefinition(name3, HighAvailabilityConstants.ServiceName, dbInfo, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Degraded, "Dag")
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, num),
					Responder = EscalateResponder.CreateDefinition(name4, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.UnMonitoredDatabaseEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName, duration), Strings.UnmonitoredDatabaseEscalationMessage(dbInfo.MailboxDatabaseName), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateLagMonitoringContext(MailboxDatabaseInfo dbInfo)
		{
			string name = "DatabaseHealthLagCopyHealthProbe";
			string name2 = "DatabaseHealthLagCopyHealthMonitor";
			ProbeDefinition probeDefinition = LaggedCopyProbe.CreateDefinition(name, HighAvailabilityConstants.ServiceName, dbInfo, 600);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, HighAvailabilityConstants.AdministrativelyDerivedFailureDetection / 600, true, 300);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by database copy lag issues";
			base.EnrollWorkItem<MonitorDefinition>(monitorDefinition);
		}

		private void CreateSuspendedCopyContext(MailboxDatabaseInfo dbInfo)
		{
			string name = "DatabaseHealthDbCopySuspendedProbe";
			string name2 = "DatabaseHealthDbCopySuspendedMonitor";
			ProbeDefinition probeDefinition = SuspendedCopyProbe.CreateDefinition(name, HighAvailabilityConstants.ServiceName, dbInfo, HighAvailabilityConstants.AdministrativelyDerivedFailureDetection, 300);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 3, true, 300);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.RecurrenceIntervalSeconds = 300;
			monitorDefinition.MonitoringIntervalSeconds = 1500;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by database copy suspension issue";
			base.EnrollWorkItem<MonitorDefinition>(monitorDefinition);
		}

		private void CreateFailedAndSuspendedCopyContext(MailboxDatabaseInfo dbInfo)
		{
			string name = "DatabaseHealthDbCopyFailedAndSuspendedProbe";
			string name2 = "DatabaseHealthDbCopyFailedAndSuspendedMonitor";
			string name3 = "DatabaseHealthDbCopyFailedAndSuspendedEscalate";
			ProbeDefinition probeDefinition = FailedAndSuspendedCopyProbe.CreateDefinition(name, HighAvailabilityConstants.ServiceName, dbInfo, 518400, 300);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 3, true, 300);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.RecurrenceIntervalSeconds = 300;
			monitorDefinition.MonitoringIntervalSeconds = 1500;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by database copy suspension or failure";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, HighAvailabilityConstants.ReqdDataProtectionInfrastructureEscalate2),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.FailedAndSuspendedCopyEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName, string.Format("{0:0.##}", (518400 + HighAvailabilityConstants.ReqdDataProtectionInfrastructureEscalate2) / 3600)), Strings.FailedAndSuspendedCopyEscalationMessage(dbInfo.MailboxDatabaseName, string.Format("{0:0.##}", (518400 + HighAvailabilityConstants.ReqdDataProtectionInfrastructureEscalate2) / 3600)), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateStalledCopyContext(MailboxDatabaseInfo dbInfo)
		{
			string name = "DatabaseHealthDbCopyStalledProbe";
			string name2 = "DatabaseHealthDbCopyStalledMonitor";
			string name3 = "DatabaseHealthDbCopyStalledEscalate";
			ProbeDefinition probeDefinition = StalledCopyProbe.CreateDefinition(name, HighAvailabilityConstants.ServiceName, dbInfo, 300);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name2, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 6, true, 300);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.RecurrenceIntervalSeconds = 300;
			monitorDefinition.MonitoringIntervalSeconds = 2100;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, HighAvailabilityConstants.ReqdDataProtectionInfrastructureEscalate2),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.StalledCopyEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName, string.Format("{0:0.##}", HighAvailabilityConstants.ReqdDataProtectionInfrastructureEscalate2 / 60)), Strings.StalledCopyEscalationMessage(dbInfo.MailboxDatabaseName, string.Format("{0:0.##}", HighAvailabilityConstants.ReqdDataProtectionInfrastructureEscalate2 / 60)), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateIoHardFailureContext(MailboxDatabaseInfo dbInfo)
		{
			string name = "StorageDbIoHardFailureItemMonitor";
			string name2 = "StorageDbIoHardFailureItemEscalate";
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, NotificationItem.GenerateResultName("msexchangerepl", "DbFailureItem", dbInfo.MailboxDatabaseName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 300);
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.RecurrenceIntervalSeconds = 300;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by IO hardware failure";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, HighAvailabilityConstants.NonHealthThreatingDataProtectionInfraDetection),
					Responder = null
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, HighAvailabilityConstants.EstimatedReseedTime),
					Responder = EscalateResponder.CreateDefinition(name2, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.DbFailureItemIoHardEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.DbFailureItemIoHardEscalationMessage(dbInfo.MailboxDatabaseName), true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateControllerFailureContext()
		{
			string name = "StorageApparentControllerIssuesMonitor";
			string name2 = "StorageApparentControllerIssuesProbe";
			string name3 = "StorageApparentControllerIssuesEscalate";
			string responderName = "StorageApparentControllerIssuesKillServer";
			EventLogSubscription eventLogSubscription = new EventLogSubscription(name2, TimeSpan.FromSeconds(1800.0), new EventMatchingRule("System", "HpCISSs2", new int[]
			{
				129
			}, -1, false, false, null, null), null, null, null);
			EventLogNotification.Instance.AddSubscription(eventLogSubscription);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(name, EventLogNotification.ConstructResultMask(eventLogSubscription.Name, null), HighAvailabilityConstants.ControllerServiceName, ExchangeComponent.DiskController, 1800, 600, 3, true);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by IO hardware failure";
			int num = 604800;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, HighAvailabilityConstants.ReqdDataProtectionInfrastructureBugcheck),
					Responder = DagForceRebootServerResponder.CreateDefinition(responderName, HighAvailabilityConstants.ControllerServiceName, monitorDefinition.ConstructWorkItemResultName(), ServiceHealthStatus.Degraded)
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, num),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ControllerServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.ControllerFailureEscalationSubject(HighAvailabilityConstants.ControllerServiceName, monitorDefinition.TargetResource), Strings.ControllerFailureMessage(monitorDefinition.TargetResource, num / 86400), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateTooManyDatabaseMountedMonitoringContext()
		{
			string perfCounterName = "MSExchange Active Manager\\database mounted\\_total";
			int? maximumActiveDatabases = CachedAdReader.Instance.LocalServer.MaximumActiveDatabases;
			int? maximumPreferredActiveDatabases = CachedAdReader.Instance.LocalServer.MaximumPreferredActiveDatabases;
			int num = 0;
			if (maximumActiveDatabases == null && maximumPreferredActiveDatabases == null)
			{
				return;
			}
			if (maximumPreferredActiveDatabases != null && maximumActiveDatabases == null)
			{
				num = maximumPreferredActiveDatabases.Value + 1;
			}
			else if (maximumActiveDatabases != null && maximumPreferredActiveDatabases == null)
			{
				num = maximumActiveDatabases.Value;
			}
			else if (maximumActiveDatabases != null && maximumPreferredActiveDatabases != null)
			{
				num = Math.Min(maximumActiveDatabases.Value, maximumPreferredActiveDatabases.Value + 1);
			}
			base.AddMessage(string.Format("TooManyDbMountedMonitorThreshold={0}. MaxActive={1}, MaxPrefActive={2}", num, (maximumActiveDatabases != null) ? maximumActiveDatabases.Value.ToString() : "NULL", (maximumPreferredActiveDatabases != null) ? maximumPreferredActiveDatabases.Value.ToString() : "NULL"));
			if (num < 1)
			{
				base.AddMessage(string.Format("Skipping creation of TooManyDbMounted Monitor because threshold is less than 1 (Value={0})", num));
				return;
			}
			MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition("DatabaseHealthTooManyMountedDatabaseMonitor", PerformanceCounterNotificationItem.GenerateResultName(perfCounterName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, (double)num, 12, true);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted too many databases mounted issue";
			monitorDefinition.MonitoringIntervalSeconds = 65;
			monitorDefinition.RecurrenceIntervalSeconds = monitorDefinition.MonitoringIntervalSeconds / 2;
			int transitionTimeoutSeconds = 14400;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, transitionTimeoutSeconds),
					Responder = EscalateResponder.CreateDefinition("DatabaseHealthTooManyMountedDatabaseEscalate", HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.Unhealthy, "High Availability", Strings.TooManyDatabaseMountedEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName), Strings.TooManyDatabaseMountedEscalationMessage(num), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateHighDiskLatencyMonitoringContext()
		{
			ProbeDefinition probeDefinition = DiskLatencyProbe.CreateDefinition("HighDiskLatencyProbe", HighAvailabilityConstants.ServiceName, 24.0, 900, 0.01);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("HighDiskLatencyMonitor", probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 1800);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.RecurrenceIntervalSeconds = 900;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by high disk write latencies";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					Responder = ResetControllerCacheResponder.CreateDefinition("HighDiskLatencyControllerCacheReset", HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.Degraded, "Dag")
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 3600),
					Responder = GenericEventWriteResponder.CreateDefinition("HighDiskLatencyRepairboxEventLogger", HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.Unhealthy, "Microsoft-Exchange-HighAvailability/Operational", "Microsoft-Exchange-HighAvailability", 387L, string.Format("{0},HighDiskLatency,{1},{2}", Environment.MachineName, 10.0, 1500.0), EventLogEntryType.Error, 30, null)
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 345600),
					Responder = EscalateResponder.CreateDefinition("HighDiskLatencyEscalate", HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.Unhealthy, "High Availability", Strings.HighDiskLatencyEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, 10.0.ToString(), 96.ToString()), Strings.HighDiskLatencyEscalationMessage(Environment.MachineName, 10.0.ToString(), 96.ToString()), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateNTFSCorruptionMonitoringContext(MailboxDatabaseInfo dbInfo)
		{
			EventLogSubscription eventLogSubscription = new EventLogSubscription("NTFSCorruptionEventProbe", new EventMatchingRule("System", "Ntfs", new int[]
			{
				55
			}, 0, false, false, null, new EventMatchingRule.CustomNotification(DatabaseCopyMonitoringContext.NTFSCorruptionEventNotificationProcessor)), null, null, null);
			EventLogNotification.Instance.AddSubscription(eventLogSubscription);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("NTFSCorruptionMonitor", EventLogNotification.ConstructResultMask(eventLogSubscription.Name, dbInfo.MailboxDatabaseName), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 86400);
			monitorDefinition.RecurrenceIntervalSeconds = 1800;
			monitorDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by NTFS corruption issues";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					Responder = RaiseNTFSCorruptionFailureItemResponder.CreateDefinition("NTFSCorruptionLogFailureItemResponder", HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, ServiceHealthStatus.Degraded, "Dag")
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 176400),
					Responder = EscalateResponder.CreateDefinition("NTFSCorruptionEscalate", HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), dbInfo.MailboxDatabaseName, ServiceHealthStatus.Unrecoverable, "High Availability", Strings.NTFSCorruptionEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, dbInfo.MailboxDatabaseName), Strings.NTFSCorruptionEscalationMessage(dbInfo.MailboxDatabaseName, 49.ToString()), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private static void NTFSCorruptionEventNotificationProcessor(EventLogNotification.EventRecordInternal eventRecord, ref EventLogNotification.EventNotificationMetadata eventNotification)
		{
			string tagName = eventNotification.TagName;
			if (string.IsNullOrEmpty(tagName))
			{
				eventNotification.TagName = "EmptyDriveName";
				eventNotification.StateAttribute3 = "Drive name is empty";
				WTFDiagnostics.TraceError(ExTraceGlobals.HighAvailabilityTracer, TracingContext.Default, "TagName from event notification metadata is empty", null, "NTFSCorruptionEventNotificationProcessor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\MonitoringContext\\DatabaseCopyMonitoringContext.cs", 867);
				return;
			}
			IADDatabase[] allLocalDatabases = CachedAdReader.Instance.AllLocalDatabases;
			if (allLocalDatabases == null)
			{
				eventNotification.StateAttribute3 = "No database is found in AD";
				WTFDiagnostics.TraceError(ExTraceGlobals.HighAvailabilityTracer, TracingContext.Default, "No database is found in AD", null, "NTFSCorruptionEventNotificationProcessor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\MonitoringContext\\DatabaseCopyMonitoringContext.cs", 880);
				return;
			}
			KeyValuePair<Guid, CopyStatusClientCachedEntry>[] dbsCopyStatusOnLocalServer = CachedDbStatusReader.Instance.GetDbsCopyStatusOnLocalServer((from d in allLocalDatabases
			select d.Guid).ToArray<Guid>());
			VolumeManager defaultInstance = VolumeManager.DefaultInstance;
			defaultInstance.RefreshIfTooStale(TimeSpan.FromMinutes(5.0));
			Exception ex = null;
			IEnumerable<CopyStatusClientCachedEntry> enumerable = defaultInstance.LookUpCopyStatusesForVolumeLabel(tagName, from keyValue in dbsCopyStatusOnLocalServer
			select keyValue.Value, out ex);
			if (enumerable == null || !enumerable.Any<CopyStatusClientCachedEntry>())
			{
				eventNotification.StateAttribute3 = "No copy status is found by volume label";
				string text = "";
				if (ex != null)
				{
					text = ex.ToString();
				}
				eventNotification.StateAttribute4 = text;
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.HighAvailabilityTracer, TracingContext.Default, "No copy status is found by volume label. {0}", text, null, "NTFSCorruptionEventNotificationProcessor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\MonitoringContext\\DatabaseCopyMonitoringContext.cs", 910);
				return;
			}
			string[] array = (from c in enumerable
			select c.CopyStatus.DBName).ToArray<string>();
			Array.Sort<string>(array);
			string text2 = array.FirstOrDefault<string>();
			eventNotification.TagName = text2;
			eventNotification.StateAttribute3 = text2;
		}
	}
}
