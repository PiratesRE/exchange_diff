using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[Serializable]
	public enum IncludeOriginalInReplyType
	{
		[XmlEnum(Name = "Auto")]
		Auto,
		[XmlEnum(Name = "Manual")]
		Manual
	}
}
