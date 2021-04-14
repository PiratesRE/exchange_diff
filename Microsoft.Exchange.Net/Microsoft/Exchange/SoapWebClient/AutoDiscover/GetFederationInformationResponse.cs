using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.AutoDiscover
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class GetFederationInformationResponse : AutodiscoverResponse
	{
		[XmlElement(DataType = "anyURI", IsNullable = true)]
		public string ApplicationUri;

		[XmlArray(IsNullable = true)]
		public TokenIssuer[] TokenIssuers;

		[XmlArray(IsNullable = true)]
		[XmlArrayItem("Domain")]
		public string[] Domains;
	}
}
