using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[KnownType(typeof(BaseNotification))]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(ConsumerId))]
	[Serializable]
	public class BrokerNotification
	{
		[DataMember(EmitDefaultValue = false)]
		public Guid NotificationId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ConsumerId ConsumerId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Guid SubscriptionId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ChannelId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int SequenceNumber { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Guid ReceiverMailboxGuid { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ReceiverMailboxSmtp { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public DateTime CreationTime { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public BaseNotification Payload { get; set; }

		public static BrokerNotification FromJson(string jsonString)
		{
			return JsonConverter.Deserialize<BrokerNotification>(jsonString, null, JsonConverter.RoundTripDateTimeFormat);
		}

		public string ToJson()
		{
			return JsonConverter.Serialize<BrokerNotification>(this, null, JsonConverter.RoundTripDateTimeFormat);
		}
	}
}
