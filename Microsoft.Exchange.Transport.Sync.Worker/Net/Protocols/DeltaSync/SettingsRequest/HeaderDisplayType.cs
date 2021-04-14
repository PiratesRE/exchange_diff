using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[Serializable]
	public enum HeaderDisplayType
	{
		[XmlEnum(Name = "No Header")]
		No_Header,
		[XmlEnum(Name = "Basic")]
		Basic,
		[XmlEnum(Name = "Full")]
		Full,
		[XmlEnum(Name = "Advanced")]
		Advanced
	}
}
