using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase
{
	[XmlType(Namespace = "AirSyncBase", TypeName = "BodyPreference")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class BodyPreference
	{
		[XmlElement(ElementName = "Type")]
		public byte? Type { get; set; }

		[XmlElement(ElementName = "TruncationSize")]
		public uint? TruncationSize { get; set; }

		[XmlIgnore]
		public bool? AllOrNone { get; set; }

		[XmlElement(ElementName = "AllOrNone")]
		public string SerializableAllOrNone
		{
			get
			{
				if (this.AllOrNone == null)
				{
					return "0";
				}
				if (!this.AllOrNone.Value)
				{
					return "0";
				}
				return "1";
			}
			set
			{
				this.AllOrNone = new bool?(XmlConvert.ToBoolean(value));
			}
		}

		[XmlElement(ElementName = "Restriction")]
		public string Restriction { get; set; }

		[XmlElement(ElementName = "Preview")]
		public uint? Preview { get; set; }

		[XmlIgnore]
		public bool TypeSpecified
		{
			get
			{
				return this.Type != null;
			}
		}

		[XmlIgnore]
		public bool TruncationSizeSpecified
		{
			get
			{
				return this.TruncationSize != null;
			}
		}

		[XmlIgnore]
		public bool SerializableAllOrNoneSpecified
		{
			get
			{
				return this.AllOrNone != null;
			}
		}

		[XmlIgnore]
		public bool PreviewSpecified
		{
			get
			{
				return this.Preview != null;
			}
		}
	}
}
