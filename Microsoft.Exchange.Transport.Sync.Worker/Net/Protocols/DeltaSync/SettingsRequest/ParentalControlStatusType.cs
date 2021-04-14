using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[Serializable]
	public enum ParentalControlStatusType
	{
		[XmlEnum(Name = "None")]
		None,
		[XmlEnum(Name = "FullAccess")]
		FullAccess,
		[XmlEnum(Name = "RestrictedAccess")]
		RestrictedAccess,
		[XmlEnum(Name = "NoAccess")]
		NoAccess
	}
}
