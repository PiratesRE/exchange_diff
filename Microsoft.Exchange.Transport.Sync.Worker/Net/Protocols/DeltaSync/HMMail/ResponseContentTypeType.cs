using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[Serializable]
	public enum ResponseContentTypeType
	{
		[XmlEnum(Name = "mtom")]
		mtom
	}
}
