using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase
{
	public enum MimeTruncationType
	{
		[XmlEnum("0")]
		TruncateAll,
		[XmlEnum("1")]
		Truncate4K,
		[XmlEnum("2")]
		Truncate5K,
		[XmlEnum("3")]
		Truncate7K,
		[XmlEnum("4")]
		Truncate10K,
		[XmlEnum("5")]
		Truncate20K,
		[XmlEnum("6")]
		Truncate50K,
		[XmlEnum("7")]
		Truncate100K,
		[XmlEnum("8")]
		NoTruncate
	}
}
