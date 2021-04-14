using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UserConfigurationDictionaryObjectTypesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum UserConfigurationDictionaryObjectType
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
