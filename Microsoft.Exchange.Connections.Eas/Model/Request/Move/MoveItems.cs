using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.Move
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Move", TypeName = "MoveItems")]
	public class MoveItems
	{
		[XmlElement(ElementName = "Move")]
		public Move[] Moves { get; set; }
	}
}
