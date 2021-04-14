using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "Fetch")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class FetchCommand : Command
	{
		[XmlElement(ElementName = "ServerId")]
		public string ServerId { get; set; }
	}
}
