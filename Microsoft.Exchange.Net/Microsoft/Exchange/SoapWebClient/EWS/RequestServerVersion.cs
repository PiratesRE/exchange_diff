using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlRoot(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[Serializable]
	public class RequestServerVersion : SoapHeader
	{
		public RequestServerVersion()
		{
			this.Version = ExchangeVersionType.Exchange2013_SP1;
		}

		[XmlAttribute]
		public ExchangeVersionType Version;

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr;
	}
}
