using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSyncBase
{
	[XmlType(Namespace = "AirSyncBase", TypeName = "BodyPart")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class BodyPart
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }

		[XmlElement(ElementName = "Type")]
		public byte Type { get; set; }

		[XmlElement(ElementName = "EstimatedDataSize")]
		public uint EstimatedDataSize { get; set; }

		[XmlElement(ElementName = "Truncated")]
		public bool Truncated { get; set; }

		[XmlIgnore]
		public bool TruncatedSpecified { get; set; }

		[XmlElement(ElementName = "Data")]
		public string Data { get; set; }

		[XmlElement(ElementName = "Preview")]
		public string Preview { get; set; }
	}
}
