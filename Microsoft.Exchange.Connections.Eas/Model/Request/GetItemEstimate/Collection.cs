using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSync;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.GetItemEstimate
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "GetItemEstimate", TypeName = "Collection")]
	public class Collection
	{
		public Collection()
		{
			this.Options = new List<Options>();
		}

		[XmlElement(ElementName = "SyncKey", Namespace = "AirSync")]
		public string SyncKey { get; set; }

		[XmlElement(ElementName = "CollectionId")]
		public string CollectionId { get; set; }

		[XmlElement(ElementName = "ConversationMode")]
		public object ConversationMode { get; set; }

		[XmlElement(ElementName = "Options", Namespace = "AirSync")]
		public List<Options> Options { get; set; }
	}
}
