using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal class OwaAppStartLogEvent : ILogEvent
	{
		public OwaAppStartLogEvent(double startDuration)
		{
			this.startDuration = (int)startDuration;
		}

		public string EventId
		{
			get
			{
				return "OwaAppStart";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return new Dictionary<string, object>
			{
				{
					"ST",
					this.startDuration
				}
			};
		}

		private readonly int startDuration;
	}
}
