using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[Serializable]
	public enum ReplyIndicatorType
	{
		[XmlEnum(Name = "None")]
		None,
		[XmlEnum(Name = "Line")]
		Line,
		[XmlEnum(Name = "Arrow")]
		Arrow
	}
}
