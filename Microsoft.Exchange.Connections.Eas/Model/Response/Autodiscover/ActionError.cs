using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Autodiscover
{
	[XmlType(TypeName = "ActionError")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ActionError
	{
		[XmlElement(ElementName = "Status")]
		public string Status { get; set; }

		[XmlElement(ElementName = "Message")]
		public string Message { get; set; }

		[XmlElement(ElementName = "DebugData")]
		public string DebugData { get; set; }

		[XmlElement(ElementName = "ErrorCode")]
		public int ErrorCode { get; set; }
	}
}
