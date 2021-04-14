using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SeedCiFileReply : NetworkChannelMessage
	{
		internal SeedCiFileReply(NetworkChannel channel, string handle) : base(channel, NetworkChannelMessage.MessageType.SeedCiFileReply)
		{
			this.handle = handle;
		}

		internal SeedCiFileReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedCiFileReply, packetContent)
		{
			this.handle = base.Packet.ExtractString();
		}

		internal string Handle
		{
			get
			{
				return this.handle;
			}
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.handle);
		}

		private readonly string handle;
	}
}
