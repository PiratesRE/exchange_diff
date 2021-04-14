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
	internal sealed class ReplServiceMonitoringContext : MonitoringContextBase
	{
		public ReplServiceMonitoringContext(IMaintenanceWorkBroker broker, TracingContext traceContext) : base(broker, traceContext)
		{
		}

		public override void CreateContext()
		{
			base.InvokeCatchAndLog(delegate
			{
				this.CreateServiceEndpointContext();
			});
			base.InvokeCatchAndLog(delegate
			{
				this.CreateReplSvcActiveManagerCheckContext();
			});
			base.InvokeCatchAndLog(delegate
			{
				this.CreateReplSvcCrashContext();
			});
			if (LocalEndpointManager.IsDataCenter)
			{
				base.InvokeCatchAndLog(delegate
				{
					this.CreateReplSvcADHealthContext();
				});
			}
		}

		private void CreateServiceEndpointContext()
		{
			int num = 45;
			int recurrenceInterval = num;
			int num2 = (int)Math.Round((double)HighAvailabilityConstants.ReqdDataProtectionInfrastructureDetection * 1.5) / num;
			int monitoringInterval = (num2 + 1) * num;
			string text = string.Format("ServiceHealth{0}EndpointProbe", "MSExchangeRepl");
			string text2 = string.Format("ServiceHealth{0}EndpointMonitor", "MSExchangeRepl");
			string text3 = string.Format("ServiceHealth{0}EndpointRestart", "MSExchangeRepl");
			string text4 = string.Format("ServiceHealth{0}EndpointRestartSecondTrial", "MSExchangeRepl");
			string responderName = string.Format("ServiceHealth{0}EndpointFailover", "MSExchangeRepl");
			string responderName2 = string.Format("ServiceHealth{0}ForceReboot", "MSExchangeRepl");
			string name = string.Format("ServiceHealth{0}EndpointEscalate", "MSExchangeRepl");
			string responderName3 = string.Format("ServiceHealth{0}CollectAndMerge", "MSExchangeRepl");
			ProbeDefinition workDefinition = TcpListenerProbe.CreateDefinition(text, num);
			ProbeDefinition workDefinition2 = TasksRpcListenerProbe.CreateDefinition(text, num);
			ProbeDefinition workDefinition3 = ServerLocatorProbe.CreateDefinition(text, num);
			base.EnrollWorkItem<ProbeDefinition>(workDefinition);
			base.EnrollWorkItem<ProbeDefinition>(workDefinition2);
			base.EnrollWorkItem<ProbeDefinition>(workDefinition3);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(text2, text, HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, monitoringInterval, recurrenceInterval, num2, true);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.IsHaImpacting = true;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by service endpoint issues";
			int transitionTimeoutSeconds = 180;
			int num3 = 180 + HighAvailabilityConstants.ReplayRestartWindow;
			int transitionTimeoutSeconds2 = 180 + HighAvailabilityConstants.ReplayRestartWindow * 2;
			int num4 = 3600;
			int num5 = 86400;
			List<MonitorStateResponderTuple> list = new List<MonitorStateResponderTuple>();
			List<MonitorStateResponderTuple> list2 = list;
			MonitorStateResponderTuple item = default(MonitorStateResponderTuple);
			item.MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, transitionTimeoutSeconds);
			string responderName4 = text3;
			string monitorName = text2;
			string windowsServiceName = "MSExchangeRepl";
			ServiceHealthStatus responderTargetState = ServiceHealthStatus.Degraded;
			string throttleGroupName = "Dag";
			item.Responder = RestartServiceResponder.CreateDefinition(responderName4, monitorName, windowsServiceName, responderTargetState, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", "wermgr", true, true, throttleGroupName, false);
			list2.Add(item);
			List<MonitorStateResponderTuple> list3 = list;
			MonitorStateResponderTuple item2 = default(MonitorStateResponderTuple);
			item2.MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded1, num3);
			string responderName5 = text4;
			string monitorName2 = text2;
			string windowsServiceName2 = "MSExchangeRepl";
			ServiceHealthStatus responderTargetState2 = ServiceHealthStatus.Degraded1;
			string throttleGroupName2 = "Dag";
			item2.Responder = RestartServiceResponder.CreateDefinition(responderName5, monitorName2, windowsServiceName2, responderTargetState2, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", "wermgr", true, true, throttleGroupName2, false);
			list3.Add(item2);
			list.Add(new MonitorStateResponderTuple
			{
				MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded2, transitionTimeoutSeconds2),
				Responder = SystemFailoverResponder.CreateDefinition(responderName, monitorDefinition.Name, ServiceHealthStatus.Degraded2, ExchangeComponent.DataProtection.ToString(), "Exchange", true)
			});
			list.Add(new MonitorStateResponderTuple
			{
				MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, num4),
				Responder = DagForceRebootServerResponder.CreateDefinition(responderName2, monitorDefinition.Name, ServiceHealthStatus.Unhealthy)
			});
			list.Add(new MonitorStateResponderTuple
			{
				MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, num5),
				Responder = EscalateResponder.CreateDefinition(name, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy1, "High Availability", Strings.ReplServiceDownEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName), Strings.ReplServiceDownEscalationMessage(Environment.MachineName, num5 / 60, (num5 - num3) / 60, (num5 - num4) / 60), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
			});
			List<MonitorStateResponderTuple> list4 = list;
			if (LocalEndpointManager.IsDataCenter)
			{
				list4.Add(new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, list4[list4.Count - 1].MonitorState.TransitionTimeout.Add(TimeSpan.FromMinutes(1.0))),
					Responder = CollectAndMergeResponder.CreateDefinition(responderName3, text2, ServiceHealthStatus.Unrecoverable2, Environment.MachineName, "Exchange", true)
				});
			}
			base.AddChainedResponders(ref monitorDefinition, list4.ToArray());
		}

		private void CreateReplSvcActiveManagerCheckContext()
		{
			int num = 120;
			int recurrenceInterval = num;
			int num2 = HighAvailabilityConstants.NonHealthThreatingDataProtectionInfraDetection * 3;
			int numberOfFailures = (int)((double)(num2 / num) * 0.8);
			string probeName = "ServiceHealthActiveManagerCheckProbe";
			string text = "ServiceHealthActiveManagerCheckMonitor";
			string responderName = "ServiceHealthActiveManagerRestartService";
			string responderName2 = "ServiceHealthActiveManagerForceReboot";
			string name = "ServiceHealthActiveManagerCheckEscalate";
			ProbeDefinition probeDefinition = ActiveManagerProbe.CreateDefinition(probeName, num);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(text, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, num2, recurrenceInterval, numberOfFailures, true);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by Active Manager issues";
			int num3 = HighAvailabilityConstants.ReplayRestartWindow + 3600;
			int num4 = 86400;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					Responder = RestartServiceResponder.CreateDefinition(responderName, text, "MSExchangeRepl", ServiceHealthStatus.Degraded, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, "Dag", false)
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded2, num3),
					Responder = DagForceRebootServerResponder.CreateDefinition(responderName2, monitorDefinition.Name, ServiceHealthStatus.Degraded2)
				},
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, num4),
					Responder = EscalateResponder.CreateDefinition(name, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.ActiveManagerUnhealthyEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName), Strings.ActiveManagerUnhealthyEscalationMessage(Environment.MachineName, num4 / 60, (num4 - num3) / 60), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateReplSvcCrashContext()
		{
			int num = HighAvailabilityConstants.TransientNonHealthThreatingDataProtectionInfraDetection / 60 / 5;
			int transientNonHealthThreatingDataProtectionInfraDetection = HighAvailabilityConstants.TransientNonHealthThreatingDataProtectionInfraDetection;
			int recurrenceInterval = Math.Max(transientNonHealthThreatingDataProtectionInfraDetection / 10, 300);
			string name = string.Format("ServiceHealth{0}CrashProbe", "MSExchangeRepl");
			string name2 = string.Format("ServiceHealth{0}CrashMonitor", "MSExchangeRepl");
			string name3 = string.Format("ServiceHealth{0}CrashEscalate", "MSExchangeRepl");
			ProbeDefinition probeDefinition = GenericProcessCrashDetectionProbe.CreateDefinition(name, "MSExchangeRepl".ToLower(), 60, null, false);
			probeDefinition.ServiceName = HighAvailabilityConstants.ServiceName;
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(name2, probeDefinition.ConstructWorkItemResultName(), HighAvailabilityConstants.ServiceName, ExchangeComponent.DataProtection, transientNonHealthThreatingDataProtectionInfraDetection, recurrenceInterval, num, true);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by cluster service issues";
			int num2 = 86400;
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, num2),
					Responder = EscalateResponder.CreateDefinition(name3, HighAvailabilityConstants.ServiceName, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", Strings.ReplServiceCrashEscalationSubject(HighAvailabilityConstants.ServiceName, Environment.MachineName, num, num2 / 60), Strings.ReplServiceCrashEscalationMessage(num, num2 / 60), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		private void CreateReplSvcADHealthContext()
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("MsExchangeReplADHealthMonitor", NotificationItem.GenerateResultName("MSExchangeRepl", "MonitoringADConfigManager", "ADConfigQueryStatus"), ExchangeComponent.DataProtection.Name, ExchangeComponent.DataProtection, 1, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate HA health is not impacted by AD related issues";
			string text = string.Format("Msexchangerepl reports AD is unhealthy on {0}", Environment.MachineName);
			base.AddChainedResponders(ref monitorDefinition, new MonitorStateResponderTuple[]
			{
				new MonitorStateResponderTuple
				{
					MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 900),
					Responder = EscalateResponder.CreateDefinition("MsExchangeReplADHealthResponder", "MSExchangeRepl", monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy, "High Availability", text, text, true, NotificationServiceClass.Scheduled, 3600, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false)
				}
			});
		}

		public const string ReplSvcADHealthMonitorName = "MsExchangeReplADHealthMonitor";

		public const string ReplSvcADHealthResponderName = "MsExchangeReplADHealthResponder";
	}
}
