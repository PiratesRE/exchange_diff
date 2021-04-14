using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "Sync")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Sync
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }

		[XmlIgnore]
		public bool StatusSpecified { get; set; }

		[XmlElement(ElementName = "Collections", Type = typeof(List<Collection>))]
		public List<Collection> Collections { get; set; }
	}
}
