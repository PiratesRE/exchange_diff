using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OAuth
{
	public sealed class OAuthPassiveDiscovery : DiscoveryWorkItem
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
					if (this.probesEnabled[PassiveMonitorType.OAuthRequestFailure])
					{
						this.CreateOAuthRequestFailureDefinitions();
					}
					if (this.probesEnabled[PassiveMonitorType.OAuthAcsTimeout])
					{
						this.CreateOAuthAcsTimeoutDefinitions();
					}
					if (this.probesEnabled[PassiveMonitorType.OAuthExpiredToken])
					{
						this.CreateOAuthExpiredTokenDefinitions();
					}
				}
			}
			finally
			{
				this.ReportResult();
			}
		}

		private void CreateOAuthRequestFailureDefinitions()
		{
			this.breadcrumbs.Drop("Creating OAuth Requests Failure work definitions");
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "OAuthRequestFailureTrigger_Error", null);
			MonitorDefinition monitorDefinition = this.CreateMonitorForEDS("EWSOAuthRequestFailureMonitor", sampleMask);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate OAuth health is not impacted by authentication request failures";
			this.CreateResponderChainForEDS(monitorDefinition, Strings.OAuthRequestFailureEscalationSubject, Strings.OAuthRequestFailureEscalationBody, PerfCounterHelper.UnitPercent, NotificationServiceClass.Urgent);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.traceContext);
		}

		private void CreateOAuthAcsTimeoutDefinitions()
		{
			this.breadcrumbs.Drop("Creating OAuth Acs timeout work definitions");
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "OAuthAcsTimeoutTrigger_Error", null);
			MonitorDefinition monitorDefinition = this.CreateMonitorForEDS("EWSOAuthAcsTimeoutMonitor", sampleMask);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate OAuth health is not impacted by authentication request failures";
			this.CreateResponderChainForEDS(monitorDefinition, Strings.OAuthRequestFailureEscalationSubject, Strings.OAuthRequestFailureEscalationBody, PerfCounterHelper.UnitPercent, NotificationServiceClass.Urgent);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.traceContext);
		}

		private void CreateOAuthExpiredTokenDefinitions()
		{
			this.breadcrumbs.Drop("Creating OAuth Requests Failure work definitions");
			string sampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "OAuthPassiveMonitoringExceptionAboveThreshold", null);
			MonitorDefinition monitorDefinition = this.CreateMonitorForEDS("EWSOAuthExpiredTokenMonitor", sampleMask);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate OAuth health is not impacted by authentication request failures";
			this.CreateResponderChainForEDS(monitorDefinition, "OAuth Expired Token passive monitor exceeded the threshold", "{Probe.ExtensionXml}", PerfCounterHelper.UnitPercent, NotificationServiceClass.Urgent);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.traceContext);
		}

		private MonitorDefinition CreateMonitorForEDS(string monitorName, string sampleMask)
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorName, sampleMask, ExchangeComponent.Ews.Name, ExchangeComponent.Ews, 1, true, this.EdsMonitoringIntervalSeconds);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate OAuth health is not impacted by authentication request failures";
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
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CafeTracer, this.traceContext, text, null, "ReportResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OAuth\\OAuthPassiveDiscovery.cs", 312);
		}

		private void Configure()
		{
			this.EdsMonitoringIntervalSeconds = (int)this.ReadAttribute("EdsMonitoringSpan", OAuthPassiveDiscovery.DefaultValues.EdsMonitoringInterval).TotalSeconds;
			foreach (object obj in Enum.GetValues(typeof(PassiveMonitorType)))
			{
				PassiveMonitorType passiveMonitorType = (PassiveMonitorType)obj;
				this.probesEnabled[passiveMonitorType] = this.ReadAttribute(passiveMonitorType + "MonitorEnabled", true);
			}
			this.CreateRespondersForTest = this.ReadAttribute("CreateRespondersForTest", true);
			this.AlertResponderEnabled = this.ReadAttribute("AlertResponderEnabled", !ExEnvironment.IsTest);
		}

		private const string OAuthRequestFailureName = "EWSOAuthRequestFailureMonitor";

		private const string OAuthRequestFailureTriggerErrorMask = "OAuthRequestFailureTrigger_Error";

		private const string OAuthAcsTimeoutName = "EWSOAuthAcsTimeoutMonitor";

		private const string OAuthAcsTimeoutTriggerErrorMask = "OAuthAcsTimeoutTrigger_Error";

		private const string OAuthExpiredTokenName = "EWSOAuthExpiredTokenMonitor";

		private const string OAuthExpiredTokenTriggerErrorMask = "OAuthPassiveMonitoringExceptionAboveThreshold";

		private Breadcrumbs breadcrumbs;

		private Dictionary<PassiveMonitorType, bool> probesEnabled = new Dictionary<PassiveMonitorType, bool>();

		private TracingContext traceContext = TracingContext.Default;

		private int EdsMonitoringIntervalSeconds;

		private bool CreateRespondersForTest;

		private bool AlertResponderEnabled;

		private class DefaultValues
		{
			public static readonly TimeSpan EdsMonitoringInterval = TimeSpan.FromMinutes(15.0);
		}
	}
}
