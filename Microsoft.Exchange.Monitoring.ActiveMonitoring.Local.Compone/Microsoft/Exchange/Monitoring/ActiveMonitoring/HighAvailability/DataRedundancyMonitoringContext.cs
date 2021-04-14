using System;
using System.Collections.Generic;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal sealed class DataRedundancyMonitoringContext : MonitoringContextBase
	{
		public DataRedundancyMonitoringContext(IMaintenanceWorkBroker broker, TracingContext traceContext) : base(broker, traceContext)
		{
		}

		public override void CreateContext()
		{
			base.InvokeCatchAndLog(delegate
			{
				this.CreateOneCopyNIContext();
			});
			base.InvokeCatchAndLog(delegate
			{
				this.CreateOneCopyMonitorMonitoringContext();
			});
			base.InvokeCatchAndLog(delegate
			{
				this.CreatePotentialOneCopyByRemoteServerNIContext();
			});
		}

		private void CreateOneCopyNIContext()
		{
			string text = "ServerOneCopyMonitor";
			string name = "ServerOneCopyEscalate";
			string responderName = "ServerOneCopyCollectAndMerge";
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, NotificationItem.GenerateResultName("msexchangerepl", "DatabaseRedundancy", "OneCopyServerEvent"), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 300);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.MonitoringIntervalSeconds = (int)Math.Round(1080.0);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by lack of redundancy";
			List<MonitorStateResponderTuple> list = new List<MonitorStateResponderTuple>
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = EscalateResponder.CreateDefinition(name, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.InsufficientRedundancyEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName), Strings.InsufficientRedundancyEscalationMessage(Environment.MachineName), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			};
			if (LocalEndpointManager.IsDataCenter)
			{
				list.Add(new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, list[list.Count - 1].MonitorState.TransitionTimeout.Add(TimeSpan.FromMinutes(1.0))),
					Responder = CollectAndMergeResponder.CreateDefinition(responderName, text, ServiceHealthStatus.Unrecoverable2, Environment.MachineName, "Exchange", true)
				});
			}
			base.AddChainedResponders(ref monitorDefinition, list.ToArray());
		}

		private void CreateSingleLocationCopyContext()
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("SingleAvailableDatabaseCopyMonitor", NotificationItem.GenerateResultName("msexchangerepl", "DatabaseCopyAvailability", null), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 300);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.MonitoringIntervalSeconds = 300;
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by available database copies only in one location";
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = EscalateResponder.CreateDefinition("SingleAvailableDatabaseCopyEscalate", HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.SingleAvailableDatabaseCopyEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName), Strings.SingleAvailableDatabaseCopyEscalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateOneCopyMonitorMonitoringContext()
		{
			string name = "ServerOneCopyInternalMonitorProbe";
			string text = "ServerOneCopyInternalMonitorMonitor";
			string responderName = "ServerOneCopyInternalMonitorServiceRestart";
			string responderName2 = "ServerOneCopyInternalMonitorForceReboot";
			string name2 = "ServerOneCopyInternalMonitorEscalate";
			ProbeDefinition probeDefinition = ServiceMonitorProbe.CreateDefinition(name, HighAvailabilityConstants.ServiceName, 300);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 2, true, 300);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by any issues";
			monitorDefinition.MonitoringIntervalSeconds = 900;
			monitorDefinition.RecurrenceIntervalSeconds = monitorDefinition.MonitoringIntervalSeconds / 2;
			int num = 1800;
			int num2 = num + 7200;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					Responder = RestartServiceResponder.CreateDefinition(responderName, text, "MSExchangeDagMgmt", ServiceHealthStatus.Degraded, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, "Dag", false)
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded2, num),
					Responder = DagForceRebootServerResponder.CreateDefinition(responderName2, monitorDefinition.Name, ServiceHealthStatus.Degraded2)
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, num2),
					Responder = EscalateResponder.CreateDefinition(name2, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.OneCopyMonitorFailureEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, ServiceMonitorProbe.Suppression), Strings.EscalationMessageFailuresUnhealthy(Strings.OneCopyMonitorFailureMessage(ServiceMonitorProbe.Suppression + num2 / 60, ServiceMonitorProbe.Suppression)), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreatePotentialOneCopyByRemoteServerNIContext()
		{
			string text = "PotentialOneCopyByRemoteServerMonitor";
			string responderName = "PotentialOneCopyByRemoteServerRemoteRebootResponder";
			string name = "PotentialOneCopyByRemoteServerEscalate";
			string responderName2 = "PotentialOneCopyByRemoteServerCollectAndMerge";
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, NotificationItem.GenerateResultName("msexchangerepl", "DatabaseRedundancy", "PotentialOneCopyByRemoteServerEvent"), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, 1, true, 300);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.MonitoringIntervalSeconds = 360;
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by potential lack of redundancy";
			List<MonitorStateResponderTuple> list = new List<MonitorStateResponderTuple>();
			if (LocalEndpointManager.IsDataCenter)
			{
				list.Add(new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = PotentialOneCopyRemoteServerRestartResponder.CreateDefinition(responderName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), ServiceHealthStatus.Unhealthy, ExchangeComponent.DataProtection.Name, HighAvailabilityConstants.ServiceName, false)
				});
			}
			else
			{
				list.Add(new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
					Responder = null
				});
			}
			list.Add(new MonitorStateResponderTuple
			{
				MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, TimeSpan.FromMinutes(20.0)),
				Responder = EscalateResponder.CreateDefinition(name, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unrecoverable, "High Availability", Strings.PotentialInsufficientRedundancyEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName), Strings.PotentialInsufficientRedundancyEscalationMessage(Environment.MachineName), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
			});
			if (LocalEndpointManager.IsDataCenter)
			{
				list.Add(new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unrecoverable2, list[list.Count - 1].MonitorState.TransitionTimeout.Add(TimeSpan.FromMinutes(1.0))),
					Responder = CollectAndMergeResponder.CreateDefinition(responderName2, text, ServiceHealthStatus.Unrecoverable2, Environment.MachineName, "Exchange", true)
				});
			}
			base.AddChainedResponders(ref monitorDefinition, list.ToArray());
		}
	}
}
