using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase
{
	[XmlType(Namespace = "AirSyncBase", TypeName = "Attachments")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Attachments
	{
		[XmlElement(ElementName = "Attachment")]
		public Attachment Attachment { get; set; }
	}
}
