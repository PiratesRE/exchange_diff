using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ProgressCiFileReply : NetworkChannelMessage
	{
		internal ProgressCiFileReply(NetworkChannel channel, int progress) : base(channel, NetworkChannelMessage.MessageType.ProgressCiFileReply)
		{
			this.progress = progress;
		}

		internal ProgressCiFileReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.ProgressCiFileReply, packetContent)
		{
			this.progress = base.Packet.ExtractInt32();
		}

		internal int Progress
		{
			get
			{
				return this.progress;
			}
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.progress);
		}

		private readonly int progress;
	}
}
