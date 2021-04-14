using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Move
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Move", TypeName = "Response")]
	public class Response
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }

		[XmlElement(ElementName = "SrcMsgId")]
		public string SrcMsgId { get; set; }

		[XmlElement(ElementName = "DstMsgId")]
		public string DstMsgId { get; set; }
	}
}
