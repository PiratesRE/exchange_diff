using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(TypeName = "Error")]
	public class Error
	{
		[XmlAttribute(AttributeName = "Id")]
		public string Id { get; set; }

		[XmlAttribute(AttributeName = "Time")]
		public string Time { get; set; }

		[XmlElement(ElementName = "ErrorCode")]
		public int ErrorCode { get; set; }

		[XmlElement(ElementName = "Message")]
		public string Message { get; set; }

		[XmlElement(ElementName = "DebugData")]
		public string DebugData { get; set; }
	}
}
