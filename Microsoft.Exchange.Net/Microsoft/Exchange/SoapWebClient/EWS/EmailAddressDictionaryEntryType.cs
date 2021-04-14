using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class EmailAddressDictionaryEntryType
	{
		[XmlAttribute]
		public EmailAddressKeyType Key;

		[XmlAttribute]
		public string Name;

		[XmlAttribute]
		public string RoutingType;

		[XmlAttribute]
		public MailboxTypeType MailboxType;

		[XmlIgnore]
		public bool MailboxTypeSpecified;

		[XmlText]
		public string Value;
	}
}
