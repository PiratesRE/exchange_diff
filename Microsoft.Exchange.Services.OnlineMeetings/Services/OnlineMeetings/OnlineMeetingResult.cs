using System;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class OnlineMeetingResult
	{
		public OnlineMeeting OnlineMeeting { get; set; }

		public CustomizationValues CustomizationValues { get; set; }

		public MeetingPolicies MeetingPolicies { get; set; }

		public DialInInformation DialIn { get; set; }

		public DefaultValues DefaultValues { get; set; }

		public OnlineMeetingLogEntry LogEntry { get; set; }
	}
}
