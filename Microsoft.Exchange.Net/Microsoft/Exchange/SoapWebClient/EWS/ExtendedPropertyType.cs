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
	public class ExtendedPropertyType
	{
		public PathToExtendedFieldType ExtendedFieldURI;

		[XmlElement("Values", typeof(NonEmptyArrayOfPropertyValuesType))]
		[XmlElement("Value", typeof(string))]
		public object Item;
	}
}
