using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface ILogEvent
	{
		string EventId { get; }

		ICollection<KeyValuePair<string, object>> GetEventData();
	}
}
