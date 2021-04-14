using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class AlternateMailbox
	{
		[XmlElement(IsNullable = true)]
		public string Type;

		[XmlElement(IsNullable = true)]
		public string DisplayName;

		[XmlElement(IsNullable = true)]
		public string LegacyDN;

		[XmlElement(IsNullable = true)]
		public string Server;

		[XmlElement(IsNullable = true)]
		public string SmtpAddress;

		[XmlElement(IsNullable = true)]
		public string OwnerSmtpAddress;
	}
}
