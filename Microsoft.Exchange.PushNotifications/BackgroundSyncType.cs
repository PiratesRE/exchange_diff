using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal enum BackgroundSyncType
	{
		[EnumMember]
		None,
		[EnumMember]
		Email
	}
}
