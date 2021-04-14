using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class FindMessageTrackingReportRequestType : BaseRequestType
	{
		public string Scope;

		public string Domain;

		public EmailAddressType Sender;

		public EmailAddressType PurportedSender;

		public EmailAddressType Recipient;

		public string Subject;

		public DateTime StartDateTime;

		[XmlIgnore]
		public bool StartDateTimeSpecified;

		public DateTime EndDateTime;

		[XmlIgnore]
		public bool EndDateTimeSpecified;

		public string MessageId;

		public EmailAddressType FederatedDeliveryMailbox;

		public string DiagnosticsLevel;

		public string ServerHint;

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
