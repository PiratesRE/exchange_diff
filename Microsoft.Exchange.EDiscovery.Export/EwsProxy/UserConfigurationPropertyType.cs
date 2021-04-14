using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[Flags]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
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
