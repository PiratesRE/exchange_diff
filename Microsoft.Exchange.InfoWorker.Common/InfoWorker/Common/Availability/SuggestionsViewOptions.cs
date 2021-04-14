using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SuggestionsViewOptions
	{
		public SuggestionsViewOptions()
		{
			this.Init();
		}

		[DataMember]
		public int GoodThreshold
		{
			get
			{
				return this.goodThreshold;
			}
			set
			{
				this.goodThreshold = value;
			}
		}

		[DataMember]
		public int MaximumResultsByDay
		{
			get
			{
				return this.maximumResultsByDay;
			}
			set
			{
				this.maximumResultsByDay = value;
			}
		}

		[DataMember]
		public int MaximumNonWorkHourResultsByDay
		{
			get
			{
				return this.maximumNonWorkHourResultsByDay;
			}
			set
			{
				this.maximumNonWorkHourResultsByDay = value;
			}
		}

		[DataMember]
		public int MeetingDurationInMinutes
		{
			get
			{
				return this.meetingDuration;
			}
			set
			{
				this.meetingDuration = value;
			}
		}

		[IgnoreDataMember]
		public SuggestionQuality MinimumSuggestionQuality
		{
			get
			{
				return this.minimumSuggestionQuality;
			}
			set
			{
				this.minimumSuggestionQuality = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "MinimumSuggestionQuality")]
		public string MinimumSuggestionQualityString
		{
			get
			{
				return EnumUtil.ToString<SuggestionQuality>(this.MinimumSuggestionQuality);
			}
			set
			{
				this.MinimumSuggestionQuality = EnumUtil.Parse<SuggestionQuality>(value);
			}
		}

		[DataMember]
		public Duration DetailedSuggestionsWindow
		{
			get
			{
				return this.detailedSuggestionsWindow;
			}
			set
			{
				this.detailedSuggestionsWindow = value;
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public DateTime CurrentMeetingTime
		{
			get
			{
				return this.currentMeetingTime;
			}
			set
			{
				this.currentMeetingTime = value;
			}
		}

		[DataMember(Name = "CurrentMeetingTime")]
		[XmlIgnore]
		public string CurrentMeetingTimeString
		{
			get
			{
				return this.CurrentMeetingTime.ToIso8061();
			}
			set
			{
				this.CurrentMeetingTime = DateTime.Parse(value);
			}
		}

		[DataMember]
		[XmlElement]
		public string GlobalObjectId
		{
			get
			{
				return this.globalObjectId;
			}
			set
			{
				this.globalObjectId = value;
			}
		}

		[XmlIgnore]
		internal byte[] GlobalObjectIdByteArray
		{
			get
			{
				if (this.globalObjectIdByteArray == null && this.globalObjectId != null)
				{
					int num = this.globalObjectId.Length / 2;
					this.globalObjectIdByteArray = new byte[num];
					for (int i = 0; i < num; i++)
					{
						this.globalObjectIdByteArray[i] = byte.Parse(this.globalObjectId.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
					}
				}
				return this.globalObjectIdByteArray;
			}
		}

		internal void Validate()
		{
			if (this.detailedSuggestionsWindow == null)
			{
				throw new MissingArgumentException(Strings.descMissingArgument("SuggestionsViewOptions.DetailedSuggestionsWindow"));
			}
			this.detailedSuggestionsWindow.Validate("SuggestionsViewOptions.DetailedSuggestionsWindow");
			if (this.detailedSuggestionsWindow.StartTime.TimeOfDay != TimeSpan.Zero)
			{
				throw new InvalidParameterException(Strings.descDateMustHaveZeroTimeSpan("SuggestionsViewOptions.DetailedSuggestionsWindow.StartDate"));
			}
			if (this.detailedSuggestionsWindow.EndTime.TimeOfDay != TimeSpan.Zero)
			{
				throw new InvalidParameterException(Strings.descDateMustHaveZeroTimeSpan("SuggestionsViewOptions.DetailedSuggestionsWindow.EndDate"));
			}
			if (this.goodThreshold < 1 || this.goodThreshold > 49)
			{
				throw new InvalidParameterException(Strings.descInvalidGoodThreshold(1, 49));
			}
			if (this.maximumResultsByDay < 0 || this.maximumResultsByDay > 48)
			{
				throw new InvalidParameterException(Strings.descInvalidMaximumResults);
			}
			if (this.maximumNonWorkHourResultsByDay < 0 || this.maximumNonWorkHourResultsByDay > 48)
			{
				throw new InvalidParameterException(Strings.descInvalidMaxNonWorkHourResultsPerDay);
			}
			if (this.meetingDuration < 30)
			{
				throw new DurationTooSmallException();
			}
			if (this.meetingDuration > 1440)
			{
				throw new DurationTooLargeException();
			}
		}

		[OnDeserializing]
		private void Init(StreamingContext context)
		{
			this.Init();
		}

		private void Init()
		{
			this.goodThreshold = 25;
			this.maximumResultsByDay = 24;
			this.maximumNonWorkHourResultsByDay = 0;
			this.meetingDuration = 60;
			this.minimumSuggestionQuality = SuggestionQuality.Fair;
		}

		private int goodThreshold;

		private int maximumResultsByDay;

		private int maximumNonWorkHourResultsByDay;

		private int meetingDuration;

		private SuggestionQuality minimumSuggestionQuality;

		private Duration detailedSuggestionsWindow;

		private DateTime currentMeetingTime;

		private string globalObjectId;

		private byte[] globalObjectIdByteArray;
	}
}
