using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Monitoring
{
	internal class ReplicationEvent
	{
		public ReplicationEvent(ReplicationEventBaseInfo eventInfo)
		{
			this.m_EventInfo = eventInfo;
			this.m_EventMessages = new List<string>();
		}

		public bool IsMomEvent
		{
			get
			{
				return this.m_EventInfo.EventType == ReplicationEventType.MOM;
			}
		}

		public void AddEvent(string eventMessage)
		{
			this.m_EventMessages.Add(eventMessage);
		}

		public string GetEventMessage(bool appendMachineInfo)
		{
			if (this.m_EventMessages.Count == 0)
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug((long)this.GetHashCode(), "GetEventMessageForMom(): No events were logged. m_EventMessages.Count = 0");
				return null;
			}
			if (!this.m_EventInfo.ShouldBeRolledUp && this.IsMomEvent)
			{
				return this.m_EventMessages[0];
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (this.m_EventMessages.Count == 1)
			{
				stringBuilder.AppendLine(this.m_EventMessages[0]);
			}
			else
			{
				foreach (string value in this.m_EventMessages)
				{
					stringBuilder.AppendLine(value);
				}
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			if (!appendMachineInfo)
			{
				StringBuilder stringBuilder3 = stringBuilder2;
				string format = "{0}{1}{2}";
				LocalizedString? baseEventMessage = this.m_EventInfo.BaseEventMessage;
				stringBuilder3.AppendFormat(format, (baseEventMessage != null) ? baseEventMessage.GetValueOrDefault() : string.Empty, Environment.NewLine, stringBuilder.ToString());
			}
			else
			{
				StringBuilder stringBuilder4 = stringBuilder2;
				string format2 = "{0} {1}{2}{3}";
				object[] array = new object[4];
				object[] array2 = array;
				int num = 0;
				LocalizedString? baseEventMessage2 = this.m_EventInfo.BaseEventMessage;
				array2[num] = ((baseEventMessage2 != null) ? baseEventMessage2.GetValueOrDefault() : string.Empty);
				array[1] = TestReplicationHealth.GetMachineConfigurationString(ReplicationCheckGlobals.ServerConfiguration);
				array[2] = Environment.NewLine;
				array[3] = stringBuilder.ToString();
				stringBuilder4.AppendFormat(format2, array);
			}
			return stringBuilder2.ToString();
		}

		public MonitoringEvent ConvertToMonitoringEvent(string momEventSource)
		{
			return this.ConvertToMonitoringEvent(momEventSource, false);
		}

		public MonitoringEvent ConvertToMonitoringEvent(string momEventSource, bool appendMachineInfo)
		{
			if (!this.IsMomEvent)
			{
				return null;
			}
			string eventMessage = this.GetEventMessage(appendMachineInfo);
			if (eventMessage == null)
			{
				return null;
			}
			MomEventInfo momEventInfo = (MomEventInfo)this.m_EventInfo;
			return new MonitoringEvent(momEventSource, momEventInfo.MomEventId, momEventInfo.MomEventType, eventMessage);
		}

		public void PublishToApplicationEventLog(bool appendMachineInfo, string instanceIdentity)
		{
			if (this.m_EventInfo.EventType != ReplicationEventType.AppLog)
			{
				return;
			}
			string eventMessage = this.GetEventMessage(appendMachineInfo);
			ApplicationEventInfo applicationEventInfo = (ApplicationEventInfo)this.m_EventInfo;
			if (string.IsNullOrEmpty(eventMessage))
			{
				applicationEventInfo.EventTuple.LogEvent(instanceIdentity, new object[]
				{
					instanceIdentity
				});
				return;
			}
			applicationEventInfo.EventTuple.LogEvent(instanceIdentity, new object[]
			{
				instanceIdentity,
				eventMessage
			});
		}

		private const string CompleteEventMessageFormatString = "{0}{1}{2}";

		private const string CompleteEventMachineInfoFormatString = "{0} {1}{2}{3}";

		private readonly ReplicationEventBaseInfo m_EventInfo;

		private List<string> m_EventMessages;
	}
}
