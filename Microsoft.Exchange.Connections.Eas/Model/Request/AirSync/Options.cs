using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "Options")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Options
	{
		[XmlElement(ElementName = "Class")]
		public string Class { get; set; }

		[XmlElement(ElementName = "BodyPreference", Namespace = "AirSyncBase")]
		public BodyPreference BodyPreference { get; set; }

		[XmlElement(ElementName = "Conflict")]
		public byte? Conflict { get; set; }

		[XmlElement(ElementName = "FilterType")]
		public byte? FilterType { get; set; }

		[XmlElement(ElementName = "MIMESupport")]
		public byte? MimeSupport { get; set; }

		[XmlElement(ElementName = "MIMETruncation")]
		public byte? MimeTruncation { get; set; }

		[XmlElement(ElementName = "MaxItems")]
		public int? MaxItems { get; set; }

		[XmlIgnore]
		public bool ConflictSpecified
		{
			get
			{
				return this.Conflict != null;
			}
		}

		[XmlIgnore]
		public bool FilterTypeSpecified
		{
			get
			{
				return this.FilterType != null;
			}
		}

		[XmlIgnore]
		public bool MimeSupportSpecified
		{
			get
			{
				return this.MimeSupport != null;
			}
		}

		[XmlIgnore]
		public bool MimeTruncationSpecified
		{
			get
			{
				return this.MimeTruncation != null;
			}
		}

		[XmlIgnore]
		public bool MaxItemsSpecified
		{
			get
			{
				return this.MaxItems != null;
			}
		}
	}
}
