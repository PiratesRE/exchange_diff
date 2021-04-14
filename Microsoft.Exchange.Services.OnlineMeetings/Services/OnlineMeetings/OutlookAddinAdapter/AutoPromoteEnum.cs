using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("AutoPromoteEnum")]
	public enum AutoPromoteEnum
	{
		[EnumMember]
		[XmlEnum]
		None,
		[EnumMember]
		[XmlEnum]
		Company,
		[EnumMember]
		[XmlEnum]
		Everyone
	}
}
