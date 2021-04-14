using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[Serializable]
	public enum VacationResponseMode
	{
		[XmlEnum(Name = "NoVacationResponse")]
		NoVacationResponse,
		[XmlEnum(Name = "OncePerSender")]
		OncePerSender,
		[XmlEnum(Name = "OncePerContact")]
		OncePerContact
	}
}
