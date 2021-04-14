using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class RemoteChannelInfo : IEquatable<RemoteChannelInfo>
	{
		public string ChannelId { get; private set; }

		public string User { get; private set; }

		internal string EndpointTestOverride { get; set; }

		public RemoteChannelInfo(string channelId, string user)
		{
			this.ChannelId = channelId;
			this.User = user;
		}

		public override bool Equals(object obj)
		{
			RemoteChannelInfo remoteChannelInfo = obj as RemoteChannelInfo;
			return remoteChannelInfo != null && this.Equals(remoteChannelInfo);
		}

		public bool Equals(RemoteChannelInfo other)
		{
			return other != null && string.Equals(this.ChannelId, other.ChannelId, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			if (this.ChannelId == null)
			{
				return base.GetHashCode();
			}
			return this.ChannelId.ToUpperInvariant().GetHashCode();
		}
	}
}
