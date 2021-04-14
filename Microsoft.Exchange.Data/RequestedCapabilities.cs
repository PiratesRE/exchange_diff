using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	[XmlType(TypeName = "RequestedCapabilities", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum RequestedCapabilities
	{
		Restricted,
		ReadItem,
		ReadWriteMailbox,
		ReadWriteItem
	}
}
