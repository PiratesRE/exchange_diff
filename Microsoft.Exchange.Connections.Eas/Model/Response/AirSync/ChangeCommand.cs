using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSync", TypeName = "ChangeCommand")]
	public class ChangeCommand : Command
	{
		[XmlElement(ElementName = "Class")]
		public string Class { get; set; }

		[XmlElement(ElementName = "ServerId")]
		public string ServerId { get; set; }

		[XmlElement(ElementName = "ApplicationData")]
		public ApplicationData ApplicationData { get; set; }
	}
}
