using System;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[CLSCompliant(false)]
	[XmlRoot("SerializedSecurityContext", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SerializedSecurityContextType : SoapHeader
	{
		public string UserSid;

		[XmlArrayItem("GroupIdentifier", IsNullable = false)]
		public SidAndAttributesType[] GroupSids;

		[XmlArrayItem("RestrictedGroupIdentifier", IsNullable = false)]
		public SidAndAttributesType[] RestrictedGroupSids;

		public string PrimarySmtpAddress;

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr;
	}
}
