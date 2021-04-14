using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class PusherQueuePayload
	{
		public NotificationPayloadBase Payload { get; private set; }

		public IEnumerable<string> ChannelIds { get; private set; }

		public PusherQueuePayload(NotificationPayloadBase payload, IEnumerable<string> channelIds)
		{
			this.Payload = payload;
			this.ChannelIds = channelIds;
		}
	}
}
