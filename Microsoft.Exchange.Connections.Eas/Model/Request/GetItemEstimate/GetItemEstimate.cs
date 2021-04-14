using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.GetItemEstimate
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "GetItemEstimate", TypeName = "GetItemEstimate")]
	public class GetItemEstimate
	{
		[XmlElement(ElementName = "Collections", Type = typeof(List<Collection>))]
		public List<Collection> Collections { get; set; }
	}
}
