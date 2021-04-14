using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	public enum AdmissionType
	{
		[EnumMember]
		[XmlEnum]
		ucLocked,
		[XmlEnum]
		[EnumMember]
		ucClosedAuthenticated,
		[XmlEnum]
		[EnumMember]
		ucOpenAuthenticated,
		[XmlEnum]
		[EnumMember]
		ucAnonymous
	}
}
