using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class OutlookNotificationRecipient : BasicNotificationRecipient
	{
		public OutlookNotificationRecipient(string appId, string deviceId) : base(appId, deviceId)
		{
		}
	}
}
