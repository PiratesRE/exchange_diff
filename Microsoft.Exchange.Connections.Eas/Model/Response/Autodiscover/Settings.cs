using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(TypeName = "Settings")]
	public class Settings
	{
		[XmlElement(ElementName = "Server")]
		public List<Server> Server { get; set; }
	}
}
