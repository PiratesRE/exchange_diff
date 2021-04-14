using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UserThrottling
{
	public sealed class UserThrottlingDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProcessIsolationTracer, base.TraceContext, "ProcessIsolationDiscovery.DoWork: Started", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UserThrottling\\UserThrottlingDiscovery.cs", 59);
			string text = "MSExchange User Throttling\\Users Locked Out";
			foreach (UserThrottlingProtocolConfiguration userThrottlingProtocolConfiguration in UserThrottlingDiscovery.Protocols)
			{
				string protocol = userThrottlingProtocolConfiguration.Protocol;
				string process = userThrottlingProtocolConfiguration.Process;
				string escalationTeam = userThrottlingProtocolConfiguration.EscalationTeam;
				MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition(string.Format("{0}UsersLockedOutMonitor", protocol), PerformanceCounterNotificationItem.GenerateResultName(string.Concat(new string[]
				{
					text,
					"\\",
					process,
					"_",
					protocol
				})), ExchangeComponent.UserThrottling.Name, ExchangeComponent.UserThrottling, (double)userThrottlingProtocolConfiguration.LockedOutUsersAlertingThreshold, userThrottlingProtocolConfiguration.LockedOutUsersAlertingSampleCount, true);
				monitorDefinition.TargetResource = protocol;
				monitorDefinition.ServicePriority = 2;
				monitorDefinition.ScenarioDescription = "Validate UserThrottling health is not impacted by any issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition definition = EscalateResponder.CreateDefinition(string.Format("{0}UsersLockedOutEscalation", protocol), ExchangeComponent.UserThrottling.Name, monitorDefinition.Name, monitorDefinition.Name, protocol, ServiceHealthStatus.None, escalationTeam, Strings.UserThrottlingLockedOutUsersSubject(protocol), Strings.UserthtottlingLockedOutUsersMessage(protocol, userThrottlingProtocolConfiguration.LockedOutUsersAlertingThreshold, userThrottlingProtocolConfiguration.LockedOutUsersAlertingSampleCount), true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			}
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly List<UserThrottlingProtocolConfiguration> Protocols = new List<UserThrottlingProtocolConfiguration>
		{
			new UserThrottlingProtocolConfiguration("imap", "microsoft.exchange.imap4", ExchangeComponent.Imap.EscalationTeam),
			new UserThrottlingProtocolConfiguration("owa", "msexchangeowaapppool", ExchangeComponent.Owa.EscalationTeam),
			new UserThrottlingProtocolConfiguration("pop", "microsoft.exchange.pop3", ExchangeComponent.Pop.EscalationTeam),
			new UserThrottlingProtocolConfiguration("eas", "msexchangesyncapppool", ExchangeComponent.Eas.EscalationTeam),
			new UserThrottlingProtocolConfiguration("ews", "msexchangeservicesapppool", ExchangeComponent.Ews.EscalationTeam),
			new UserThrottlingProtocolConfiguration("powershell", "msexchangepowershellapppool", ExchangeComponent.Rps.EscalationTeam),
			new UserThrottlingProtocolConfiguration("rca", "microsoft.exchange.rpcclientaccess.service", ExchangeComponent.OutlookProtocol.EscalationTeam)
		};
	}
}
