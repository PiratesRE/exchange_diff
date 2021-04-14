using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	[XmlType(TypeName = "ClientExtensionProvidedTo", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum ClientExtensionProvidedTo
	{
		Everyone,
		SpecificUsers
	}
}
