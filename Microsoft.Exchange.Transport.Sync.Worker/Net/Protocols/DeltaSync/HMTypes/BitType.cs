using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMTypes
{
	[Serializable]
	public enum BitType
	{
		[XmlEnum(Name = "0")]
		zero,
		[XmlEnum(Name = "1")]
		one
	}
}
