using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	internal class EscalateResponderWithCustomMessage : EscalateResponder
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Urgent, int minimumSecondsBetweenEscalates = 14400, string dailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59")
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, escalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, enabled, notificationServiceClass, minimumSecondsBetweenEscalates, string.Empty, false);
			responderDefinition.AssemblyPath = EscalateResponderWithCustomMessage.AssemblyPath;
			responderDefinition.TypeName = EscalateResponderWithCustomMessage.TypeName;
			return responderDefinition;
		}

		internal override void GetEscalationSubjectAndMessage(MonitorResult monitorResult, out string escalationSubject, out string escalationMessage, bool rethrow = false, Action<ResponseMessageReader> textGeneratorModifier = null)
		{
			escalationSubject = base.Definition.EscalationSubject;
			escalationMessage = this.GetCustomMessage();
			if (string.IsNullOrEmpty(escalationMessage))
			{
				escalationMessage = base.Definition.EscalationMessage;
				base.GetEscalationSubjectAndMessage(monitorResult, out escalationSubject, out escalationMessage, rethrow, textGeneratorModifier);
				return;
			}
			lock (EscalateResponderHelper.CustomMessageDictionaryLock)
			{
				EscalateResponderHelper.AlertMaskToCustomMessageMap.Remove(base.Definition.AlertMask);
			}
		}

		private string GetCustomMessage()
		{
			EscalateResponderHelper.AdditionalMessageContainer additionalMessageContainer = new EscalateResponderHelper.AdditionalMessageContainer(DateTime.MinValue, string.Empty);
			string result = string.Empty;
			DateTime t = DateTime.UtcNow.AddMinutes(-15.0);
			try
			{
				lock (EscalateResponderHelper.CustomMessageDictionaryLock)
				{
					EscalateResponderHelper.AlertMaskToCustomMessageMap.TryGetValue(base.Definition.AlertMask, out additionalMessageContainer);
				}
				if (!string.IsNullOrEmpty(additionalMessageContainer.additionalMessage) && additionalMessageContainer.updateTime >= t)
				{
					result = additionalMessageContainer.additionalMessage;
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Failed to add message from file: {0}", ex.ToString(), null, "GetCustomMessage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Responders\\EscalateResponderWithCustomMessage.cs", 155);
			}
			return result;
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(EscalateResponderWithCustomMessage).FullName;
	}
}
