using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[KnownType(typeof(SequenceIssueNotification))]
	[KnownType(typeof(ApplicationNotification))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public abstract class BaseNotification
	{
		public BaseNotification(NotificationType notificationType)
		{
			this.NotificationType = notificationType;
		}

		[DataMember(EmitDefaultValue = false)]
		public NotificationType NotificationType { get; set; }

		public BaseNotification Clone()
		{
			return (BaseNotification)base.MemberwiseClone();
		}
	}
}
