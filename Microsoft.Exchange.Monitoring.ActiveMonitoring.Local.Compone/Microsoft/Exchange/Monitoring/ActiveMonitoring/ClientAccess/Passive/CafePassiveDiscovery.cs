using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess.Passive
{
	public sealed class CafePassiveDiscovery : DiscoveryWorkItem
	{
		protected override Trace Trace
		{
			get
			{
				return ExTraceGlobals.CafeTracer;
			}
		}

		protected override void CreateWorkTasks(CancellationToken cancellationToken)
		{
			this.breadcrumbs = new Breadcrumbs(1024, this.traceContext);
			try
			{
				if (!LocalEndpointManager.IsDataCenter)
				{
					this.breadcrumbs.Drop("CreateWorkTasks: Datacenter only.");
				}
				else if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
				{
					this.breadcrumbs.Drop("CreateWorkTasks: Cafe role is not present on this server.");
				}
				else
				{
					this.Configure();
					if (this.probesEnabled[PassiveMonitorType.CASRoutingLatency])
					{
						this.CreateCASRoutingLatencyDefinitions();
					}
					if (this.probesEnabled[PassiveMonitorType.CASRoutingFailure])
					{
						this.CreateCASRoutingFailureDefinitions();
					}
					if (this.probesEnabled[PassiveMonitorType.ThreadCount])
					{
						this.CreateThreadPoolDefinitions();
					}
				}
			}
			finally
			{
				this.ReportResult();
			}
		}

		private void CreateCASRoutingLatencyDefinitions()
		{
			this.breadcrumbs.Drop("Creating CAS Routing Latency work definitions");
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "CASRoutingLatencyTrigger_Error", null);
			MonitorDefinition monitorDefinition = this.CreateMonitorForEDS("ClientAccessRoutingLatencyMonitor", sampleMask);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate CAFE health is not impacted by CAS routing latency issues";
			this.CreateResponderChainForEDS(monitorDefinition, Strings.CASRoutingLatencyEscalationSubject, Strings.CASRoutingLatencyEscalationBody, PerfCounterHelper.UnitMs, NotificationServiceClass.UrgentInTraining);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.traceContext);
		}

		private void CreateCASRoutingFailureDefinitions()
		{
			this.breadcrumbs.Drop("Creating CAS Routing Failure work definitions");
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "CASRoutingFailureTrigger_Error", null);
			MonitorDefinition monitorDefinition = this.CreateMonitorForEDS("ClientAccessRoutingFailureMonitor", sampleMask);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate CAFE health is not impacted by CAS routing failure issues";
			this.CreateResponderChainForEDS(monitorDefinition, Strings.CASRoutingFailureEscalationSubject, Strings.CASRoutingFailureEscalationBody, PerfCounterHelper.UnitPercent, NotificationServiceClass.Urgent);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.traceContext);
		}

		private void CreateThreadPoolDefinitions()
		{
			this.breadcrumbs.Drop("Creating ThreadPool work definitions");
			int maxWorkerThreads = CafePassiveDiscovery.GetMaxWorkerThreads();
			int num = (int)((double)(maxWorkerThreads * Environment.ProcessorCount) * this.ThreadCountThresholdFraction);
			this.breadcrumbs.Drop("MaxWorkerThreads = {0}; Threshold = {1}", new object[]
			{
				maxWorkerThreads,
				num
			});
			foreach (ProtocolDescriptor protocolDescriptor in CafeProtocols.Protocols)
			{
				if (CafeDiscovery.IsProtocolAvailableInEnvironment(protocolDescriptor.HttpProtocol))
				{
					ProbeIdentity probeIdentity = ProbeIdentity.Create(protocolDescriptor.HealthSet, ProbeType.ProxyTest, "ThreadCount", protocolDescriptor.AppPool);
					probeIdentity = this.CreateProbeDefinition(probeIdentity, this.ThreadCountProbeRecurrenceIntervalSeconds, num);
					MonitorDefinition monitorDefinition = this.CreateMonitorForConsecutiveProbeFailures(probeIdentity, (this.ThreadCountProbeFailureCount + 1) * this.ThreadCountProbeRecurrenceIntervalSeconds, this.ThreadCountProbeFailureCount);
					monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
					{
						new MonitorStateTransition(ServiceHealthStatus.Unhealthy, this.ThreadCountUnheahlthyTransitionSeconds),
						new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.ThreadCountUnrecoverableTransitionSeconds)
					};
					monitorDefinition.ServicePriority = 0;
					monitorDefinition.ScenarioDescription = "Validate Cafe health is not impacted by threadpool issues";
					this.CreateResponderChain(monitorDefinition, Strings.CafeThreadCountSubjectUnhealthy(protocolDescriptor.AppPool), Strings.CafeThreadCountMessageUnhealthy(protocolDescriptor.AppPool, Math.Round(this.ThreadCountThresholdFraction * 100.0, 2).ToString(), maxWorkerThreads.ToString()), NotificationServiceClass.UrgentInTraining, ServiceHealthStatus.Unrecoverable);
					base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.traceContext);
				}
			}
		}

		private ProbeIdentity CreateProbeDefinition(ProbeIdentity probeIdentity, int recurrenceIntervalSeconds, int threshold)
		{
			ProbeDefinition probeDefinition = ThreadCountLocalProbe.CreateDefinition(probeIdentity, threshold);
			probeDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = 30;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, this.traceContext);
			return probeDefinition;
		}

		private MonitorDefinition CreateMonitorForConsecutiveProbeFailures(ProbeIdentity probeIdentity, int monitoringInterval, int numSamples)
		{
			MonitorIdentity monitorIdentity = probeIdentity.CreateMonitorIdentity();
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorIdentity.Name, probeIdentity.GetAlertMask(), monitorIdentity.Component.Name, monitorIdentity.Component, numSamples, true, monitoringInterval);
			monitorDefinition.TargetResource = probeIdentity.TargetResource;
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			return monitorDefinition;
		}

		private MonitorDefinition CreateMonitorForEDS(string monitorName, string sampleMask)
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorName, sampleMask, ExchangeComponent.Cafe.Name, ExchangeComponent.Cafe, 1, true, this.EdsMonitoringIntervalSeconds);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			return monitorDefinition;
		}

		private void CreateResponderChainForEDS(MonitorDefinition monitorDefinition, string escalationSubject, string escalationBody, string counterUnits, NotificationServiceClass alertClass)
		{
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			if (Utils.EnableResponderForCurrentEnvironment(this.AlertResponderEnabled, this.CreateRespondersForTest))
			{
				MonitorIdentity monitorIdentity = monitorDefinition;
				ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity("Escalate", null);
				this.breadcrumbs.Drop("Responder: {0}", new object[]
				{
					responderIdentity.Name
				});
				ResponderDefinition definition = PerfCounterEscalateResponder.CreateDefinition(responderIdentity.Name, responderIdentity.Component.Name, monitorIdentity.Name, monitorIdentity.GetAlertMask(), responderIdentity.TargetResource, ServiceHealthStatus.Unhealthy, monitorIdentity.Component.EscalationTeam, escalationSubject, escalationBody, counterUnits, true, alertClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59");
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, this.traceContext);
			}
		}

		private void CreateResponderChain(MonitorIdentity monitorIdentity, string escalationSubject, string escalationBody, NotificationServiceClass alertClass, ServiceHealthStatus healthStateForEscalation)
		{
			if (Utils.EnableResponderForCurrentEnvironment(this.AlertResponderEnabled, this.CreateRespondersForTest))
			{
				ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity("Escalate", null);
				this.breadcrumbs.Drop("Responder: {0}", new object[]
				{
					responderIdentity.Name
				});
				ResponderDefinition definition = EscalateResponder.CreateDefinition(responderIdentity.Name, responderIdentity.Component.Name, monitorIdentity.Name, monitorIdentity.GetAlertMask(), responderIdentity.TargetResource, healthStateForEscalation, monitorIdentity.Component.EscalationTeam, escalationSubject, escalationBody, true, alertClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, this.traceContext);
			}
		}

		private void ReportResult()
		{
			string text = this.breadcrumbs.ToString();
			base.Result.StateAttribute5 = text;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CafeTracer, this.traceContext, text, null, "ReportResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Cafe\\CafePassiveDiscovery.cs", 383);
		}

		private void Configure()
		{
			this.EdsMonitoringIntervalSeconds = (int)this.ReadAttribute("EdsMonitoringSpan", CafePassiveDiscovery.DefaultValues.EdsMonitoringInterval).TotalSeconds;
			this.ThreadCountProbeFailureCount = this.ReadAttribute("ThreadCountProbeFailureCount", 5);
			this.ThreadCountThresholdFraction = this.ReadAttribute("ThreadCountThresholdFractionOfMax", 0.9);
			this.ThreadCountProbeRecurrenceIntervalSeconds = (int)this.ReadAttribute("ThreadCountProbeRecurrenceInterval", CafePassiveDiscovery.DefaultValues.ThreadCountProbeRecurrenceIntervalSeconds).TotalSeconds;
			this.ThreadCountUnheahlthyTransitionSeconds = (int)this.ReadAttribute("ThreadCountUnheahlthyTransition", CafePassiveDiscovery.DefaultValues.ThreadCountUnheahlthyTransition).TotalSeconds;
			this.ThreadCountUnrecoverableTransitionSeconds = (int)this.ReadAttribute("ThreadCountUnrecoverableTransition", CafePassiveDiscovery.DefaultValues.ThreadCountUnrecoverableTransition).TotalSeconds;
			foreach (object obj in Enum.GetValues(typeof(PassiveMonitorType)))
			{
				PassiveMonitorType passiveMonitorType = (PassiveMonitorType)obj;
				this.probesEnabled[passiveMonitorType] = this.ReadAttribute(passiveMonitorType + "MonitorEnabled", true);
			}
			this.CreateRespondersForTest = this.ReadAttribute("CreateRespondersForTest", false);
			this.AlertResponderEnabled = this.ReadAttribute("AlertResponderEnabled", !ExEnvironment.IsTest);
		}

		private static int GetMaxWorkerThreads()
		{
			Configuration configuration = WebConfigurationManager.OpenMachineConfiguration();
			ProcessModelSection processModelSection = (ProcessModelSection)configuration.GetSection("system.web/processModel");
			return processModelSection.MaxWorkerThreads;
		}

		private const string CASRoutingLatencyName = "ClientAccessRoutingLatencyMonitor";

		private const string CASRoutingLatencyTriggerErrorMask = "CASRoutingLatencyTrigger_Error";

		private const string CASRoutingFailureName = "ClientAccessRoutingFailureMonitor";

		private const string CASRoutingFailureTriggerErrorMask = "CASRoutingFailureTrigger_Error";

		private Breadcrumbs breadcrumbs;

		private Dictionary<PassiveMonitorType, bool> probesEnabled = new Dictionary<PassiveMonitorType, bool>();

		private TracingContext traceContext = TracingContext.Default;

		private int ThreadCountProbeRecurrenceIntervalSeconds;

		private int ThreadCountUnheahlthyTransitionSeconds;

		private int ThreadCountUnrecoverableTransitionSeconds;

		private int ThreadCountProbeFailureCount;

		private double ThreadCountThresholdFraction;

		private int EdsMonitoringIntervalSeconds;

		private bool CreateRespondersForTest;

		private bool AlertResponderEnabled;

		private class DefaultValues
		{
			public const int ThreadCountProbeFailureCount = 5;

			public const double ThreadCountThresholdFraction = 0.9;

			public static readonly TimeSpan EdsMonitoringInterval = TimeSpan.FromMinutes(15.0);

			public static readonly TimeSpan ThreadCountProbeRecurrenceIntervalSeconds = TimeSpan.FromSeconds(60.0);

			public static readonly TimeSpan ThreadCountUnheahlthyTransition = TimeSpan.FromSeconds(0.0);

			public static readonly TimeSpan ThreadCountUnrecoverableTransition = TimeSpan.FromMinutes(15.0);
		}
	}
}
