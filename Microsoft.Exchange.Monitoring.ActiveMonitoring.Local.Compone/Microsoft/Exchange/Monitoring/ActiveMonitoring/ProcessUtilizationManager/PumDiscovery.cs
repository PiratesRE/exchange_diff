using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ProcessUtilizationManager
{
	public sealed class PumDiscovery : MaintenanceWorkItem
	{
		public PumDiscovery()
		{
			this.traceContext = new TracingContext();
			this.jobObjectNames = new List<string>
			{
				"DefaultAppPool"
			};
			if (ExEnvironment.IsTest)
			{
				this.jobObjectNames.Add("TestingThresholdBreachEventJobObject");
			}
		}

		private static bool IsWindows2012OrHigher
		{
			get
			{
				Version version = Environment.OSVersion.Version;
				return version.Major == 6 && version.Minor >= 2;
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (PumDiscovery.IsWindows2012OrHigher && LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceFunction(ExTraceGlobals.PUMTracer, this.traceContext, "PumDiscovery.DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ProcessUtilizationManager\\PumDiscovery.cs", 120);
				this.CreatePumServiceContext();
				this.CreateJobObjectMonitorsAndResponders();
			}
		}

		private void CreatePumServiceContext()
		{
			bool enabled = bool.Parse(base.Definition.Attributes["PumServiceRunningEnabled"]);
			int recurrenceIntervalSeconds = int.Parse(base.Definition.Attributes["PumServiceRunningProbeRecurrenceIntervalSeconds"]);
			ProbeDefinition definition = this.CreateProbeDefinition("PumServiceRunningProbe", typeof(GenericServiceProbe), "MSExchangeProcessUtilizationManager", recurrenceIntervalSeconds, enabled);
			base.Broker.AddWorkDefinition<ProbeDefinition>(definition, this.traceContext);
			int num = int.Parse(base.Definition.Attributes["PumServiceRunningMonitorRecurrenceIntervalSeconds"]);
			int monitoringInterval = int.Parse(base.Definition.Attributes["PumServiceRunningMonitorMonitoringIntervalSeconds"]);
			int numberOfFailures = int.Parse(base.Definition.Attributes["PumServiceRunningMonitorMonitoringThreshold"]);
			int transitionTimeoutSeconds = int.Parse(base.Definition.Attributes["PumServiceRunningMonitorUnhealthyStateTransitionTimeOut"]);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("PumServiceRunningMonitor", "PumServiceRunningProbe/MSExchangeProcessUtilizationManager", ExchangeComponent.Pum.Name, ExchangeComponent.Pum, monitoringInterval, num, numberOfFailures, enabled);
			monitorDefinition.TargetResource = "MSExchangeProcessUtilizationManager";
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, transitionTimeoutSeconds)
			};
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			int num2 = int.Parse(base.Definition.Attributes["PumServiceRestartTimeoutSeconds"]);
			int waitIntervalSeconds = int.Parse(base.Definition.Attributes["PumServiceRestartResponderThrottleSeconds"]);
			ResponderDefinition responderDefinition = RestartServiceResponder.CreateDefinition("PumServiceRestartResponder", monitorDefinition.Name, "MSExchangeProcessUtilizationManager", ServiceHealthStatus.Degraded, 15, 120, 0, false, DumpMode.None, null, 15.0, 0, "Exchange", null, true, true, null, false);
			WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.PUMTracer, this.traceContext, "PumDiscovery.DoWork: Created {0} for {1}", "PumServiceRestartResponder", "MSExchangeProcessUtilizationManager", null, "CreatePumServiceContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ProcessUtilizationManager\\PumDiscovery.cs", 197);
			responderDefinition.RecurrenceIntervalSeconds = num;
			responderDefinition.ServiceName = ExchangeComponent.Pum.Name;
			responderDefinition.WaitIntervalSeconds = waitIntervalSeconds;
			responderDefinition.Attributes["ServiceStartTimeout"] = TimeSpan.FromSeconds((double)num2).ToString();
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, this.traceContext);
			ResponderDefinition responderDefinition2 = this.CreateEscalateResponderDefinition("PumServiceRunningMonitor", ExchangeComponent.Pum, "MSExchangeProcessUtilizationManager", Strings.EscalationSubjectUnhealthy, Strings.EscalationMessageFailuresUnhealthy(Strings.PumServiceNotRunningEscalationMessage), enabled, NotificationServiceClass.Urgent);
			responderDefinition2.TargetHealthState = ServiceHealthStatus.Unhealthy;
			responderDefinition2.WaitIntervalSeconds = int.Parse(base.Definition.Attributes["PumServiceEscalateResponderThrottleSeconds"]);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, this.traceContext);
		}

		private void CreateJobObjectMonitorsAndResponders()
		{
			Array values = Enum.GetValues(typeof(PumDiscovery.ResourceTypes));
			foreach (string text in this.jobObjectNames)
			{
				foreach (object obj in values)
				{
					PumDiscovery.ResourceTypes resourceTypes = (PumDiscovery.ResourceTypes)obj;
					Component pum = ExchangeComponent.Pum;
					int recurrenceInterval = int.Parse(base.Definition.Attributes["JobObjectMonitorRecurrenceIntervalSeconds"]);
					int monitoringInterval = int.Parse(base.Definition.Attributes["JobObjectMonitorMonitoringIntervalSeconds"]);
					int numberOfFailures = int.Parse(base.Definition.Attributes["JobObjectMonitorMonitoringThreshold"]);
					MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(string.Join("_", new string[]
					{
						resourceTypes.ToString(),
						text
					}), NotificationItem.GenerateResultName(pum.Name, resourceTypes.ToString(), text), pum.Name, pum, monitoringInterval, recurrenceInterval, numberOfFailures, true);
					monitorDefinition.TargetResource = text;
					monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
					{
						new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
					};
					base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.traceContext);
					string escalationSubject = string.Empty;
					string escalationMessage = string.Empty;
					NotificationServiceClass notificationType = NotificationServiceClass.UrgentInTraining;
					PumDiscovery.ResourceTypes resourceTypes2 = resourceTypes;
					if (resourceTypes2 == PumDiscovery.ResourceTypes.Cpu)
					{
						escalationSubject = Strings.JobobjectCpuExceededThresholdSubject(text);
						escalationMessage = Strings.JobobjectCpuExceededThresholdMessage(text);
					}
					ResponderDefinition responderDefinition = this.CreateEscalateResponderDefinition(monitorDefinition.Name, pum, monitorDefinition.TargetResource, escalationSubject, escalationMessage, true, notificationType);
					responderDefinition.TargetHealthState = ServiceHealthStatus.Unhealthy;
					responderDefinition.WaitIntervalSeconds = int.Parse(base.Definition.Attributes["JobObjectEscalateResponderThrottleSeconds"]);
					base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, this.traceContext);
				}
			}
		}

		private ProbeDefinition CreateProbeDefinition(string probeName, Type probeType, string targetResource, int recurrenceIntervalSeconds, bool enabled)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = probeType.Assembly.Location;
			probeDefinition.TypeName = probeType.FullName;
			probeDefinition.Name = probeName;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = recurrenceIntervalSeconds;
			probeDefinition.MaxRetryAttempts = 3;
			probeDefinition.TargetResource = targetResource;
			probeDefinition.ServiceName = ExchangeComponent.Pum.Name;
			probeDefinition.Enabled = enabled;
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.PUMTracer, this.traceContext, "PumDiscovery.CreateProbeDefinition: Created ProbeDefinition '{0}' for '{1}'.", probeName, targetResource, null, "CreateProbeDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ProcessUtilizationManager\\PumDiscovery.cs", 328);
			return probeDefinition;
		}

		private ResponderDefinition CreateEscalateResponderDefinition(string monitorName, Component component, string targetResource, string escalationSubject, string escalationMessage, bool enabled, NotificationServiceClass notificationType)
		{
			string text = monitorName + "EscalateResponder";
			string alertMask = string.IsNullOrEmpty(targetResource) ? monitorName : (monitorName + "/" + targetResource);
			ResponderDefinition result = EscalateResponder.CreateDefinition(text, component.Name, monitorName, alertMask, targetResource, ServiceHealthStatus.None, component.Service, component.EscalationTeam, escalationSubject, escalationMessage, enabled, notificationType, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.PUMTracer, this.traceContext, "PumDiscovery.CreateEscalateResponderDefinition: Created Escalate ResponderDefinition '{0}' for '{1}'", text, targetResource, null, "CreateEscalateResponderDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ProcessUtilizationManager\\PumDiscovery.cs", 373);
			return result;
		}

		internal const string ServiceRunningProbeName = "PumServiceRunningProbe";

		private const int MaxRetryAttempt = 3;

		private const string ServiceName = "MSExchangeProcessUtilizationManager";

		private const string ServiceRunningMonitorName = "PumServiceRunningMonitor";

		private const string ServiceRestartResponderName = "PumServiceRestartResponder";

		private static readonly Type restartServiceResponderType = typeof(RestartServiceResponder);

		private static readonly Type OverallXFailuresMonitorType = typeof(OverallXFailuresMonitor);

		private readonly TracingContext traceContext;

		private readonly List<string> jobObjectNames;

		public enum ResourceTypes
		{
			Cpu
		}
	}
}
