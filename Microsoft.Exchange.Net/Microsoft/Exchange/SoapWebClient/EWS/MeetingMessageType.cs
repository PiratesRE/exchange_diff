using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[XmlInclude(typeof(MeetingRequestMessageType))]
	[DebuggerStepThrough]
	[XmlInclude(typeof(MeetingCancellationMessageType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(MeetingResponseMessageType))]
	[Serializable]
	public class MeetingMessageType : MessageType
	{
		public ItemIdType AssociatedCalendarItemId;

		public bool IsDelegated;

		[XmlIgnore]
		public bool IsDelegatedSpecified;

		public bool IsOutOfDate;

		[XmlIgnore]
		public bool IsOutOfDateSpecified;

		public bool HasBeenProcessed;

		[XmlIgnore]
		public bool HasBeenProcessedSpecified;

		public ResponseTypeType ResponseType;

		[XmlIgnore]
		public bool ResponseTypeSpecified;

		public string UID;

		public DateTime RecurrenceId;

		[XmlIgnore]
		public bool RecurrenceIdSpecified;

		public DateTime DateTimeStamp;

		[XmlIgnore]
		public bool DateTimeStampSpecified;

		public bool IsOrganizer;

		[XmlIgnore]
		public bool IsOrganizerSpecified;
	}
}
