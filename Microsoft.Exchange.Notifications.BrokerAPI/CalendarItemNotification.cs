using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public sealed class CalendarItemNotification : RowNotification
	{
		public CalendarItemNotification() : base(NotificationType.CalendarItem)
		{
		}
	}
}
