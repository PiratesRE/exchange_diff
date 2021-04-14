using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.GetItemEstimate
{
	[XmlRoot(Namespace = "GetItemEstimate", ElementName = "Response")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "GetItemEstimate", TypeName = "Response")]
	public class Response
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }

		[XmlElement(ElementName = "Collection")]
		public Collection Collection { get; set; }
	}
}
