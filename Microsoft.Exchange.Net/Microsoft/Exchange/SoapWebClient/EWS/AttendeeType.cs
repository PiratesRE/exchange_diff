using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class AttendeeType
	{
		public EmailAddressType Mailbox;

		public ResponseTypeType ResponseType;

		[XmlIgnore]
		public bool ResponseTypeSpecified;

		public DateTime LastResponseTime;

		[XmlIgnore]
		public bool LastResponseTimeSpecified;

		public DateTime ProposedStart;

		[XmlIgnore]
		public bool ProposedStartSpecified;

		public DateTime ProposedEnd;

		[XmlIgnore]
		public bool ProposedEndSpecified;
	}
}
