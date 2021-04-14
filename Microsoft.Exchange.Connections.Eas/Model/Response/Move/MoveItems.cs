using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Move
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Move", TypeName = "MoveItems")]
	public class MoveItems
	{
		[XmlElement(ElementName = "Response")]
		public Response[] Responses { get; set; }
	}
}
