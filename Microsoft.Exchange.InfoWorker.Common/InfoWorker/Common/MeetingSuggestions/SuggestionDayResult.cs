using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SuggestionDayResult
	{
		public SuggestionDayResult()
		{
		}

		internal SuggestionDayResult(ExDateTime currentDate, int inputMeetingDuration, int requiredAttendeeCount, int optionalAttendeeCount, AttendeeData[] attendees, ConfigOptions options, ExDateTime currentMeetingTime)
		{
			this.date = currentDate.LocalTime;
			ExDateTime t = ExDateTime.Now.ToUtc();
			List<Suggestion> list = new List<Suggestion>();
			ExDateTime t2 = currentDate.AddDays(1.0);
			ExDateTime exDateTime = currentDate.ToUtc();
			while (exDateTime < t2)
			{
				if (exDateTime >= t || exDateTime == currentMeetingTime)
				{
					list.Add(new Suggestion(currentDate.TimeZone.ConvertDateTime(exDateTime), inputMeetingDuration, requiredAttendeeCount, optionalAttendeeCount, attendees, options));
				}
				exDateTime = exDateTime.AddMinutes((double)options.SuggestionInterval);
			}
			this.rawSuggestionsList = list.ToArray();
			this.FilterSuggestions(list, options, currentMeetingTime);
			foreach (Suggestion suggestion in list)
			{
				if (suggestion.SuggestionQuality < this.quality)
				{
					this.quality = suggestion.SuggestionQuality;
				}
			}
			this.meetingSuggestions = list.ToArray();
		}

		[XmlIgnore]
		public Suggestion this[TimeSpan timeOfDay]
		{
			get
			{
				if (timeOfDay.TotalMinutes < 1440.0)
				{
					return this.rawSuggestionsList[(int)((double)this.rawSuggestionsList.Length * timeOfDay.TotalMinutes) / 1440];
				}
				throw new ArgumentException("timeOfDay must be between 0:00 and 23:59:59");
			}
		}

		private void FilterSuggestions(List<Suggestion> suggestionList, ConfigOptions options, ExDateTime currentMeetingTime)
		{
			suggestionList.RemoveAll((Suggestion suggestion) => !(suggestion.MeetingTime == (DateTime)currentMeetingTime) && (suggestion.TimeSlotRating == -1L || suggestion.SuggestionQuality > options.MinimumSuggestionQuality) && options.MinimumSuggestionQuality != SuggestionQuality.Poor);
			SuggestionDayResult.Tracer.TraceDebug<object, int>((long)this.GetHashCode(), "{0}: {1} suggestions passing minimum quality.", TraceContext.Get(), suggestionList.Count);
			suggestionList.Sort(delegate(Suggestion x, Suggestion y)
			{
				if (x == y)
				{
					return 0;
				}
				if (x.MeetingTime == (DateTime)currentMeetingTime)
				{
					return -1;
				}
				if (y.MeetingTime == (DateTime)currentMeetingTime)
				{
					return 1;
				}
				if (x.TimeSlotRating == y.TimeSlotRating)
				{
					if (x.MeetingTime < y.MeetingTime)
					{
						return -1;
					}
					if (x.MeetingTime > y.MeetingTime)
					{
						return 1;
					}
					return 0;
				}
				else
				{
					if (x.TimeSlotRating > y.TimeSlotRating)
					{
						return 1;
					}
					return -1;
				}
			});
			int num = 0;
			for (int i = 0; i < suggestionList.Count; i++)
			{
				if (!suggestionList[i].IsWorkTime)
				{
					if (num >= options.MaximumNonWorkHourResultsPerDay)
					{
						suggestionList.RemoveAt(i);
						i--;
					}
					else
					{
						num++;
					}
				}
			}
			SuggestionDayResult.Tracer.TraceDebug<object, int>((long)this.GetHashCode(), "{0}: {1} suggestions after non-working hour restrictions.", TraceContext.Get(), suggestionList.Count);
			if (suggestionList.Count > options.MaximumResultsPerDay)
			{
				suggestionList.RemoveRange(options.MaximumResultsPerDay, suggestionList.Count - options.MaximumResultsPerDay);
				SuggestionDayResult.Tracer.TraceDebug<object, int>((long)this.GetHashCode(), "{0}: suggestions array length shortened to MaximumResultsPerDay ({1}).", TraceContext.Get(), options.MaximumResultsPerDay);
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public DateTime Date
		{
			get
			{
				return this.date;
			}
			set
			{
				this.date = value;
			}
		}

		[DataMember(Name = "Date")]
		[XmlIgnore]
		public string DateString
		{
			get
			{
				return this.Date.ToIso8061();
			}
			set
			{
				this.Date = DateTime.Parse(value);
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public SuggestionQuality DayQuality
		{
			get
			{
				return this.quality;
			}
			set
			{
				this.quality = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "DayQuality")]
		public string DayQualityString
		{
			get
			{
				return EnumUtil.ToString<SuggestionQuality>(this.DayQuality);
			}
			set
			{
				this.DayQuality = EnumUtil.Parse<SuggestionQuality>(value);
			}
		}

		[XmlArrayItem(Type = typeof(Suggestion), IsNullable = false)]
		[DataMember]
		[XmlArray(IsNullable = false)]
		public Suggestion[] SuggestionArray
		{
			get
			{
				return this.meetingSuggestions;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.meetingSuggestions = value;
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.MeetingSuggestionsTracer;

		private DateTime date;

		private Suggestion[] meetingSuggestions;

		private Suggestion[] rawSuggestionsList;

		private SuggestionQuality quality = SuggestionQuality.Poor;
	}
}
