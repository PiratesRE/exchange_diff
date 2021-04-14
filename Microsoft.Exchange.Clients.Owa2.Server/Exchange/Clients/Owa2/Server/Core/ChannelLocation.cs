using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class ChannelLocation : NotificationLocation
	{
		public ChannelLocation(string channelId)
		{
			if (string.IsNullOrEmpty(channelId))
			{
				throw new ArgumentException("The channel id cannot be null or empty string.", "channelId");
			}
			this.channelId = channelId;
		}

		public override KeyValuePair<string, object> GetEventData()
		{
			return new KeyValuePair<string, object>("ChannelId", this.channelId);
		}

		public override int GetHashCode()
		{
			return ChannelLocation.TypeHashCode ^ this.channelId.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			ChannelLocation channelLocation = obj as ChannelLocation;
			return channelLocation != null && this.channelId.Equals(channelLocation.channelId);
		}

		public override string ToString()
		{
			return this.channelId;
		}

		private const string EventKey = "ChannelId";

		private static readonly int TypeHashCode = typeof(ChannelLocation).GetHashCode();

		private readonly string channelId;
	}
}
