using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TestHealthReply : NetworkChannelMessage
	{
		internal TestHealthReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.TestHealthReply)
		{
		}

		internal TestHealthReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.TestHealthReply, packetContent)
		{
		}
	}
}
