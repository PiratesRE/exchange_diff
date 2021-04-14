using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.ItemOperations
{
	[XmlType(Namespace = "ItemOperations", TypeName = "Fetch")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Fetch
	{
		[XmlElement(ElementName = "Store")]
		public string Store { get; set; }

		[XmlElement(ElementName = "CollectionId", Namespace = "AirSync")]
		public string CollectionId { get; set; }

		[XmlElement(ElementName = "ServerId", Namespace = "AirSync")]
		public string ServerId { get; set; }

		[XmlElement(ElementName = "LongId", Namespace = "Search")]
		public string LongId { get; set; }

		[XmlElement(ElementName = "FileReference", Namespace = "AirSyncBase")]
		public string FileReference { get; set; }

		[XmlElement(ElementName = "Options")]
		public Options Options { get; set; }
	}
}
