using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.Email
{
	[XmlRoot(ElementName = "Subject", Namespace = "EMAIL:", IsNullable = false)]
	[Serializable]
	public class Subject : stringWithEncodingType2
	{
	}
}
