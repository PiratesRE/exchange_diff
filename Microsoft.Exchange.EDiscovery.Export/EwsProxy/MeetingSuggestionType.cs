using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MeetingSuggestionType : EntityType
	{
		[XmlArrayItem("EmailUser", IsNullable = false)]
		public EmailUserType[] Attendees
		{
			get
			{
				return this.attendeesField;
			}
			set
			{
				this.attendeesField = value;
			}
		}

		public string Location
		{
			get
			{
				return this.locationField;
			}
			set
			{
				this.locationField = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.subjectField;
			}
			set
			{
				this.subjectField = value;
			}
		}

		public string MeetingString
		{
			get
			{
				return this.meetingStringField;
			}
			set
			{
				this.meetingStringField = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.startTimeField;
			}
			set
			{
				this.startTimeField = value;
			}
		}

		[XmlIgnore]
		public bool StartTimeSpecified
		{
			get
			{
				return this.startTimeFieldSpecified;
			}
			set
			{
				this.startTimeFieldSpecified = value;
			}
		}

		public DateTime EndTime
		{
			get
			{
				return this.endTimeField;
			}
			set
			{
				this.endTimeField = value;
			}
		}

		[XmlIgnore]
		public bool EndTimeSpecified
		{
			get
			{
				return this.endTimeFieldSpecified;
			}
			set
			{
				this.endTimeFieldSpecified = value;
			}
		}

		private EmailUserType[] attendeesField;

		private string locationField;

		private string subjectField;

		private string meetingStringField;

		private DateTime startTimeField;

		private bool startTimeFieldSpecified;

		private DateTime endTimeField;

		private bool endTimeFieldSpecified;
	}
}
