using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[KnownType(typeof(NewMailNotification))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[KnownType(typeof(RowNotification))]
	[KnownType(typeof(UnseenCountNotification))]
	[Serializable]
	public abstract class ApplicationNotification : BaseNotification
	{
		protected ApplicationNotification(NotificationType notificationType) : base(notificationType)
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public string ConsumerSubscriptionId { get; set; }

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
	}
}
