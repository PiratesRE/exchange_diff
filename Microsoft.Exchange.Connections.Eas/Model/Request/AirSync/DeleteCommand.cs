using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSync", TypeName = "Delete")]
	public class DeleteCommand : Command
	{
		[XmlElement(ElementName = "ServerId")]
		public string ServerId { get; set; }
	}
}
