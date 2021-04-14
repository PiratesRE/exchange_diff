using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal abstract class MulticastNotification : BasicNotification
	{
		public abstract IEnumerable<Notification> GetFragments();

		protected MulticastNotification() : base(null)
		{
		}
	}
}
