using System;
using System.Collections.Generic;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal sealed class ClusterMonitoringContext : MonitoringContextBase
	{
		public ClusterMonitoringContext(IMaintenanceWorkBroker broker, TracingContext traceContext) : base(broker, traceContext)
		{
		}

		public override void CreateContext()
		{
			base.InvokeCatchAndLog(delegate
			{
				this.CreateClusterRpcContext();
			});
			base.InvokeCatchAndLog(delegate
			{
				this.CreateClusterGroupContext();
			});
			base.InvokeCatchAndLog(delegate
			{
				this.CreateClusterNetworkContext();
			});
			base.InvokeCatchAndLog(delegate
			{
				this.CreateClusSvcCrashContext();
			});
			base.InvokeCatchAndLog(delegate
			{
				this.CreateClusterHangDetectionContext();
			});
			base.InvokeCatchAndLog(delegate
			{
				this.CreateNodeEvictedNotificationContext();
			});
		}

		private void CreateClusterRpcContext()
		{
			int num = HighAvailabilityConstants.ReqdDataProtectionInfrastructureDetection / 30;
			string probeName = "ClusterEndpointProbe";
			string text = "ClusterEndpointMonitor";
			string responderName = "ClusterEndpointRestart";
			string name = "ClusterEndpointEscalate";
			ProbeDefinition probeDefinition = ClusterRpcProbe.CreateDefinition(probeName, 30);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ClusteringServiceName, ExchangeComponent.Clustering, num, true, 300);
			monitorDefinition.MonitoringIntervalSeconds = (num + 1) * 30;
			monitorDefinition.RecurrenceIntervalSeconds = monitorDefinition.MonitoringIntervalSeconds / 2;
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by cluster related issues";
			int num2 = 86400;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					Responder = RestartServiceResponder.CreateDefinition(responderName, text, "ClusSvc", ServiceHealthStatus.Degraded, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, "Dag", false)
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, num2),
					Responder = EscalateResponder.CreateDefinition(name, HighAvailabilityConstants.ClusteringServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.ClusterServiceDownEscalationSubject(HighAvailabilityConstants.ClusteringServiceName, Environment.MachineName, num2 / 60), Strings.ClusterServiceDownEscalationMessage(num2 / 60), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateClusterGroupContext()
		{
			int num = HighAvailabilityConstants.NonHealthThreatingDataProtectionInfraDetection / 60;
			string probeName = "ClusterGroupProbe";
			string name = "ClusterGroupMonitor";
			string name2 = "ClusterGroupEscalate";
			ProbeDefinition probeDefinition = ClusterGroupProbe.CreateDefinition(probeName, 60);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ClusteringServiceName, ExchangeComponent.Clustering, num, true, (num + 1) * 60);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by cluster related issues";
			int num2 = 432000;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, num2),
					Responder = EscalateResponder.CreateDefinition(name2, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.ClusterGroupDownEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, num2 / 60), Strings.ClusterGroupDownEscalationMessage(num2 / 60), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateClusterNetworkContext()
		{
			int num = HighAvailabilityConstants.AdministrativelyDerivedFailureDetection / 900;
			string probeName = "ClusterNetworkProbe";
			string name = "ClusterNetworkMonitor";
			ProbeDefinition probeDefinition = ClusterNetworkProbe.CreateDefinition(probeName, 900);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ClusteringServiceName, ExchangeComponent.Clustering, num, true, 300);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by cluster related issues";
			monitorDefinition.MonitoringIntervalSeconds = 900 * (num + 1);
			monitorDefinition.RecurrenceIntervalSeconds = monitorDefinition.MonitoringIntervalSeconds / 2;
			base.EnrollWorkItem<MonitorDefinition>(monitorDefinition);
		}

		private void CreateClusterHangDetectionContext()
		{
			string text = "ClusterHangMonitor";
			string name = "ClusterHangRestartMonitor";
			string responderName = "ClusterHangRestartRemoteForceReboot";
			string responderName2 = "ClusterHangRestartEvict";
			string name2 = "ClusterHangEscalate";
			string name3 = "ClusterHangNodeEvictNowMonitor";
			string responderName3 = "ClusterHangNodeEvictNow";
			string responderName4 = "ClusterHangNodeCollectAndMerge";
			string serviceName = "MSExchangeRepl";
			string component = "Cluster";
			string tag = "ClusterHung";
			string tag2 = "ClusterNodeRestart";
			string tag3 = "HammerDown";
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, NotificationItem.GenerateResultName(serviceName, component, tag), HighAvailabilityConstants.ClusteringServiceName, ExchangeComponent.Clustering, 1, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = 120;
			monitorDefinition.MonitoringIntervalSeconds = 1020;
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by cluster related issues";
			List<MonitorStateResponderTuple> list = new List<MonitorStateResponderTuple>
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 900),
					Responder = EscalateResponder.CreateDefinition(name2, HighAvailabilityConstants.ClusteringServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.ClusterHangEscalationSubject(HighAvailabilityConstants.ClusteringServiceName, Environment.MachineName), Strings.ClusterHangEscalationMessage, true, NotificationServiceClass.Urgent, 3600, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			};
			if (LocalEndpointManager.IsDataCenter)
			{
				list.Add(new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, list[list.Count - 1].MonitorState.TransitionTimeout.Add(TimeSpan.FromMinutes(1.0))),
					Responder = CollectAndMergeResponder.CreateDefinition(responderName4, text, ServiceHealthStatus.Unrecoverable2, Environment.MachineName, "Exchange", true)
				});
			}
			base.AddChainedResponders(ref monitorDefinition, list.ToArray());
			if (LocalEndpointManager.IsDataCenter)
			{
				MonitorDefinition monitorDefinition2 = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, NotificationItem.GenerateResultName(serviceName, component, tag2), HighAvailabilityConstants.ClusteringServiceName, ExchangeComponent.Clustering, 1, true, 300);
				monitorDefinition2.RecurrenceIntervalSeconds = 120;
				monitorDefinition2.MonitoringIntervalSeconds = 300;
				monitorDefinition2.TargetResource = Environment.MachineName;
				monitorDefinition2.ServicePriority = 0;
				monitorDefinition2.ScenarioDescription = "Validate HA health is not impacted by cluster related issues";
				base.AddChainedResponders(ref monitorDefinition2, new MonitorStateResponderTuple[]
				{
					new MonitorStateResponderTuple
					{
						MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
						Responder = ClusterHungNodesForceRestartResponder.CreateDefinition(responderName, monitorDefinition2.Name, ServiceHealthStatus.Unhealthy, HighAvailabilityConstants.ClusteringServiceName, HighAvailabilityConstants.ClusteringServiceName, 60, 1, 1440, 1, true)
					},
					new MonitorStateResponderTuple
					{
						MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, 420),
						Responder = ClusterNodeShutdownEvictResponder.CreateDefinition(responderName2, monitorDefinition2.Name, ServiceHealthStatus.Unhealthy, HighAvailabilityConstants.ClusteringServiceName, HighAvailabilityConstants.ClusteringServiceName, true)
					}
				});
				MonitorDefinition monitorDefinition3 = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name3, NotificationItem.GenerateResultName(serviceName, component, tag3), HighAvailabilityConstants.ClusteringServiceName, ExchangeComponent.Clustering, 1, true, 300);
				monitorDefinition3.RecurrenceIntervalSeconds = 0;
				monitorDefinition3.TargetResource = Environment.MachineName;
				monitorDefinition3.ServicePriority = 0;
				monitorDefinition3.ScenarioDescription = "Validate HA health is not impacted by recurrent cluster hang related issues";
				base.AddChainedResponders(ref monitorDefinition3, new MonitorStateResponderTuple[]
				{
					new MonitorStateResponderTuple
					{
						MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
						Responder = ClusterNodeShutdownEvictResponder.CreateDefinition(responderName3, monitorDefinition3.Name, ServiceHealthStatus.Unhealthy, HighAvailabilityConstants.ClusteringServiceName, HighAvailabilityConstants.ClusteringServiceName, true)
					}
				});
			}
		}

		private void CreateNodeEvictedNotificationContext()
		{
			if (LocalEndpointManager.IsDataCenter)
			{
				string name = "ClusterServiceNodeEvictedMonitor";
				string name2 = "ClusterServiceNodeEvictedEscalate";
				string serviceName = "ExCapacity";
				string component = "NodeEvicted";
				string tag = "RepeatedlyOffendingNode";
				MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, NotificationItem.GenerateResultName(serviceName, component, tag), HighAvailabilityConstants.ClusteringServiceName, ExchangeComponent.Clustering, 1, true, 300);
				monitorDefinition.RecurrenceIntervalSeconds = 0;
				monitorDefinition.TargetResource = Environment.MachineName;
				monitorDefinition.ServicePriority = 0;
				monitorDefinition.ScenarioDescription = "Validate Capacity health is not impacted by cluster node down";
				base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
				{
					new MonitorStateResponderTuple
					{
						MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
						Responder = EscalateResponder.CreateDefinition(name2, HighAvailabilityConstants.ClusteringServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "Capacity", Strings.ClusterNodeEvictedEscalationSubject(HighAvailabilityConstants.ClusteringServiceName, Environment.MachineName), Strings.ClusterNodeEvictedEscalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
					}
				});
			}
		}

		private void CreateClusSvcCrashContext()
		{
			int num = HighAvailabilityConstants.TransientNonHealthThreatingDataProtectionInfraDetection / 60 / 5;
			int transientNonHealthThreatingDataProtectionInfraDetection = HighAvailabilityConstants.TransientNonHealthThreatingDataProtectionInfraDetection;
			int recurrenceInterval = Math.Max(transientNonHealthThreatingDataProtectionInfraDetection / 10, 300);
			string name = "ClusterServiceCrashProbe";
			string name2 = "ClusterServiceCrashMonitor";
			string responderName = "ClusterServiceCrashForceReboot";
			string name3 = "ClusterServiceCrashEscalate";
			ProbeDefinition probeDefinition = GenericProcessCrashDetectionProbe.CreateDefinition(name, "ClusSvc", 60, null, false);
			probeDefinition.ServiceName = HighAvailabilityConstants.ClusteringServiceName;
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(name2, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ClusteringServiceName, ExchangeComponent.Clustering, transientNonHealthThreatingDataProtectionInfraDetection, recurrenceInterval, num, true);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by cluster related issues";
			int transitionTimeoutSeconds = 3600;
			int num2 = 86400;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, transitionTimeoutSeconds),
					Responder = DagForceRebootServerResponder.CreateDefinition(responderName, monitorDefinition.Name, ServiceHealthStatus.Degraded)
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, num2),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ClusteringServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.ClusterServiceCrashEscalationSubject(HighAvailabilityConstants.ClusteringServiceName, Environment.MachineName, num, HighAvailabilityConstants.TransientNonHealthThreatingDataProtectionInfraDetection / 3600), Strings.ClusterServiceCrashEscalationMessage(num, HighAvailabilityConstants.TransientNonHealthThreatingDataProtectionInfraDetection / 3600, num2 / 3600), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}
	}
}
