using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMFolder
{
	[Serializable]
	public enum bitType
	{
		[XmlEnum(Name = "0")]
		zero,
		[XmlEnum(Name = "1")]
		one
	}
}
