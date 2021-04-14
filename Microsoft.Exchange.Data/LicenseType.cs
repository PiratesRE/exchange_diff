using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	[XmlType(TypeName = "LicenseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum LicenseType
	{
		Free,
		Trial,
		Paid
	}
}
