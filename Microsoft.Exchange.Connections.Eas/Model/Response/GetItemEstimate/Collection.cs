using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.GetItemEstimate
{
	[XmlRoot(Namespace = "GetItemEstimate", ElementName = "Collection")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "GetItemEstimate", TypeName = "Collection")]
	public class Collection
	{
		[XmlElement(ElementName = "CollectionId")]
		public string CollectionId { get; set; }

		[XmlElement(ElementName = "Estimate")]
		public int Estimate { get; set; }
	}
}
