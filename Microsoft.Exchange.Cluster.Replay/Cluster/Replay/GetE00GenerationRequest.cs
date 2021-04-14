using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class GetE00GenerationRequest : NetworkChannelDatabaseRequest
	{
		internal GetE00GenerationRequest(NetworkChannel channel, Guid dbGuid) : base(channel, NetworkChannelMessage.MessageType.GetE00GenerationRequest, dbGuid)
		{
		}

		internal GetE00GenerationRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.GetE00GenerationRequest, packetContent)
		{
		}

		public override void Execute()
		{
			base.Channel.MonitoredDatabase.SendE00Generation(base.Channel);
		}
	}
}
