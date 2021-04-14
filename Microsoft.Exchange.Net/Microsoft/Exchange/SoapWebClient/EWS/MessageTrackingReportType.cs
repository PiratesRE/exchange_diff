using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MessageTrackingReportType
	{
		public EmailAddressType Sender;

		public EmailAddressType PurportedSender;

		public string Subject;

		public DateTime SubmitTime;

		[XmlIgnore]
		public bool SubmitTimeSpecified;

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] OriginalRecipients;

		[XmlArrayItem("RecipientTrackingEvent", IsNullable = false)]
		public RecipientTrackingEventType[] RecipientTrackingEvents;

		[XmlArrayItem(IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
