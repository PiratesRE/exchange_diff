using System;
using Microsoft.Exchange.Diagnostics.Service.Common;
using Microsoft.ExLogAnalyzer;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs
{
	public class EventLogger : EventLogger
	{
		protected override bool InternalTrigger(string triggerName, params object[] args)
		{
			EventLogger.EventData eventData = base.Tuples[triggerName];
			ExEventLog.EventTuple tuple = eventData.Tuple;
			TriggerHandler.TriggerData triggerData = TriggerHandler.Triggers[triggerName];
			string type;
			ResultSeverityLevel severity;
			if ((type = triggerData.Type) != null)
			{
				if (type == "Warning")
				{
					severity = ResultSeverityLevel.Warning;
					goto IL_53;
				}
				if (type == "Error")
				{
					severity = ResultSeverityLevel.Error;
					goto IL_53;
				}
			}
			severity = ResultSeverityLevel.Informational;
			IL_53:
			string component = triggerName;
			if (!string.IsNullOrEmpty(eventData.Component))
			{
				component = string.Format(eventData.Component, args);
			}
			string notificationReason = string.Empty;
			if (!string.IsNullOrEmpty(eventData.Tag))
			{
				notificationReason = string.Format(eventData.Tag, args);
			}
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.Eds.Name, component, notificationReason, severity);
			eventNotificationItem.Message = string.Format(triggerData.Format, args);
			if (!string.IsNullOrEmpty(eventData.Exception))
			{
				eventNotificationItem.Exception = string.Format(eventData.Exception, args);
			}
			eventNotificationItem.Publish(false);
			return base.InternalTrigger(triggerName, args);
		}
	}
}
