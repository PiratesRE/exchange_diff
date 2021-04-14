using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedPageReaderRollLogFileReply : NetworkChannelMessage
	{
		internal SeedPageReaderRollLogFileReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderRollLogFileReply)
		{
		}

		protected override void Serialize()
		{
			base.Serialize();
		}

		internal SeedPageReaderRollLogFileReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderRollLogFileReply, packetContent)
		{
		}
	}
}
