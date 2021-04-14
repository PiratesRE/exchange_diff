using System;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Security
{
	public class PowerShellProfileResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateEscalateResponderDefinition(string monitorName, string serviceName, string mask, string targetResource, string escalationTeam, bool enabled, NotificationServiceClass notificationType)
		{
			string name = monitorName + "EscalateResponder";
			return EscalateResponder.CreateDefinition(name, serviceName, monitorName, mask, targetResource, ServiceHealthStatus.None, escalationTeam, Strings.PowerShellProfileEscalationSubject, PowerShellProfileResponder.ConstructEscalationMessage(), enabled, notificationType, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		private static string ConstructEscalationMessage()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Strings.PowerShellProfileEscalationMessage + "<br/>");
			stringBuilder.Append("<b>{{Probe.ResultName}}<hr/>");
			stringBuilder.Append("Error: <pre>{{Probe.Error}}</pre><br/>");
			stringBuilder.Append("Exception: <pre>{{Probe.Exception}}</pre><br/>");
			stringBuilder.Append("StateAttribute11: <pre>{{Probe.StateAttribute11}}</pre><br/>");
			stringBuilder.Append("ExecutionStartTime: <pre>{{Probe.ExecutionStartTime}}</pre>");
			stringBuilder.Append("ExecutionStartTime: <pre>{{Probe.ExecutionEndTime}}</pre>");
			return string.Format(stringBuilder.ToString(), new object[0]);
		}

		public static ResponderDefinition CreateFileRemoveResponderDefinition(string monitorName, string serviceName, string mask, string targetResource, int recurrenceIntervalSeconds, int timeoutSeconds, int waitIntervalSeconds, bool enabled)
		{
			string name = monitorName + "FileRemoveResponder";
			return new ResponderDefinition
			{
				AssemblyPath = PowerShellProfileResponder.AssemblyPath,
				TypeName = PowerShellProfileResponder.TypeName,
				ServiceName = serviceName,
				Enabled = enabled,
				Name = name,
				AlertMask = mask,
				AlertTypeId = monitorName,
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = timeoutSeconds,
				WaitIntervalSeconds = waitIntervalSeconds
			};
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "PowershellProfile FileRemoveResponder responder successfully executed.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\PowerShellProfileResponder.cs", 134);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(PowerShellProfileResponder).FullName;
	}
}
