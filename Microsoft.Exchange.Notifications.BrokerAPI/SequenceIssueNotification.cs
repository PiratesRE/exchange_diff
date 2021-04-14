using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[KnownType(typeof(DroppedNotification))]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public abstract class SequenceIssueNotification : BaseNotification
	{
		public SequenceIssueNotification(NotificationType notificationType) : base(notificationType)
		{
		}
	}
}
