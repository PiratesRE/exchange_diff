using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class AvailabilityQueryResult
	{
		public FreeBusyQueryResult[] FreeBusyResults
		{
			get
			{
				return this.freeBusyResults;
			}
			internal set
			{
				this.freeBusyResults = value;
			}
		}

		public SuggestionDayResult[] DailyMeetingSuggestions
		{
			get
			{
				return this.dailyMeetingSuggestions;
			}
			internal set
			{
				this.dailyMeetingSuggestions = value;
			}
		}

		public LocalizedException MeetingSuggestionsException
		{
			get
			{
				return this.meetingSuggestionsException;
			}
			internal set
			{
				this.meetingSuggestionsException = value;
			}
		}

		internal static AvailabilityQueryResult Create()
		{
			return new AvailabilityQueryResult();
		}

		private AvailabilityQueryResult()
		{
		}

		private FreeBusyQueryResult[] freeBusyResults;

		private SuggestionDayResult[] dailyMeetingSuggestions;

		private LocalizedException meetingSuggestionsException;
	}
}
