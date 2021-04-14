using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RemoteNotificationPayload : NotificationPayloadBase
	{
		public RemoteNotificationPayload(int notificationsCount, string remotePayload, string[] channelIds)
		{
			this.NotificationsCount = notificationsCount;
			this.RemotePayload = remotePayload;
			this.ChannelIds = channelIds;
		}

		[DataMember]
		public string RemotePayload { get; set; }

		[DataMember]
		public int NotificationsCount { get; set; }

		[DataMember]
		public string[] ChannelIds { get; set; }
	}
}
