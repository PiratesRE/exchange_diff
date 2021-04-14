using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class CalendarItemSubscription : RowSubscription
	{
		public CalendarItemSubscription() : base(NotificationType.CalendarItem)
		{
		}
	}
}
