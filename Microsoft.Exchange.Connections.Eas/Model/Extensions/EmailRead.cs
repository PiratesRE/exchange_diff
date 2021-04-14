using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Connections.Eas.Model.Extensions
{
	public enum EmailRead : byte
	{
		[XmlEnum("0")]
		Unread,
		[XmlEnum("1")]
		Read
	}
}
