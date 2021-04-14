using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Connections.Eas.Model.Extensions
{
	public enum FlagStatus
	{
		[XmlEnum("0")]
		NotFlagged,
		[XmlEnum("1")]
		Complete,
		[XmlEnum("2")]
		Flagged
	}
}
