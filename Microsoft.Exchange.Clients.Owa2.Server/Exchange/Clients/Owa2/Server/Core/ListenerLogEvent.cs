using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ListenerLogEvent : ILogEvent
	{
		public ListenerLogEvent(ListenerEventType eventType)
		{
			this.EventType = eventType;
		}

		public string EventId
		{
			get
			{
				return "Listener";
			}
		}

		public ListenerEventType EventType { get; set; }

		public string OriginationServer { get; set; }

		public Exception HandledException { get; set; }

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			ICollection<KeyValuePair<string, object>> collection = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("ET", this.EventType.ToString())
			};
			switch (this.EventType)
			{
			case ListenerEventType.Listen:
				collection.Add(new KeyValuePair<string, object>("OS", this.OriginationServer));
				break;
			case ListenerEventType.ListenFailed:
				collection.Add(new KeyValuePair<string, object>("OS", this.OriginationServer));
				if (this.HandledException != null)
				{
					collection.Add(new KeyValuePair<string, object>("Ex", this.HandledException.Message));
					collection.Add(new KeyValuePair<string, object>("ExS", this.HandledException.StackTrace));
					Exception innerException = this.HandledException.InnerException;
					int num = 1;
					while (innerException != null && num <= 10)
					{
						collection.Add(new KeyValuePair<string, object>("Ex" + num, innerException.Message ?? "Not specified"));
						collection.Add(new KeyValuePair<string, object>("ExS" + num, innerException.StackTrace ?? "Not specified"));
						innerException = innerException.InnerException;
						num++;
					}
				}
				break;
			}
			return collection;
		}
	}
}
