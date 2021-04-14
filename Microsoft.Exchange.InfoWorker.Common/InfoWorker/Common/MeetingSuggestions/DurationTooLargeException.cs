using System;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	internal class DurationTooLargeException : InvalidParameterException
	{
		public DurationTooLargeException() : base(Strings.descMeetingSuggestionsDurationTooLarge)
		{
			base.ErrorCode = 102;
		}
	}
}
