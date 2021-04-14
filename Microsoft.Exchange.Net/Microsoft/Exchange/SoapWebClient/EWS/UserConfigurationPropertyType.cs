using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Flags]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum UserConfigurationPropertyType
	{
		Id = 1,
		Dictionary = 2,
		XmlData = 4,
		BinaryData = 8,
		All = 16
	}
}
