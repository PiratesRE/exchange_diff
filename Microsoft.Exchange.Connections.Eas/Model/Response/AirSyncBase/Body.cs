using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSyncBase
{
	[XmlType(Namespace = "AirSyncBase", TypeName = "Body")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Body
	{
		[XmlElement(ElementName = "Data")]
		public string Data { get; set; }

		[XmlElement(ElementName = "EstimatedDataSize")]
		public uint EstimatedDataSize { get; set; }

		[XmlElement(ElementName = "Preview")]
		public string Preview { get; set; }

		[XmlElement(ElementName = "Type")]
		public BodyType Type { get; set; }

		[XmlElement(ElementName = "Truncated")]
		public bool Truncated { get; set; }
	}
}
