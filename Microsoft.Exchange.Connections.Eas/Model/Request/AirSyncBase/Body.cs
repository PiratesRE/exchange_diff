using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase
{
	[XmlType(Namespace = "AirSyncBase", TypeName = "Body")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Body
	{
		[XmlElement(ElementName = "Type")]
		public byte? Type { get; set; }

		[XmlElement(ElementName = "Data")]
		public string Data { get; set; }

		[XmlElement(ElementName = "EstimatedDataSize")]
		public uint? EstimatedDataSize { get; set; }

		[XmlElement(ElementName = "Truncated")]
		public bool? Truncated { get; set; }

		[XmlIgnore]
		public bool TypeSpecified
		{
			get
			{
				return this.Type != null;
			}
		}

		[XmlIgnore]
		public bool EstimatedDataSizeSpecified
		{
			get
			{
				return this.EstimatedDataSize != null;
			}
		}

		[XmlIgnore]
		public bool TruncatedSpecified
		{
			get
			{
				return this.Truncated != null;
			}
		}
	}
}
