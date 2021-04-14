using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class StringArrayAttributedValueType
	{
		[XmlArrayItem("Value", IsNullable = false)]
		public string[] Values;

		[XmlArrayItem("Attribution", IsNullable = false)]
		public string[] Attributions;
	}
}
