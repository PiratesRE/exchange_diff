using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	[XmlRoot(Namespace = "", ElementName = "TransportQueueLogs")]
	[Serializable]
	public class TransportQueueLogs : List<TransportQueueLog>
	{
		internal static readonly XmlSerializer Serializer = new XmlSerializer(typeof(TransportQueueLogs));
	}
}
