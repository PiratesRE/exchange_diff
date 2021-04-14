using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class GenericEventWriteResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string eventLogName, string eventSource, long eventId, string eventDataCsv, EventLogEntryType eventType = EventLogEntryType.Information, int eventTaskId = 0, string throttleGroupName = null)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			responderDefinition.TypeName = typeof(GenericEventWriteResponder).FullName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.WaitIntervalSeconds = 30;
			responderDefinition.TimeoutSeconds = 100;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Enabled = true;
			responderDefinition.Attributes["eventLogName"] = eventLogName.Trim();
			responderDefinition.Attributes["eventSource"] = eventSource.Trim();
			responderDefinition.Attributes["eventId"] = eventId.ToString();
			responderDefinition.Attributes["eventDataCsv"] = eventDataCsv.Trim();
			responderDefinition.Attributes["eventType"] = eventType.ToString();
			responderDefinition.Attributes["eventTaskId"] = eventTaskId.ToString();
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			string text = base.Definition.Attributes["eventLogName"];
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new HighAvailabilityMAResponderException("EventLogName is either NULL or Empty!");
			}
			base.Result.StateAttribute1 = string.Format("EventLogName = {0}", text);
			string text2 = base.Definition.Attributes["eventSource"];
			if (string.IsNullOrWhiteSpace(text2))
			{
				throw new HighAvailabilityMAResponderException("EventSource is either NULL or Empty!");
			}
			base.Result.StateAttribute2 = string.Format("EventSource = {0}", text2);
			long num = -1L;
			if (!long.TryParse(base.Definition.Attributes["eventId"], out num))
			{
				throw new HighAvailabilityMAResponderException(string.Format("EventId is not in correct format! Value={0}", (base.Definition.Attributes["eventId"] == null) ? "NULL" : base.Definition.Attributes["eventId"].ToString()));
			}
			base.Result.StateAttribute3 = string.Format("EventId = {0}", num);
			int num2 = 0;
			if (!int.TryParse(base.Definition.Attributes["eventTaskId"], out num2))
			{
				throw new HighAvailabilityMAResponderException(string.Format("EventTaskId is not in correct format! Value={0}", (base.Definition.Attributes["eventTaskId"] == null) ? "NULL" : base.Definition.Attributes["eventTaskId"].ToString()));
			}
			base.Result.StateAttribute4 = string.Format("EventTaskId = {0}", num2);
			object[] values;
			if (!string.IsNullOrWhiteSpace(base.Definition.Attributes["eventDataCsv"]))
			{
				values = base.Definition.Attributes["eventDataCsv"].Split(new char[]
				{
					','
				});
				base.Result.StateAttribute5 = string.Format("EventDataCsv = {0}", base.Definition.Attributes["eventDataCsv"]);
			}
			else
			{
				values = new object[0];
				base.Result.StateAttribute5 = "EventDataCsv = NULL";
			}
			EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
			if (!string.IsNullOrWhiteSpace(base.Definition.Attributes["eventType"]))
			{
				eventLogEntryType = (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), base.Definition.Attributes["eventType"]);
				ResponderResult result = base.Result;
				result.StateAttribute4 += string.Format("EventType = {0}", eventLogEntryType);
			}
			using (EventLog eventLog = new EventLog(text))
			{
				eventLog.Source = text2;
				EventInstance instance = new EventInstance(num, num2, eventLogEntryType);
				eventLog.WriteEvent(instance, values);
			}
		}
	}
}
