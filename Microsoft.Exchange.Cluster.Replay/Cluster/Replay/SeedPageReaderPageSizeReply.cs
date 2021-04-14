using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedPageReaderPageSizeReply : NetworkChannelMessage
	{
		public long PageSize
		{
			get
			{
				return this.m_pageSize;
			}
			set
			{
				this.m_pageSize = value;
			}
		}

		internal SeedPageReaderPageSizeReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderPageSizeReply)
		{
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_pageSize);
		}

		internal SeedPageReaderPageSizeReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderPageSizeReply, packetContent)
		{
			this.m_pageSize = base.Packet.ExtractInt64();
		}

		private long m_pageSize;
	}
}
