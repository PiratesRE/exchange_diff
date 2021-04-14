using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.ELC;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ELC
{
	public sealed class ELCDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.ELCAssistantTracer, base.TraceContext, "ELCDiscovery.DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ELC\\ELCDiscovery.cs", 209);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				this.WriteTrace("ELCProcessing.DoWork: Skipping workitem generation since not on a mailbox server.");
				return;
			}
			this.CreateTransientContext();
			this.CreatePermanentContext();
			this.CreateMailboxSLAContext();
			this.CreateDumpsterWarningQuota();
		}

		private void CreateTransientContext()
		{
			bool flag = bool.Parse(base.Definition.Attributes["ELCTransientMonitorEnabled"]);
			int interval = (int)TimeSpan.Parse(base.Definition.Attributes["ELCTransientMonitorInterval"]).TotalSeconds;
			int recurrence = (int)TimeSpan.Parse(base.Definition.Attributes["ELCTransientMonitorRecurranceInterval"]).TotalSeconds;
			int threshold = int.Parse(base.Definition.Attributes["ELCTransientMonitorMinErrorCount"]);
			string name = ExchangeComponent.Compliance.Name;
			string monitorSampleMask = NotificationItem.GenerateResultName(name, "ELCComponent_Transient", null);
			if (flag)
			{
				this.CreateXFailureMonitor("ELCTransientMonitor", monitorSampleMask, interval, recurrence, threshold);
				string alertMask = string.Format("{0}/{1}", "ELCTransientMonitor", ExchangeComponent.Compliance.Name);
				this.CreateResponder("ELCTransientEscalateResponder", "ELCTransientMonitor", alertMask, Strings.ELCTransientEscalationSubject, Strings.ELCExceptionEscalationMessage);
			}
		}

		private void CreatePermanentContext()
		{
			bool flag = bool.Parse(base.Definition.Attributes["ELCPermanentMonitorEnabled"]);
			int interval = (int)TimeSpan.Parse(base.Definition.Attributes["ELCPermanentMonitorInterval"]).TotalSeconds;
			int recurrence = (int)TimeSpan.Parse(base.Definition.Attributes["ELCPermanentMonitorRecurranceInterval"]).TotalSeconds;
			int threshold = int.Parse(base.Definition.Attributes["ELCPermanentMonitorMinErrorCount"]);
			string name = ExchangeComponent.Compliance.Name;
			string monitorSampleMask = NotificationItem.GenerateResultName(name, "ELCComponent_Permanent", null);
			if (flag)
			{
				this.CreateXFailureMonitor("ELCPermanentMonitor", monitorSampleMask, interval, recurrence, threshold);
				string alertMask = string.Format("{0}/{1}", "ELCPermanentMonitor", ExchangeComponent.Compliance.Name);
				this.CreateResponder("ELCPermanentEscalateResponder", "ELCPermanentMonitor", alertMask, Strings.ELCPermanentEscalationSubject, Strings.ELCExceptionEscalationMessage);
			}
		}

		private void CreateMailboxSLAContext()
		{
			bool flag = bool.Parse(base.Definition.Attributes["ELCMailboxSLAMonitorEnabled"]);
			int interval = (int)TimeSpan.Parse(base.Definition.Attributes["ELCMailboxSLAMonitorInterval"]).TotalSeconds;
			int recurrence = (int)TimeSpan.Parse(base.Definition.Attributes["ELCMailboxSLAMonitorRecurranceInterval"]).TotalSeconds;
			if (ExEnvironment.IsTest)
			{
				recurrence = (int)TimeSpan.FromSeconds(10.0).TotalSeconds;
			}
			int threshold = int.Parse(base.Definition.Attributes["ELCMailboxSLAMonitorMinErrorCount"]);
			string name = ExchangeComponent.Compliance.Name;
			string monitorSampleMask = NotificationItem.GenerateResultName(name, "ELCComponent_LastSuccessTooLongAgo", null);
			if (flag)
			{
				this.CreateXFailureMonitor("ELCMailboxSLAMonitor", monitorSampleMask, interval, recurrence, threshold);
				string alertMask = string.Format("{0}/{1}", "ELCMailboxSLAMonitor", ExchangeComponent.Compliance.Name);
				this.CreateResponder("ELCMailboxSLAEscalateResponder", "ELCMailboxSLAMonitor", alertMask, Strings.ELCMailboxSLAEscalationSubject, Strings.ELCMailboxSLAEscalationMessage);
			}
		}

		private void CreateDumpsterWarningQuota()
		{
			bool flag = bool.Parse(base.Definition.Attributes["ELCDumpsterWarningExceededMonitorEnabled"]);
			int interval = (int)TimeSpan.Parse(base.Definition.Attributes["ELCDumpsterWarningExceededMonitorInterval"]).TotalSeconds;
			int recurrence = (int)TimeSpan.Parse(base.Definition.Attributes["ELCDumpsterWarningExceededMonitorRecurranceInterval"]).TotalSeconds;
			int threshold = int.Parse(base.Definition.Attributes["ELCDumpsterWarningExceededMonitorMinErrorCount"]);
			string name = ExchangeComponent.Compliance.Name;
			string monitorSampleMask = NotificationItem.GenerateResultName(name, "ELCComponent_DumpsterWarningQuota", null);
			if (flag)
			{
				this.CreateXFailureMonitor("ELCDumpsterWarningExceededMonitor", monitorSampleMask, interval, recurrence, threshold);
				string alertMask = string.Format("{0}/{1}", "ELCDumpsterWarningExceededMonitor", ExchangeComponent.Compliance.Name);
				this.CreateResponder("ELCDumpsterWarningExceededResponder", "ELCDumpsterWarningExceededMonitor", alertMask, Strings.ELCDumpsterWarningEscalationSubject, Strings.ELCDumpsterEscalationMessage);
			}
		}

		private void CreateArchiveDumpsterWarningQuota()
		{
			bool flag = bool.Parse(base.Definition.Attributes["ELCArchiveDumpsterWarningExceededMonitorEnabled"]);
			int interval = (int)TimeSpan.Parse(base.Definition.Attributes["ELCArchiveDumpsterWarningExceededMonitorInterval"]).TotalSeconds;
			int recurrence = (int)TimeSpan.Parse(base.Definition.Attributes["ELCArchiveDumpsterWarningExceededMonitorRecurranceInterval"]).TotalSeconds;
			int threshold = int.Parse(base.Definition.Attributes["ELCArchiveDumpsterWarningExceededMonitorMinErrorCount"]);
			string name = ExchangeComponent.Compliance.Name;
			string monitorSampleMask = NotificationItem.GenerateResultName(name, "ELCComponent_ArchiveWarningQuota", null);
			if (flag)
			{
				this.CreateXFailureMonitor("ELCArchiveDumpsterWarningExceededMonitor", monitorSampleMask, interval, recurrence, threshold);
				string alertMask = string.Format("{0}/{1}", "ELCArchiveDumpsterWarningExceededMonitor", ExchangeComponent.Compliance.Name);
				this.CreateResponder("ELCArchiveDumpsterWarningExceededResponder", "ELCArchiveDumpsterWarningExceededMonitor", alertMask, Strings.ELCArchiveDumpsterWarningEscalationSubject, Strings.ELCArchiveDumpsterEscalationMessage);
			}
		}

		private void ExceededDumpserWarningQuota()
		{
			bool.Parse(base.Definition.Attributes["ELCDumpsterWarningExceededMonitorEnabled"]);
		}

		private void CreateXFailureMonitor(string monitorName, string monitorSampleMask, int interval, int recurrence, int threshold)
		{
			TimeSpan timeSpan = TimeSpan.Parse(base.Definition.Attributes["ELCUnrecoverableTransitionTimeSpan"]);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(monitorName, monitorSampleMask, ExchangeComponent.Compliance.Name, ExchangeComponent.Compliance, interval, recurrence, threshold, true);
			monitorDefinition.TargetResource = monitorDefinition.ServiceName;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)timeSpan.TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			this.WriteTrace(string.Format("ELCProcessing: Created Monitor Definition {0}.", monitorName));
		}

		private void CreateResponder(string responderName, string monitorName, string alertMask, string escalationSubject, string escalationMessage)
		{
			ResponderDefinition definition = EscalateResponder.CreateDefinition(responderName, ExchangeComponent.Compliance.Name, monitorName, alertMask, ExchangeComponent.Compliance.Name, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Compliance.EscalationTeam, escalationSubject, escalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			this.WriteTrace(string.Format("ELCProcessing: Created EscalateResponder Definition {0}.", responderName));
		}

		private void WriteTrace(string message)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ELCAssistantTracer, base.TraceContext, message, null, "WriteTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ELC\\ELCDiscovery.cs", 529);
		}

		private const string TransientMonitorEnabledAttributeName = "ELCTransientMonitorEnabled";

		private const string TransientMonitorIntervalAttributeName = "ELCTransientMonitorInterval";

		private const string TransientMonitorRecurranceIntervalAttributeName = "ELCTransientMonitorRecurranceInterval";

		private const string TransientMonitorMinErrorCountAttributeName = "ELCTransientMonitorMinErrorCount";

		private const string TransientMonitorName = "ELCTransientMonitor";

		private const string TransientResponderName = "ELCTransientEscalateResponder";

		private const string PermanentMonitorEnabledAttributeName = "ELCPermanentMonitorEnabled";

		private const string PermanentMonitorIntervalAttributeName = "ELCPermanentMonitorInterval";

		private const string PermanentMonitorRecurranceIntervalAttributeName = "ELCPermanentMonitorRecurranceInterval";

		private const string PermanentMonitorMinErrorCountAttributeName = "ELCPermanentMonitorMinErrorCount";

		private const string PermanentMonitorName = "ELCPermanentMonitor";

		private const string PermanentResponderName = "ELCPermanentEscalateResponder";

		private const string MailboxSLAMonitorEnabledAttributeName = "ELCMailboxSLAMonitorEnabled";

		private const string MailboxSLAMonitorIntervalAttributeName = "ELCMailboxSLAMonitorInterval";

		private const string MailboxSLAMonitorRecurranceIntervalAttributeName = "ELCMailboxSLAMonitorRecurranceInterval";

		private const string MailboxSLAMonitorMinErrorCountAttributeName = "ELCMailboxSLAMonitorMinErrorCount";

		private const string MailboxSLAMonitorName = "ELCMailboxSLAMonitor";

		private const string MailboxSLAResponderName = "ELCMailboxSLAEscalateResponder";

		private const string ELCDumpsterWarningExceededMonitorEnabledName = "ELCDumpsterWarningExceededMonitorEnabled";

		private const string ELCDumpsterWarningExceededMonitorIntervalName = "ELCDumpsterWarningExceededMonitorInterval";

		private const string ELCDumpsterWarningExceededMonitorRecurranceIntervalName = "ELCDumpsterWarningExceededMonitorRecurranceInterval";

		private const string ELCDumpsterWarningExceededMonitorMinErrorCountName = "ELCDumpsterWarningExceededMonitorMinErrorCount";

		private const string DumpsterWarningExceededMonitorName = "ELCDumpsterWarningExceededMonitor";

		private const string DumpsterWarningExceededResponderName = "ELCDumpsterWarningExceededResponder";

		private const string ELCArchiveDumpsterWarningExceededMonitorEnabledName = "ELCArchiveDumpsterWarningExceededMonitorEnabled";

		private const string ELCArchiveDumpsterWarningExceededMonitorIntervalName = "ELCArchiveDumpsterWarningExceededMonitorInterval";

		private const string ELCArchiveDumpsterWarningExceededMonitorRecurranceIntervalName = "ELCArchiveDumpsterWarningExceededMonitorRecurranceInterval";

		private const string ELCArchiveDumpsterWarningExceededMonitorMinErrorCountName = "ELCArchiveDumpsterWarningExceededMonitorMinErrorCount";

		private const string ArchiveDumpsterWarningExceededMonitorName = "ELCArchiveDumpsterWarningExceededMonitor";

		private const string ArchiveDumpsterWarningExceededResponderName = "ELCArchiveDumpsterWarningExceededResponder";

		private const string UnrecoverableTimeAttributeName = "ELCUnrecoverableTransitionTimeSpan";

		private static TracingContext traceContext = new TracingContext();
	}
}
