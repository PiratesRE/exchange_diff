using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "ChangeResponse")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ChangeResponse
	{
		[XmlElement(ElementName = "Class")]
		public string Class { get; set; }

		[XmlElement(ElementName = "ServerId")]
		public string ServerId { get; set; }

		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }
	}
}
