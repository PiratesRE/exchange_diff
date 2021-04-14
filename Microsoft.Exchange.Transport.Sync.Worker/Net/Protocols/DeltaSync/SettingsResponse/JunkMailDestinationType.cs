using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[Serializable]
	public enum JunkMailDestinationType
	{
		[XmlEnum(Name = "Immediate Deletion")]
		Immediate_Deletion,
		[XmlEnum(Name = "Junk Mail")]
		Junk_Mail
	}
}
