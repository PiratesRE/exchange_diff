using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum UserConfigurationDictionaryObjectTypesType
	{
		DateTime,
		Boolean,
		Byte,
		String,
		Integer32,
		UnsignedInteger32,
		Integer64,
		UnsignedInteger64,
		StringArray,
		ByteArray
	}
}
