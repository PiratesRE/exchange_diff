using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	[XmlType(TypeName = "ExtensionInstallScope", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum ExtensionInstallScope
	{
		None,
		User,
		Organization,
		Default
	}
}
