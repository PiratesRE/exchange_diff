using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations
{
	[XmlRoot(Namespace = "ItemOperations", ElementName = "Response")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "ItemOperations", TypeName = "Response")]
	public class Response
	{
		[XmlElement(ElementName = "Fetch")]
		public Fetch[] Fetches { get; set; }
	}
}
