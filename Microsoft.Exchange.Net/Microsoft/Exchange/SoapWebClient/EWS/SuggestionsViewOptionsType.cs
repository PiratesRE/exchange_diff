using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SuggestionsViewOptionsType
	{
		public int GoodThreshold;

		[XmlIgnore]
		public bool GoodThresholdSpecified;

		public int MaximumResultsByDay;

		[XmlIgnore]
		public bool MaximumResultsByDaySpecified;

		public int MaximumNonWorkHourResultsByDay;

		[XmlIgnore]
		public bool MaximumNonWorkHourResultsByDaySpecified;

		public int MeetingDurationInMinutes;

		[XmlIgnore]
		public bool MeetingDurationInMinutesSpecified;

		public SuggestionQuality MinimumSuggestionQuality;

		[XmlIgnore]
		public bool MinimumSuggestionQualitySpecified;

		public Duration DetailedSuggestionsWindow;

		public DateTime CurrentMeetingTime;

		[XmlIgnore]
		public bool CurrentMeetingTimeSpecified;

		public string GlobalObjectId;
	}
}
