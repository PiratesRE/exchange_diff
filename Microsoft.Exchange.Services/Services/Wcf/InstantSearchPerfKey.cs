using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf
{
	[XmlType(TypeName = "InstantSearchPerfKey", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public enum InstantSearchPerfKey
	{
		Unknown,
		ServiceCommandInvocationTimeStamp,
		InstantSearchAPIMethodInvocationTimeStamp,
		InstantSearchAPICallback,
		NotificationHandlerPayloadDeliveryTimeStamp,
		NotificationQueuedTime,
		NotificationPickupFromQueueTime,
		NotificationSerializationTime
	}
}
