using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PusherLogEvent : ILogEvent
	{
		public PusherLogEvent(PusherEventType eventType)
		{
			this.EventType = eventType;
		}

		public string EventId
		{
			get
			{
				return "Pusher";
			}
		}

		public PusherEventType EventType { get; set; }

		public string OriginationUserContextKey { get; set; }

		public int DestinationCount { get; set; }

		public string Destination { get; set; }

		public int PayloadCount { get; set; }

		public bool OverLimit { get; set; }

		public int InTransitCount { get; set; }

		public Exception HandledException { get; set; }

		public int ThreadId { get; set; }

		public int TaskCount { get; set; }

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			ICollection<KeyValuePair<string, object>> collection = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("ET", this.EventType.ToString())
			};
			switch (this.EventType)
			{
			case PusherEventType.Distribute:
				collection.Add(new KeyValuePair<string, object>("OCK", this.OriginationUserContextKey));
				collection.Add(new KeyValuePair<string, object>("DC", this.DestinationCount));
				collection.Add(new KeyValuePair<string, object>("PC", this.PayloadCount));
				break;
			case PusherEventType.Push:
				collection.Add(new KeyValuePair<string, object>("D", this.Destination));
				collection.Add(new KeyValuePair<string, object>("PC", this.PayloadCount));
				break;
			case PusherEventType.PushFailed:
				collection.Add(new KeyValuePair<string, object>("D", this.Destination));
				this.AddExceptionData(collection);
				break;
			case PusherEventType.ConcurrentLimit:
				collection.Add(new KeyValuePair<string, object>("OL", this.OverLimit.ToString()));
				collection.Add(new KeyValuePair<string, object>("ITC", this.InTransitCount));
				break;
			case PusherEventType.PusherThreadStart:
				collection.Add(new KeyValuePair<string, object>("TI", this.ThreadId));
				break;
			case PusherEventType.PusherThreadCleanup:
				collection.Add(new KeyValuePair<string, object>("TI", this.ThreadId));
				collection.Add(new KeyValuePair<string, object>("TC", this.TaskCount));
				break;
			case PusherEventType.PusherThreadEnd:
				collection.Add(new KeyValuePair<string, object>("TI", this.ThreadId));
				this.AddExceptionData(collection);
				break;
			}
			return collection;
		}

		private void AddExceptionData(ICollection<KeyValuePair<string, object>> eventData)
		{
			if (this.HandledException != null)
			{
				eventData.Add(new KeyValuePair<string, object>("Ex", this.HandledException.Message));
				eventData.Add(new KeyValuePair<string, object>("ExS", this.HandledException.StackTrace));
				Exception innerException = this.HandledException.InnerException;
				int num = 1;
				while (innerException != null && num <= 10)
				{
					eventData.Add(new KeyValuePair<string, object>("Ex" + num, innerException.Message ?? "Not specified"));
					eventData.Add(new KeyValuePair<string, object>("ExS" + num, innerException.StackTrace ?? "Not specified"));
					innerException = innerException.InnerException;
					num++;
				}
			}
		}
	}
}
