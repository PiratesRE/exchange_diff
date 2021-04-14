using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ConnectionSettingsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum ConnectionSettingsInfoType
	{
		[XmlEnum("item:Office365")]
		Office365 = 10,
		[XmlEnum("item:ExchangeActiveSync")]
		ExchangeActiveSync = 20,
		[XmlEnum("item:Imap")]
		Imap = 30,
		[XmlEnum("item:Pop")]
		Pop = 40,
		[XmlEnum("item:Smtp")]
		Smtp = 50
	}
}
