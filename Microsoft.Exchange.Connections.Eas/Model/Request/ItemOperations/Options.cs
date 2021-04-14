using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.ItemOperations
{
	[XmlType(Namespace = "ItemOperations", TypeName = "Options")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Options
	{
		[XmlElement(ElementName = "MIMESupport", Namespace = "AirSync")]
		public byte? MimeSupport { get; set; }

		[XmlElement(ElementName = "BodyPreference", Namespace = "AirSyncBase")]
		public BodyPreference BodyPreference { get; set; }

		[XmlElement(ElementName = "Password")]
		public string Password { get; set; }

		[XmlElement(ElementName = "Range")]
		public string Range { get; set; }

		[XmlElement(ElementName = "Schema")]
		public Schema Schema { get; set; }

		[XmlElement(ElementName = "UserName")]
		public string UserName { get; set; }

		[XmlIgnore]
		public bool MimeSupportSpecified
		{
			get
			{
				return this.MimeSupport != null;
			}
		}
	}
}
