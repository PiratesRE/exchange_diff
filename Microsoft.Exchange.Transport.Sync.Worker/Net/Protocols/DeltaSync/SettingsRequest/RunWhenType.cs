using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[Serializable]
	public enum RunWhenType
	{
		[XmlEnum(Name = "MessageReceived")]
		MessageReceived,
		[XmlEnum(Name = "MessageSent")]
		MessageSent
	}
}
