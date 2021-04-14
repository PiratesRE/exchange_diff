using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "ItemOperations", TypeName = "Fetch")]
	public class Fetch
	{
		[XmlElement(ElementName = "Class", Namespace = "AirSync")]
		public string Class { get; set; }

		[XmlElement(ElementName = "CollectionId", Namespace = "AirSync")]
		public string CollectionId { get; set; }

		[XmlElement(ElementName = "Properties")]
		public Properties Properties { get; set; }

		[XmlElement(ElementName = "ServerId", Namespace = "AirSync")]
		public string ServerId { get; set; }

		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }
	}
}
