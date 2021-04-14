using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class UnseenItemNotificationPayload : NotificationPayloadBase
	{
		[DataMember(Name = "UnseenData", IsRequired = true)]
		public UnseenDataType UnseenData { get; set; }
	}
}
