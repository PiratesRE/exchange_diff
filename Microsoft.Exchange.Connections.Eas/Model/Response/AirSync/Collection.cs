using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSync", TypeName = "Collection")]
	public class Collection
	{
		public Collection()
		{
			this.Commands = new Commands();
			this.Responses = new Responses();
		}

		[XmlElement(ElementName = "CollectionId")]
		public string CollectionId { get; set; }

		[XmlElement(ElementName = "MoreAvailable")]
		public EmptyTag MoreAvailable { get; set; }

		[XmlElement(ElementName = "Commands")]
		public Commands Commands { get; set; }

		[XmlElement(ElementName = "Responses")]
		public Responses Responses { get; set; }

		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }

		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }
	}
}
