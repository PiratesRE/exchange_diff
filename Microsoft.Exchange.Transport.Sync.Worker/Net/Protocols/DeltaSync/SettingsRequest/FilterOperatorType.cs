using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[Serializable]
	public enum FilterOperatorType
	{
		[XmlEnum(Name = "Contains")]
		Contains,
		[XmlEnum(Name = "Does not contain")]
		Does_not_contain,
		[XmlEnum(Name = "Contains word")]
		Contains_word,
		[XmlEnum(Name = "Starts with")]
		Starts_with,
		[XmlEnum(Name = "Ends with")]
		Ends_with,
		[XmlEnum(Name = "Equals")]
		Equals
	}
}
