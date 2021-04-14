using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class MeetingCancellationMessageType : MeetingMessageType
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

		public EnhancedLocationType EnhancedLocation;
	}
}
