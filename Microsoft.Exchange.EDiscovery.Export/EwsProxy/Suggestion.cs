using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class Suggestion
	{
		public DateTime MeetingTime
		{
			get
			{
				return this.meetingTimeField;
			}
			set
			{
				this.meetingTimeField = value;
			}
		}

		public bool IsWorkTime
		{
			get
			{
				return this.isWorkTimeField;
			}
			set
			{
				this.isWorkTimeField = value;
			}
		}

		public SuggestionQuality SuggestionQuality
		{
			get
			{
				return this.suggestionQualityField;
			}
			set
			{
				this.suggestionQualityField = value;
			}
		}

		[XmlArrayItem(typeof(TooBigGroupAttendeeConflictData))]
		[XmlArrayItem(typeof(GroupAttendeeConflictData))]
		[XmlArrayItem(typeof(UnknownAttendeeConflictData))]
		[XmlArrayItem(typeof(IndividualAttendeeConflictData))]
		public AttendeeConflictData[] AttendeeConflictDataArray
		{
			get
			{
				return this.attendeeConflictDataArrayField;
			}
			set
			{
				this.attendeeConflictDataArrayField = value;
			}
		}

		private DateTime meetingTimeField;

		private bool isWorkTimeField;

		private SuggestionQuality suggestionQualityField;

		private AttendeeConflictData[] attendeeConflictDataArrayField;
	}
}
