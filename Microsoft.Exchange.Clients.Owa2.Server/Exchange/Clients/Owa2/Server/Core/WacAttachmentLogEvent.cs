using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class WacAttachmentLogEvent : ILogEvent
	{
		public WacAttachmentLogEvent(string msg) : this(msg, null)
		{
		}

		public WacAttachmentLogEvent(string msg, Exception ex)
		{
			this.message = msg;
			this.handledException = ex;
		}

		public string EventId
		{
			get
			{
				return "WacAttachmentLogEvent";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			ICollection<KeyValuePair<string, object>> collection = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("MSG", this.message)
			};
			if (this.handledException != null)
			{
				collection.Add(new KeyValuePair<string, object>("Ex", this.handledException.Message));
				collection.Add(new KeyValuePair<string, object>("ExS", this.handledException.StackTrace));
				Exception innerException = this.handledException.InnerException;
				int num = 1;
				while (innerException != null && num <= 10)
				{
					collection.Add(new KeyValuePair<string, object>("Ex" + num, innerException.Message ?? "Not specified"));
					collection.Add(new KeyValuePair<string, object>("ExS" + num, innerException.StackTrace ?? "Not specified"));
					innerException = innerException.InnerException;
					num++;
				}
			}
			return collection;
		}

		private readonly string message;

		private readonly Exception handledException;
	}
}
