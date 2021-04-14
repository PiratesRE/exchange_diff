using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase
{
	public enum ConflictResolution
	{
		[XmlEnum("0")]
		KeepClientVersion,
		[XmlEnum("1")]
		KeepServerVersion
	}
}
