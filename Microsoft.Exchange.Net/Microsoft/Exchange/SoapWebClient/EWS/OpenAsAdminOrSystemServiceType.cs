using System;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[CLSCompliant(false)]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlRoot("OpenAsAdminOrSystemService", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
	[Serializable]
	public class OpenAsAdminOrSystemServiceType : SoapHeader
	{
		public ConnectingSIDType ConnectingSID;

		[XmlAttribute]
		public SpecialLogonType LogonType;

		[XmlAttribute]
		public int BudgetType;

		[XmlIgnore]
		public bool BudgetTypeSpecified;

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr;
	}
}
