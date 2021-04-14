using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(TypeName = "User")]
	public class User
	{
		[XmlElement(ElementName = "DisplayName")]
		public string DisplayName { get; set; }

		[XmlElement(ElementName = "EMailAddress")]
		public string EMailAddress { get; set; }
	}
}
