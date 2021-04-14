using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[Serializable]
	public enum JunkLevelType
	{
		[XmlEnum(Name = "Off")]
		Off,
		[XmlEnum(Name = "Low")]
		Low,
		[XmlEnum(Name = "High")]
		High,
		[XmlEnum(Name = "Exclusive")]
		Exclusive
	}
}
