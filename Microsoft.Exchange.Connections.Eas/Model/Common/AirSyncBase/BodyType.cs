using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase
{
	public enum BodyType
	{
		[XmlEnum("0")]
		NoType,
		[XmlEnum("1")]
		PlainText,
		[XmlEnum("2")]
		HTML,
		[XmlEnum("3")]
		RTF,
		[XmlEnum("4")]
		MIME
	}
}
