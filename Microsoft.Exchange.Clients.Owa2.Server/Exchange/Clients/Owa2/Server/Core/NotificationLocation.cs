using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class NotificationLocation
	{
		public abstract KeyValuePair<string, object> GetEventData();
	}
}
