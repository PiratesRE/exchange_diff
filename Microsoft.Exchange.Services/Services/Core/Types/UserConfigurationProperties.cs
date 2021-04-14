using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UserConfigurationPropertyType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Flags]
	[Serializable]
	public enum UserConfigurationProperties
	{
		Id = 1,
		Dictionary = 2,
		XmlData = 4,
		BinaryData = 8,
		All = 15
	}
}
