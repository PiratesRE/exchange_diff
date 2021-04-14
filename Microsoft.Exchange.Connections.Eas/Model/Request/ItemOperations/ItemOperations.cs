using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.ItemOperations
{
	[XmlType(Namespace = "ItemOperations", TypeName = "ItemOperations")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ItemOperations
	{
		[XmlElement(ElementName = "Fetch")]
		public Fetch[] Fetches { get; set; }
	}
}
