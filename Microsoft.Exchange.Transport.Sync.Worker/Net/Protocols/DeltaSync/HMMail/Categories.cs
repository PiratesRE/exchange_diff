using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail
{
	[XmlRoot(ElementName = "Categories", Namespace = "HMMAIL:", IsNullable = false)]
	[Serializable]
	public class Categories : stringWithCharSetType
	{
	}
}
