using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PeopleIKnowRowNotificationLogEvent : ILogEvent
	{
		internal PeopleIKnowRowNotificationLogEvent(double browsePeopleTime, int personaCount)
		{
			this.browsePeopleTime = browsePeopleTime;
			this.personaCount = personaCount;
		}

		public string EventId
		{
			get
			{
				return "PeopleIKnowRowNotification";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("PIK.BPT", this.browsePeopleTime),
				new KeyValuePair<string, object>("PIK.PC", this.personaCount)
			};
		}

		private readonly double browsePeopleTime;

		private readonly int personaCount;
	}
}
