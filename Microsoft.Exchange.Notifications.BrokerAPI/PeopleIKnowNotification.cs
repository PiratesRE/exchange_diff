using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public sealed class PeopleIKnowNotification : RowNotification
	{
		public PeopleIKnowNotification() : base(NotificationType.PeopleIKnow)
		{
		}
	}
}
