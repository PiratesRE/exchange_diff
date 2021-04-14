using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Hygiene.Data.MessageTrace;

namespace Microsoft.Exchange.Hygiene.Data.SystemProbe
{
	internal sealed class SystemProbeData
	{
		public static List<Guid> GetProbes(DateTimeOffset start, DateTimeOffset end)
		{
			MessageTraceSession messageTraceSession = new MessageTraceSession();
			MessageTrace[] array = messageTraceSession.FindPagedTrace(SystemProbeConstants.TenantID, start.DateTime, end.DateTime, null, null, null, null, null, 0, -1);
			List<Guid> list = new List<Guid>(array.Length);
			foreach (MessageTrace messageTrace in array)
			{
				list.Add(messageTrace.ExMessageId);
			}
			return list;
		}

		public static List<object> GetProbeEvents(Guid probe)
		{
			MessageTraceSession messageTraceSession = new MessageTraceSession();
			MessageTrace messageTrace = messageTraceSession.Read(SystemProbeConstants.TenantID, probe);
			List<object> events = new List<object>();
			if (messageTrace != null)
			{
				messageTrace.Events.ForEach(delegate(MessageEvent e)
				{
					events.Add(SystemProbeData.CreateEventRecord(e));
				});
			}
			return events;
		}

		private static SystemProbeEvent CreateEventRecord(MessageEvent messageEvent)
		{
			return new SystemProbeEvent
			{
				MessageId = messageEvent.ExMessageId,
				EventId = messageEvent.EventId,
				TimeStamp = messageEvent.TimeStamp,
				ServerHostName = messageEvent.GetExtendedProperty("SysProbe", "Server").PropertyValueString,
				ComponentName = messageEvent.GetExtendedProperty("SysProbe", "Component").PropertyValueString,
				EventStatus = (SystemProbeEvent.Status)Enum.Parse(typeof(SystemProbeEvent.Status), messageEvent.GetExtendedProperty("SysProbe", "Status").PropertyValueString, true),
				EventMessage = messageEvent.GetExtendedProperty("SysProbe", "MessageBlob").PropertyValueBlob.Value
			};
		}
	}
}
