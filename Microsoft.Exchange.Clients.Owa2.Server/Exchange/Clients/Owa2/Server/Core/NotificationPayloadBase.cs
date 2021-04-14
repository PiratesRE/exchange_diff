using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NotificationPayloadBase
	{
		public string SubscriptionId
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		[DataMember(Name = "EventType", EmitDefaultValue = false)]
		public string EventTypeString
		{
			get
			{
				return this.EventType.ToString();
			}
			set
			{
				this.EventType = (QueryNotificationType)Enum.Parse(typeof(QueryNotificationType), value);
			}
		}

		[IgnoreDataMember]
		internal QueryNotificationType EventType { get; set; }

		[IgnoreDataMember]
		internal NotificationLocation Source { get; set; }

		[IgnoreDataMember]
		internal DateTime? CreatedTime { get; set; }

		[IgnoreDataMember]
		internal DateTime? ReceivedTime { get; set; }

		[IgnoreDataMember]
		internal DateTime? QueuedTime { get; set; }

		[DataMember(EmitDefaultValue = false)]
		private string id = "notification";
	}
}
