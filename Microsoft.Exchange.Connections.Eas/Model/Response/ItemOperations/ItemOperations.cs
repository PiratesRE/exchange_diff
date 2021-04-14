using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations
{
	[XmlType(Namespace = "ItemOperations", TypeName = "ItemOperations")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ItemOperations
	{
		[XmlElement(ElementName = "Response")]
		public Response Response { get; set; }

		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }
	}
}
