using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.ItemOperations;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.ItemOperations
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlRoot(ElementName = "ItemOperations", Namespace = "ItemOperations", IsNullable = false)]
	public class ItemOperationsRequest : ItemOperations
	{
	}
}
