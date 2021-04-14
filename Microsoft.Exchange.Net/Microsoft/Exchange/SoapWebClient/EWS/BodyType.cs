using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class BodyType
	{
		[XmlAttribute("BodyType")]
		public BodyTypeType BodyType1;

		[XmlAttribute]
		public bool IsTruncated;

		[XmlIgnore]
		public bool IsTruncatedSpecified;

		[XmlText]
		public string Value;
	}
}
