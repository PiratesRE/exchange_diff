using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class BrokerSubscription
	{
		[DataMember(EmitDefaultValue = false)]
		public Guid SubscriptionId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ConsumerId ConsumerId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ChannelId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public DateTime Expiration { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public NotificationParticipant Sender { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public NotificationParticipant Receiver { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public BaseSubscription Parameters { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsValid
		{
			get
			{
				return this.Parameters != null && this.Parameters.IsValid;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		internal IBrokerMailboxData MailboxData { get; set; }

		[IgnoreDataMember]
		internal StoreObjectId StoreId { get; set; }

		[IgnoreDataMember]
		internal INotificationHandler Handler { get; set; }

		[IgnoreDataMember]
		internal int LastSequenceNumber
		{
			get
			{
				return this.lastSequenceNumber;
			}
			private set
			{
				this.lastSequenceNumber = value;
			}
		}

		public static BrokerSubscription FromJson(string jsonString)
		{
			return JsonConverter.Deserialize<BrokerSubscription>(jsonString, BrokerSubscription.knownTypes, JsonConverter.RoundTripDateTimeFormat);
		}

		public string ToJson()
		{
			return JsonConverter.Serialize<BrokerSubscription>(this, BrokerSubscription.knownTypes, JsonConverter.RoundTripDateTimeFormat);
		}

		internal int GetNextSequenceNumber()
		{
			return Interlocked.Increment(ref this.lastSequenceNumber);
		}

		internal void TrimForSubscribeRequest()
		{
			this.Sender = this.Sender.AsNotificationSender();
			this.Receiver = this.Receiver.AsNotificationReceiver();
		}

		internal void TrimForUnsubscribeRequest()
		{
			this.Sender = this.Sender.AsNotificationSender();
		}

		private static readonly Type[] knownTypes = new Type[]
		{
			typeof(ConsumerId),
			typeof(NotificationType),
			typeof(BaseSubscription)
		};

		private int lastSequenceNumber;
	}
}
