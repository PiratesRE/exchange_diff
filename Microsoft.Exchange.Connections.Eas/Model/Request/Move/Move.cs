using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.Move
{
	[XmlType(Namespace = "Move", TypeName = "Move")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Move
	{
		[XmlElement(ElementName = "SrcMsgId")]
		public string SrcMsgId { get; set; }

		[XmlElement(ElementName = "SrcFldId")]
		public string SrcFldId { get; set; }

		[XmlElement(ElementName = "DstFldId")]
		public string DstFldId { get; set; }
	}
}
