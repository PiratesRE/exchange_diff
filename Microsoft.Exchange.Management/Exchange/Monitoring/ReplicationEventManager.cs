using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Monitoring
{
	internal class ReplicationEventManager : IEventManager
	{
		public ReplicationEventManager()
		{
			this.m_MomEvents = new Dictionary<int, ReplicationEvent>();
			this.m_AppLogSingleEvents = new Dictionary<int, Dictionary<string, ReplicationEvent>>();
		}

		public bool HasEvents()
		{
			return this.m_MomEvents.Count > 0 || this.m_AppLogSingleEvents.Count > 0;
		}

		public bool HasMomEvents()
		{
			return this.m_MomEvents.Count > 0;
		}

		public void LogEvent(int momEventId, string eventMessage)
		{
			ReplicationEvent replicationEvent;
			if (this.m_MomEvents.TryGetValue(momEventId, out replicationEvent))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<int>((long)this.GetHashCode(), "LogEvent(): Attempting to add another eventId {0}, having logged it already.", momEventId);
				replicationEvent.AddEvent(eventMessage);
				return;
			}
			MomEventInfo eventInfo = null;
			if (!ReplicationEventLookupTable.TryGetReplicationEventInfo(momEventId, out eventInfo))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<int>((long)this.GetHashCode(), "LogEvent(): Couldn't find MOM EventInfo for MOM Event ID {0}.", momEventId);
				return;
			}
			ExTraceGlobals.HealthChecksTracer.TraceDebug<int>((long)this.GetHashCode(), "LogEvent(): Attempting to add eventId {0} for the first time.", momEventId);
			replicationEvent = new ReplicationEvent(eventInfo);
			replicationEvent.AddEvent(eventMessage);
			this.m_MomEvents[momEventId] = replicationEvent;
		}

		public void LogEvents(CheckId checkId, ReplicationCheckResultEnum result, List<MessageInfo> messages)
		{
			ReplicationEventBaseInfo replicationEventBaseInfo = null;
			if (!ReplicationEventLookupTable.TryGetReplicationEventInfo(checkId, result, out replicationEventBaseInfo))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<CheckId, ReplicationCheckResultEnum>((long)this.GetHashCode(), "LogEvents(): Could not find ReplicationEventBaseInfo for Check '{0}' with Result '{1}'.", checkId, result);
				return;
			}
			if (replicationEventBaseInfo.EventType == ReplicationEventType.Null)
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<CheckId, ReplicationCheckResultEnum>((long)this.GetHashCode(), "LogEvents(): NullEventInfo encountered for check {0} and result {1}. No event will be logged.", checkId, result);
				return;
			}
			if (replicationEventBaseInfo.EventType != ReplicationEventType.MOM)
			{
				if (replicationEventBaseInfo.EventType == ReplicationEventType.AppLog)
				{
					bool flag = false;
					Dictionary<string, ReplicationEvent> dictionary = null;
					if (!this.m_AppLogSingleEvents.TryGetValue((int)checkId, out dictionary))
					{
						dictionary = new Dictionary<string, ReplicationEvent>();
						flag = true;
					}
					foreach (MessageInfo messageInfo in messages)
					{
						if (messageInfo.IsTransitioningState)
						{
							ReplicationEvent replicationEvent = new ReplicationEvent(replicationEventBaseInfo);
							replicationEvent.AddEvent(messageInfo.Message);
							dictionary[messageInfo.InstanceIdentity] = replicationEvent;
						}
					}
					if (flag)
					{
						this.m_AppLogSingleEvents[(int)checkId] = dictionary;
						return;
					}
				}
				else
				{
					DiagCore.RetailAssert(false, "Unhandled ReplicationEventType!", new object[0]);
				}
				return;
			}
			int momEventId = ((MomEventInfo)replicationEventBaseInfo).MomEventId;
			string eventMessage = this.BuildErrorMessageForMomEvent(messages);
			ReplicationEvent replicationEvent2;
			if (this.m_MomEvents.TryGetValue(momEventId, out replicationEvent2))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<int>((long)this.GetHashCode(), "LogEvent(): Attempting to add another eventId {0}, having logged it already.", momEventId);
				replicationEvent2.AddEvent(eventMessage);
				return;
			}
			ExTraceGlobals.HealthChecksTracer.TraceDebug<int>((long)this.GetHashCode(), "LogEvent(): Attempting to add eventId {0} for the first time.", momEventId);
			replicationEvent2 = new ReplicationEvent(replicationEventBaseInfo);
			replicationEvent2.AddEvent(eventMessage);
			this.m_MomEvents[momEventId] = replicationEvent2;
		}

		public void WriteMonitoringEvents(MonitoringData monitoringData, string momEventSource)
		{
			if (!this.HasEvents())
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "WriteMonitoringEvents(): No events have been logged. m_Events.Count = 0");
				return;
			}
			foreach (ReplicationEvent replicationEvent in this.m_MomEvents.Values)
			{
				MonitoringEvent monitoringEvent = replicationEvent.ConvertToMonitoringEvent(momEventSource, true);
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "WriteMonitoringEvents(): Adding Monitoring Event with message: '{0}'", monitoringEvent.EventMessage);
				monitoringData.Events.Add(monitoringEvent);
			}
			foreach (Dictionary<string, ReplicationEvent> dictionary in this.m_AppLogSingleEvents.Values)
			{
				foreach (KeyValuePair<string, ReplicationEvent> keyValuePair in dictionary)
				{
					keyValuePair.Value.PublishToApplicationEventLog(true, keyValuePair.Key);
				}
			}
		}

		private string BuildErrorMessageForMomEvent(List<MessageInfo> messages)
		{
			if (messages == null || messages.Count == 0)
			{
				return string.Empty;
			}
			string checkTitle = messages[0].CheckTitle;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}:", checkTitle);
			foreach (MessageInfo messageInfo in messages)
			{
				stringBuilder.AppendFormat("{0}\t{1}", Environment.NewLine, messageInfo.Message);
			}
			string text = stringBuilder.ToString();
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "BuildErrorMessageForMomEvent(): Message after running check '{0}' is:{1}", checkTitle, (!string.IsNullOrEmpty(text)) ? text.Replace(Environment.NewLine, "; ") : "<blank>");
			return text;
		}

		private Dictionary<int, ReplicationEvent> m_MomEvents;

		private Dictionary<int, Dictionary<string, ReplicationEvent>> m_AppLogSingleEvents;
	}
}
