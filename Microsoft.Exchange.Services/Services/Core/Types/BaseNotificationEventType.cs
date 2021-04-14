using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(BaseObjectChangedEventType))]
	[KnownType(typeof(ModifiedEventType))]
	[XmlInclude(typeof(MovedCopiedEventType))]
	[XmlInclude(typeof(ModifiedEventType))]
	[KnownType(typeof(MovedCopiedEventType))]
	[XmlInclude(typeof(BaseObjectChangedEventType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "BaseNotificationEvent", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class BaseNotificationEventType
	{
		public BaseNotificationEventType()
		{
			this.notificationType = NotificationTypeEnum.StatusEvent;
		}

		public BaseNotificationEventType(NotificationTypeEnum notificationType)
		{
			this.notificationType = notificationType;
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string Watermark { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public NotificationTypeEnum NotificationType
		{
			get
			{
				return this.notificationType;
			}
		}

		[DataMember(Name = "NotificationType", IsRequired = true, Order = 1)]
		[XmlIgnore]
		public string NotificationTypeString
		{
			get
			{
				return EnumUtilities.ToString<NotificationTypeEnum>(this.notificationType);
			}
			set
			{
				this.notificationType = EnumUtilities.Parse<NotificationTypeEnum>(value);
			}
		}

		private NotificationTypeEnum notificationType;
	}
}
