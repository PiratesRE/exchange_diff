using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase
{
	public enum MimeSupport
	{
		[XmlEnum("0")]
		NeverSendMime,
		[XmlEnum("1")]
		SendMimeForSMime,
		[XmlEnum("2")]
		SendMimeForAll
	}
}
