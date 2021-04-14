using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SuggestionsViewOptionsType
	{
		public int GoodThreshold
		{
			get
			{
				return this.goodThresholdField;
			}
			set
			{
				this.goodThresholdField = value;
			}
		}

		[XmlIgnore]
		public bool GoodThresholdSpecified
		{
			get
			{
				return this.goodThresholdFieldSpecified;
			}
			set
			{
				this.goodThresholdFieldSpecified = value;
			}
		}

		public int MaximumResultsByDay
		{
			get
			{
				return this.maximumResultsByDayField;
			}
			set
			{
				this.maximumResultsByDayField = value;
			}
		}

		[XmlIgnore]
		public bool MaximumResultsByDaySpecified
		{
			get
			{
				return this.maximumResultsByDayFieldSpecified;
			}
			set
			{
				this.maximumResultsByDayFieldSpecified = value;
			}
		}

		public int MaximumNonWorkHourResultsByDay
		{
			get
			{
				return this.maximumNonWorkHourResultsByDayField;
			}
			set
			{
				this.maximumNonWorkHourResultsByDayField = value;
			}
		}

		[XmlIgnore]
		public bool MaximumNonWorkHourResultsByDaySpecified
		{
			get
			{
				return this.maximumNonWorkHourResultsByDayFieldSpecified;
			}
			set
			{
				this.maximumNonWorkHourResultsByDayFieldSpecified = value;
			}
		}

		public int MeetingDurationInMinutes
		{
			get
			{
				return this.meetingDurationInMinutesField;
			}
			set
			{
				this.meetingDurationInMinutesField = value;
			}
		}

		[XmlIgnore]
		public bool MeetingDurationInMinutesSpecified
		{
			get
			{
				return this.meetingDurationInMinutesFieldSpecified;
			}
			set
			{
				this.meetingDurationInMinutesFieldSpecified = value;
			}
		}

		public SuggestionQuality MinimumSuggestionQuality
		{
			get
			{
				return this.minimumSuggestionQualityField;
			}
			set
			{
				this.minimumSuggestionQualityField = value;
			}
		}

		[XmlIgnore]
		public bool MinimumSuggestionQualitySpecified
		{
			get
			{
				return this.minimumSuggestionQualityFieldSpecified;
			}
			set
			{
				this.minimumSuggestionQualityFieldSpecified = value;
			}
		}

		public Duration DetailedSuggestionsWindow
		{
			get
			{
				return this.detailedSuggestionsWindowField;
			}
			set
			{
				this.detailedSuggestionsWindowField = value;
			}
		}

		public DateTime CurrentMeetingTime
		{
			get
			{
				return this.currentMeetingTimeField;
			}
			set
			{
				this.currentMeetingTimeField = value;
			}
		}

		[XmlIgnore]
		public bool CurrentMeetingTimeSpecified
		{
			get
			{
				return this.currentMeetingTimeFieldSpecified;
			}
			set
			{
				this.currentMeetingTimeFieldSpecified = value;
			}
		}

		public string GlobalObjectId
		{
			get
			{
				return this.globalObjectIdField;
			}
			set
			{
				this.globalObjectIdField = value;
			}
		}

		private int goodThresholdField;

		private bool goodThresholdFieldSpecified;

		private int maximumResultsByDayField;

		private bool maximumResultsByDayFieldSpecified;

		private int maximumNonWorkHourResultsByDayField;

		private bool maximumNonWorkHourResultsByDayFieldSpecified;

		private int meetingDurationInMinutesField;

		private bool meetingDurationInMinutesFieldSpecified;

		private SuggestionQuality minimumSuggestionQualityField;

		private bool minimumSuggestionQualityFieldSpecified;

		private Duration detailedSuggestionsWindowField;

		private DateTime currentMeetingTimeField;

		private bool currentMeetingTimeFieldSpecified;

		private string globalObjectIdField;
	}
}
