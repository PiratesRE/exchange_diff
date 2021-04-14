using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class EwsNotificationType
	{
		public EwsNotificationType()
		{
			this.eventList = new List<BaseNotificationEventType>();
			this.eventTypeList = new List<NotificationTypeEnum>();
		}

		[DataMember(EmitDefaultValue = false)]
		public string SubscriptionId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string PreviousWatermark { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool MoreEvents
		{
			get
			{
				return this.moreEvents;
			}
			set
			{
				this.MoreEventsSpecified = true;
				this.moreEvents = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool MoreEventsSpecified { get; set; }

		[XmlElement("CreatedEvent", typeof(BaseObjectChangedEventType))]
		[XmlElement("FreeBusyChangedEvent", typeof(BaseObjectChangedEventType))]
		[XmlElement("ModifiedEvent", typeof(ModifiedEventType))]
		[XmlElement("MovedEvent", typeof(MovedCopiedEventType))]
		[XmlElement("NewMailEvent", typeof(BaseObjectChangedEventType))]
		[XmlElement("StatusEvent", typeof(BaseNotificationEventType))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[DataMember(EmitDefaultValue = false)]
		[XmlElement("CopiedEvent", typeof(MovedCopiedEventType))]
		[XmlElement("DeletedEvent", typeof(BaseObjectChangedEventType))]
		public BaseNotificationEventType[] Events
		{
			get
			{
				return this.eventList.ToArray();
			}
			set
			{
				this.eventList = new List<BaseNotificationEventType>(value);
			}
		}

		[XmlIgnore]
		[XmlElement("ItemsElementName")]
		[IgnoreDataMember]
		public NotificationTypeEnum[] ItemsElementName
		{
			get
			{
				return this.eventTypeList.ToArray();
			}
			set
			{
				this.eventTypeList = new List<NotificationTypeEnum>(value);
			}
		}

		public void AddNotificationEvent(BaseNotificationEventType notificationEvent)
		{
			this.eventList.Add(notificationEvent);
			this.eventTypeList.Add(notificationEvent.NotificationType);
		}

		private bool moreEvents;

		private List<BaseNotificationEventType> eventList;

		private List<NotificationTypeEnum> eventTypeList;
	}
}
