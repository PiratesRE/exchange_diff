using System;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	internal class InvalidTimeIntervalException : InvalidParameterException
	{
		public InvalidTimeIntervalException() : base(Strings.descMeetingSuggestionsInvalidTimeInterval)
		{
			base.ErrorCode = 100;
		}
	}
}
