using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(TypeName = "Response")]
	public class Response
	{
		[XmlElement(ElementName = "Culture")]
		public string Culture { get; set; }

		[XmlElement(ElementName = "User")]
		public User User { get; set; }

		[XmlElement(ElementName = "Action")]
		public Action Action { get; set; }

		[XmlElement(ElementName = "Error")]
		public Error Error { get; set; }
	}
}
