using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSyncBase
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSyncBase", TypeName = "Attachments")]
	public class Attachments
	{
		[XmlElement(ElementName = "Attachment")]
		public Attachment Attachment { get; set; }
	}
}
