using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Autodiscover
{
	[XmlType(TypeName = "Server")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Server
	{
		[XmlElement(ElementName = "Type")]
		public ServerType Type { get; set; }

		[XmlElement(ElementName = "Url")]
		public string Url { get; set; }

		[XmlElement(ElementName = "Name")]
		public string Name { get; set; }

		[XmlElement(ElementName = "ServerData")]
		public string ServerData { get; set; }
	}
}
