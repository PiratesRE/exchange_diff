using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TestHealthRequest : NetworkChannelMessage, INetworkChannelRequest
	{
		internal TestHealthRequest(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.TestHealthRequest)
		{
		}

		internal TestHealthRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.TestHealthRequest, packetContent)
		{
		}

		public void Execute()
		{
			TestHealthReply testHealthReply = new TestHealthReply(base.Channel);
			testHealthReply.Send();
		}
	}
}
