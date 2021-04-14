using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "AddCommand")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class AddCommand : Command
	{
		[XmlElement(ElementName = "ServerId")]
		public string ServerId { get; set; }

		[XmlElement(ElementName = "ApplicationData")]
		public ApplicationData ApplicationData { get; set; }
	}
}
