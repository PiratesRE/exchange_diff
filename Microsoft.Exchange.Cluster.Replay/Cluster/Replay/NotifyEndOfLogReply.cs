using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NotifyEndOfLogReply : NetworkChannelMessage
	{
		internal long EndOfLogGeneration
		{
			get
			{
				return this.m_endOfLogGeneration;
			}
		}

		internal DateTime EndOfLogUtc
		{
			get
			{
				return this.m_endOfLogUtc;
			}
		}

		internal NotifyEndOfLogReply(NetworkChannel channel, NetworkChannelMessage.MessageType msgType, long endOfLogGen, DateTime utc) : base(channel, msgType)
		{
			this.m_endOfLogGeneration = endOfLogGen;
			this.m_endOfLogUtc = utc;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_endOfLogGeneration);
			base.Packet.Append(this.m_endOfLogUtc);
		}

		internal NotifyEndOfLogReply(NetworkChannel channel, NetworkChannelMessage.MessageType msgType, byte[] packetContent) : base(channel, msgType, packetContent)
		{
			this.m_endOfLogGeneration = base.Packet.ExtractInt64();
			this.m_endOfLogUtc = base.Packet.ExtractDateTime();
		}

		private long m_endOfLogGeneration;

		private DateTime m_endOfLogUtc;
	}
}
