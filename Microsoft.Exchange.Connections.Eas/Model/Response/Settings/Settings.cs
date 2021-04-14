using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Settings
{
	[XmlType(Namespace = "Settings", TypeName = "Settings")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Settings
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }
	}
}
