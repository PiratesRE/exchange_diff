using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.HMSync
{
	[XmlRoot(ElementName = "DeletesAsMoves", Namespace = "HMSYNC:", IsNullable = false)]
	[Serializable]
	public class DeletesAsMoves
	{
	}
}
