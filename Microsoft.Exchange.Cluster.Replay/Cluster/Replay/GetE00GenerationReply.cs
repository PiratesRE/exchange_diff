using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class GetE00GenerationReply : NetworkChannelMessage
	{
		internal long LogGeneration
		{
			get
			{
				return this.m_logGeneration;
			}
			set
			{
				this.m_logGeneration = value;
			}
		}

		internal GetE00GenerationReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.GetE00GenerationReply)
		{
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_logGeneration);
		}

		internal GetE00GenerationReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.GetE00GenerationReply, packetContent)
		{
			this.m_logGeneration = base.Packet.ExtractInt64();
		}

		private long m_logGeneration;
	}
}
