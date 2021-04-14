using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "Add")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class AddCommand : Command
	{
		[XmlElement(ElementName = "Class")]
		public string Class { get; set; }

		[XmlElement(ElementName = "ClientId")]
		public string ClientId { get; set; }

		[XmlElement(ElementName = "ApplicationData")]
		public ApplicationData ApplicationData { get; set; }
	}
}
