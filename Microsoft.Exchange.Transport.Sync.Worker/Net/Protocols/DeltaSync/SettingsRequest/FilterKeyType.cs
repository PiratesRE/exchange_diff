using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[Serializable]
	public enum FilterKeyType
	{
		[XmlEnum(Name = "Subject")]
		Subject,
		[XmlEnum(Name = "From Name")]
		From_Name,
		[XmlEnum(Name = "From Address")]
		From_Address,
		[XmlEnum(Name = "To or CC Line")]
		To_or_CC_Line
	}
}
