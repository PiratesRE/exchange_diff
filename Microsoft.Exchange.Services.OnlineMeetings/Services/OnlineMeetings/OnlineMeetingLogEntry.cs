using System;
using Microsoft.Exchange.Services.OnlineMeetings.ResourceContract;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class OnlineMeetingLogEntry : LogEntry
	{
		internal Guid UserGuid { get; set; }

		internal string ItemId { get; set; }

		internal string MeetingUrl { get; set; }

		internal OnlineMeetingSettings MeetingSettings { get; set; }

		internal OnlineMeetingDefaultValuesResource DefaultValuesResource { get; set; }

		internal OnlineMeetingPoliciesResource PoliciesResource { get; set; }
	}
}
