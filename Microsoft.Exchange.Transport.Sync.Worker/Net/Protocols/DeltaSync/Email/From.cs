using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.Email
{
	[XmlRoot(ElementName = "From", Namespace = "EMAIL:", IsNullable = false)]
	[Serializable]
	public class From : stringWithEncodingType2
	{
	}
}
