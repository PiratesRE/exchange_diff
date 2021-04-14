using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.ComposeMail
{
	[XmlType(Namespace = "ComposeMail", TypeName = "SendMail")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class SendMail
	{
		[XmlElement(ElementName = "Status")]
		public byte Status { get; set; }
	}
}
