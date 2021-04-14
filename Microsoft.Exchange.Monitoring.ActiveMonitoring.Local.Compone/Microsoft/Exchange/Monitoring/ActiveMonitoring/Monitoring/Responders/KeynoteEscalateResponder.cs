using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Monitoring.Responders
{
	public class KeynoteEscalateResponder : EscalateResponder
	{
		public static ResponderDefinition CreateResponderDefinition(string alertMask, string target, double minAvailabilityThreshold, int minISPCount, NotificationServiceClass notificationType)
		{
			string text = "https://osp.outlook.com/ecp/osp/Health/ErrorsPopup.aspx?ProbeName=Keynote Probe&Service=Exchange&HourCount=1&scope=&scopetype=isp";
			string text2 = "onenote:https://msft.spoppe.com/teams/exchange/dc/Shared Documents/Network/EXO_Network_Guide/Automated Escalations.one#Keynote%20Availability%20alert&section-id={B08EE15E-F03E-4554-BDB3-0E67C681A188}&page-id={E8CF9B34-CFAB-417A-8AB4-9720D2E8187D}&end";
			string text3 = "https://msft.spoppe.com/teams/exchange/dc/_layouts/OneNote.aspx?id=%2fteams%2fexchange%2fdc%2fShared%20Documents%2fNetwork%2fEXO_Network_Guide&wd=target%28Automated%20Escalations.one%7cB08EE15E-F03E-4554-BDB3-0E67C681A188%2fKeynote%20Availability%20alert%7cE8CF9B34-CFAB-417A-8AB4-9720D2E8187D%2f%29";
			ResponderDefinition responderDefinition = new ResponderDefinition
			{
				AssemblyPath = KeynoteEscalateResponder.AssemblyPath,
				TypeName = KeynoteEscalateResponder.TypeName,
				Name = "Keynote Escalate Responder",
				ServiceName = "Monitoring",
				AlertTypeId = "Monitoring/CentralActiveMonitoring/KeynoteMeasurementFailure",
				AlertMask = alertMask,
				TargetResource = target,
				TargetHealthState = ServiceHealthStatus.Unhealthy,
				EscalationTeam = ExchangeComponent.Monitoring.Name,
				EscalationSubject = string.Format("Local Active Monitoring Keynote measurement failed for agent '{0}'", target),
				EscalationMessage = string.Format("Keynote measurements monitor failed. \r\n                    Agent: '{0}'\r\n                    Availability measured from the agent fell below {1} for at least {2} ISPs\r\n                    Detailed graph available on <a href='{3}'>OSP errors page</a>\r\n                    Detailed instruction available on <a href='{4}'>Keynote Availability alert</a> (<a href='{5}'>Web view</a>)", new object[]
				{
					target,
					minAvailabilityThreshold,
					minISPCount,
					text,
					text2,
					text3
				}),
				Enabled = true,
				NotificationServiceClass = notificationType,
				MinimumSecondsBetweenEscalates = 3600
			};
			responderDefinition.WorkItemVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.TimeoutSeconds = 30;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.TargetResource = target;
			responderDefinition.AccountPassword = string.Empty;
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.MonitoringTracer, base.TraceContext, "Running keynote escalate responder actions", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Monitoring\\Keynote\\KeynoteEscalateResponder.cs", 142);
			MonitorResult lastFailedMonitorResult = WorkItemResultHelper.GetLastFailedMonitorResult(this, base.Broker, cancellationToken);
			if (lastFailedMonitorResult != null)
			{
				base.Result.StateAttribute5 = lastFailedMonitorResult.StateAttribute3;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.MonitoringTracer, base.TraceContext, "Custom keynote escalate responder action completed.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Monitoring\\Keynote\\KeynoteEscalateResponder.cs", 152);
			base.DoResponderWork(cancellationToken);
		}

		protected override void InvokeNewServiceAlert(Guid alertGuid, string alertTypeId, string alertName, string alertDescription, DateTime raisedTime, string escalationTeam, string service, string alertSource, bool isDatacenter, bool urgent, string environment, string location, string forest, string dag, string site, string region, string capacityUnit, string rack, string alertCategory)
		{
			alertDescription = string.Format("{0}\r\n                {1}", alertDescription, base.Result.StateAttribute5);
			alertSource = "KeynoteOSP";
			base.InvokeNewServiceAlert(alertGuid, alertTypeId, alertName, alertDescription, raisedTime, escalationTeam, service, alertSource, isDatacenter, urgent, environment, location, "KeynoteForest", "Keynote", "keynoteDag", region, capacityUnit, rack, alertCategory);
		}

		private const string KeynoteSiteName = "keynoteDag";

		private const string KeynoteDagName = "Keynote";

		private const string KeynoteForestName = "KeynoteForest";

		private const string KeynoteMachineName = "KeynoteMachine";

		private const string KeynoteAlertSource = "KeynoteOSP";

		public const int ResponderRecurrenceSeconds = 300;

		private const string KeynoteResponderName = "Keynote Escalate Responder";

		private const string KeynoteAlertTypeId = "Monitoring/CentralActiveMonitoring/KeynoteMeasurementFailure";

		private const int MinimumSecondsBetweenEscalations = 3600;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(KeynoteEscalateResponder).FullName;
	}
}
