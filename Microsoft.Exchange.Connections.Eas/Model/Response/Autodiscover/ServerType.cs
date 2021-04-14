using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Autodiscover
{
	[XmlType(TypeName = "ServerType")]
	public enum ServerType
	{
		MobileSync = 1,
		CertEnroll
	}
}
