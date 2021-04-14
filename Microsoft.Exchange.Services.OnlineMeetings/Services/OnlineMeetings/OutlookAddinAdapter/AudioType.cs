using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("AudioType")]
	public enum AudioType
	{
		[XmlEnum("none")]
		[EnumMember]
		None,
		[EnumMember]
		[XmlEnum("caa")]
		CAA,
		[XmlEnum("acp")]
		[EnumMember]
		ACP
	}
}
