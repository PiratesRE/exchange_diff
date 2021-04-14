using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[Serializable]
	public enum AlertStateType
	{
		[XmlEnum(Name = "None")]
		None,
		[XmlEnum(Name = "Always")]
		Always,
		[XmlEnum(Name = "Rules")]
		Rules
	}
}
