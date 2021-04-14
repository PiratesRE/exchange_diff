using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[Serializable]
	public enum AccountStatusType
	{
		[XmlEnum(Name = "OK")]
		OK,
		[XmlEnum(Name = "Blocked")]
		Blocked,
		[XmlEnum(Name = "RequiresHIP")]
		RequiresHIP
	}
}
