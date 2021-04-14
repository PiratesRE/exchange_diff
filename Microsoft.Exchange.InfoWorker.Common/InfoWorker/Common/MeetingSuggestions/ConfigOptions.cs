using System;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	internal class ConfigOptions
	{
		public MeetingFrequencyEnum MeetingFrequency
		{
			get
			{
				return this.meetingFrequency;
			}
			set
			{
				this.meetingFrequency = value;
			}
		}

		public const int MaximumResults = 48;

		public const int MinimumMeetingDuration = 30;

		public const int MaximumMeetingDuration = 1440;

		public const int DefaultFreeBusyInterval = 30;

		public const int DefaultSuggestionInterval = 30;

		public const int DefaultMaximumResultsPerDay = 10;

		public const int DefaultMaximumNonWorkHourResultsPerDay = 0;

		public const SuggestionQuality DefaultMinimumSuggestionQuality = SuggestionQuality.Fair;

		public const int DefaultGoodThreshold = 25;

		public const int MaximumGoodThreshold = 49;

		public const int MinimumGoodThreshold = 1;

		internal int FreeBusyInterval = 30;

		internal int SuggestionInterval = 30;

		internal int MaximumResultsPerDay = 10;

		internal int MaximumNonWorkHourResultsPerDay;

		internal SuggestionQuality MinimumSuggestionQuality = SuggestionQuality.Fair;

		internal int GoodThreshold = 25;

		private MeetingFrequencyEnum meetingFrequency = MeetingFrequencyEnum.ThirtyMinutes;
	}
}
