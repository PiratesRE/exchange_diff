using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	[XmlType(TypeName = "ExtensionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum ExtensionType
	{
		Default,
		Private,
		MarketPlace
	}
}
