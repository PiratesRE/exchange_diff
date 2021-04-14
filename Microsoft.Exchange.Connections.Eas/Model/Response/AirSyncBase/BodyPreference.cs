using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSyncBase
{
	[XmlType(Namespace = "AirSyncBase", TypeName = "BodyPreference")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class BodyPreference
	{
		[XmlElement(ElementName = "Type")]
		public byte Type { get; set; }

		[XmlElement(ElementName = "TruncationSize")]
		public uint TruncationSize { get; set; }

		[XmlIgnore]
		public bool TruncationSizeSpecified { get; set; }

		[XmlElement(ElementName = "AllOrNone")]
		public bool AllOrNone { get; set; }

		[XmlIgnore]
		public bool AllOrNoneSpecified { get; set; }

		[XmlElement(ElementName = "Preview")]
		public uint Preview { get; set; }

		[XmlIgnore]
		public bool PreviewSpecified { get; set; }
	}
}
