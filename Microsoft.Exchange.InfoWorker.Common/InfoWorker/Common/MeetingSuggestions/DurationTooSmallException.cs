using System;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	internal class DurationTooSmallException : InvalidParameterException
	{
		public DurationTooSmallException() : base(Strings.descMeetingSuggestionsDurationTooSmall)
		{
			base.ErrorCode = 101;
		}
	}
}
