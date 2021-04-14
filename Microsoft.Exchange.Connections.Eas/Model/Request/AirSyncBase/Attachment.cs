using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase
{
	[XmlType(Namespace = "AirSyncBase", TypeName = "Attachment")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Attachment
	{
		[XmlElement(ElementName = "DisplayName")]
		public string DisplayName { get; set; }

		[XmlElement(ElementName = "FileReference")]
		public string FileReference { get; set; }

		[XmlElement(ElementName = "Method")]
		public byte? Method { get; set; }

		[XmlElement(ElementName = "EstimatedDataSize")]
		public uint? EstimatedDataSize { get; set; }

		[XmlElement(ElementName = "ContentId")]
		public string ContentId { get; set; }

		[XmlElement(ElementName = "ContentLocation")]
		public string ContentLocation { get; set; }

		[XmlElement(ElementName = "IsInline")]
		public bool? IsInline { get; set; }

		[XmlIgnore]
		public bool MethodSpecified
		{
			get
			{
				return this.Method != null;
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
		public bool IsInlineSpecified
		{
			get
			{
				return this.IsInline != null;
			}
		}
	}
}
