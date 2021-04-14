using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedDatabaseFileReply : NetworkChannelFileTransferReply
	{
		internal SeedDatabaseFileReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.SeedDatabaseFileReply)
		{
		}

		internal SeedDatabaseFileReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedDatabaseFileReply, packetContent)
		{
		}

		protected override void Serialize()
		{
			base.Serialize();
		}
	}
}
