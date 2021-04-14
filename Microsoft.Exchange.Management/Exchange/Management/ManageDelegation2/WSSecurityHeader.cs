using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.ManageDelegation2
{
	[XmlType(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
	[DebuggerStepThrough]
	[XmlRoot("Security", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", IsNullable = false)]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class WSSecurityHeader : SoapHeader
	{
		[XmlAnyElement(Name = "Timestamp", Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
		public XmlElement Timestamp;

		[XmlAnyElement(Name = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public XmlElement Signature;

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr;
	}
}
