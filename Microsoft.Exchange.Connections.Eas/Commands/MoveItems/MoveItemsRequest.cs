using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.Move;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.MoveItems
{
	[XmlRoot(ElementName = "MoveItems", Namespace = "Move", IsNullable = false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class MoveItemsRequest : MoveItems
	{
	}
}
