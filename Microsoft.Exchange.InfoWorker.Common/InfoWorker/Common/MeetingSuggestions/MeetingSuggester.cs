using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	internal class MeetingSuggester
	{
		private void ValidateAndProcessInput(ExDateTime startDate, ExDateTime endDate, int inputMeetingDuration, AttendeeData[] attendees)
		{
			this.requiredAttendeeCount = 0;
			this.optionalAttendeeCount = 0;
			foreach (AttendeeData attendeeData in attendees)
			{
				if (attendeeData == null)
				{
					MeetingSuggester.Tracer.TraceError((long)this.GetHashCode(), "{0}: Invalid inputAttendee array; an element is null.", new object[]
					{
						TraceContext.Get()
					});
					throw new ArgumentException("Invalid inputAttendee array; an element is null.");
				}
				switch (attendeeData.AttendeeType)
				{
				case MeetingAttendeeType.Organizer:
				case MeetingAttendeeType.Required:
					this.requiredAttendeeCount++;
					break;
				case MeetingAttendeeType.Optional:
					this.optionalAttendeeCount++;
					break;
				}
				if (startDate < attendeeData.FreeBusyStartTime || startDate >= attendeeData.FreeBusyEndTime || endDate <= attendeeData.FreeBusyStartTime || endDate > attendeeData.FreeBusyEndTime)
				{
					MeetingSuggester.Tracer.TraceError((long)this.GetHashCode(), "{0}: start and end times outside availability data.", new object[]
					{
						TraceContext.Get()
					});
					throw new InvalidParameterException(Strings.descStartAndEndTimesOutSideFreeBusyData);
				}
			}
		}

		public SuggestionDayResult[] GetSuggestionsByDateRange(ExDateTime startDate, ExDateTime endDate, ExTimeZone timeZone, int inputMeetingDuration, AttendeeData[] attendees)
		{
			this.meetingDuration = inputMeetingDuration;
			MeetingSuggester.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: Entering MeetingSuggester.GetSuggestionsByDateRange()", new object[]
			{
				TraceContext.Get()
			});
			MeetingSuggester.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: startDate={1}, endDate={2}, inputMeetingDuration={3}", new object[]
			{
				TraceContext.Get(),
				startDate,
				endDate,
				inputMeetingDuration
			});
			this.ValidateAndProcessInput(startDate, endDate, inputMeetingDuration, attendees);
			List<SuggestionDayResult> list = new List<SuggestionDayResult>();
			ExDateTime exDateTime = startDate;
			while (exDateTime < endDate)
			{
				list.Add(new SuggestionDayResult(exDateTime, this.meetingDuration, this.requiredAttendeeCount, this.optionalAttendeeCount, attendees, this.options, this.currentMeetingTime));
				exDateTime = exDateTime.AddDays(1.0);
			}
			return list.ToArray();
		}

		internal void SetOptionsFromSuggestionsViewOptions(SuggestionsViewOptions svOptions, ExTimeZone timeZone)
		{
			this.options.GoodThreshold = svOptions.GoodThreshold;
			this.options.MaximumResultsPerDay = svOptions.MaximumResultsByDay;
			this.options.MaximumNonWorkHourResultsPerDay = svOptions.MaximumNonWorkHourResultsByDay;
			this.meetingDuration = svOptions.MeetingDurationInMinutes;
			this.options.MinimumSuggestionQuality = svOptions.MinimumSuggestionQuality;
			this.currentMeetingTime = new ExDateTime(timeZone, svOptions.CurrentMeetingTime);
		}

		internal int MaximumResultsPerDay
		{
			get
			{
				return this.options.MaximumResultsPerDay;
			}
			set
			{
				if (value < 0 || value > 48)
				{
					throw new InvalidParameterException(Strings.descInvalidMaximumResults);
				}
				this.options.MaximumResultsPerDay = value;
			}
		}

		internal int MaximumNonWorkHourResultsPerDay
		{
			get
			{
				return this.options.MaximumNonWorkHourResultsPerDay;
			}
			set
			{
				if (value < 0 || value > 48)
				{
					throw new InvalidParameterException(Strings.descInvalidMaxNonWorkHourResultsPerDay);
				}
				this.options.MaximumNonWorkHourResultsPerDay = value;
			}
		}

		internal SuggestionQuality MinimumSuggestionQuality
		{
			get
			{
				return this.options.MinimumSuggestionQuality;
			}
			set
			{
				this.options.MinimumSuggestionQuality = value;
			}
		}

		internal int GoodThreshold
		{
			get
			{
				return this.options.GoodThreshold;
			}
			set
			{
				if (value < 1 || value > 49)
				{
					throw new InvalidParameterException(Strings.descInvalidGoodThreshold(1, 49));
				}
				this.options.GoodThreshold = value;
			}
		}

		public const int MaxMinutesInFullDayMeeting = 1440;

		private static readonly Trace Tracer = ExTraceGlobals.MeetingSuggestionsTracer;

		private int meetingDuration;

		private ConfigOptions options = new ConfigOptions();

		private int requiredAttendeeCount;

		private int optionalAttendeeCount;

		private ExDateTime currentMeetingTime;
	}
}
