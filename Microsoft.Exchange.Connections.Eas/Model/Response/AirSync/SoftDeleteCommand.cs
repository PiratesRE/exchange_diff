using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSync", TypeName = "SoftDeleteCommand")]
	public class SoftDeleteCommand : Command
	{
		[XmlElement(ElementName = "ServerId")]
		public string ServerId { get; set; }
	}
}
