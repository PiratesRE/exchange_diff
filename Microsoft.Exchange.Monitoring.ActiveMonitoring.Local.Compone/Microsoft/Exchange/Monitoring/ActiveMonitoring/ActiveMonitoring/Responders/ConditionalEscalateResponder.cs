using System;
using System.Reflection;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class ConditionalEscalateResponder : EscalateResponder
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubject, string escalationMessage, string conditionalEscalateProperty, NotificationServiceClass notificationServiceClass, bool enabled = true, int recurrenceIntervalSeconds = 300)
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, escalationTeam, escalationSubject, escalationMessage, enabled, notificationServiceClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.AssemblyPath = ConditionalEscalateResponder.AssemblyPath;
			responderDefinition.TypeName = ConditionalEscalateResponder.TypeName;
			responderDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			responderDefinition.Attributes[ConditionalEscalateResponder.ConditionalPropertyString] = conditionalEscalateProperty;
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			ProbeResult lastFailedProbeResult = WorkItemResultHelper.GetLastFailedProbeResult(this, base.Broker, cancellationToken);
			string text;
			if (lastFailedProbeResult != null && base.Definition.Attributes.TryGetValue(ConditionalEscalateResponder.ConditionalPropertyString, out text) && !string.IsNullOrWhiteSpace(text))
			{
				Type type = lastFailedProbeResult.GetType();
				PropertyInfo property = type.GetProperty(text);
				if (property != null)
				{
					string text2 = (string)property.GetValue(lastFailedProbeResult, null);
					string value;
					bool flag;
					if (!string.IsNullOrWhiteSpace(text2) && base.Definition.Attributes.TryGetValue(text2, out value) && !string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out flag) && flag)
					{
						base.Result.StateAttribute4 = string.Format("Suppressing escalation for exception or criteria {0}", text2);
						return;
					}
				}
			}
			base.Result.StateAttribute4 = string.Format("Unable to suppress, escalating", new object[0]);
			base.DoResponderWork(cancellationToken);
		}

		internal static readonly string ConditionalPropertyString = "ConditionalProperty";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ConditionalEscalateResponder).FullName;
	}
}
