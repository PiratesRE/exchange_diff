using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class MeetingResponseMessageType : MeetingMessageType
	{
		public DateTime Start;

		[XmlIgnore]
		public bool StartSpecified;

		public DateTime End;

		[XmlIgnore]
		public bool EndSpecified;

		public string Location;

		public RecurrenceType Recurrence;

		public string CalendarItemType;

		public DateTime ProposedStart;

		[XmlIgnore]
		public bool ProposedStartSpecified;

		public DateTime ProposedEnd;

		[XmlIgnore]
		public bool ProposedEndSpecified;

		public EnhancedLocationType EnhancedLocation;
	}
}
