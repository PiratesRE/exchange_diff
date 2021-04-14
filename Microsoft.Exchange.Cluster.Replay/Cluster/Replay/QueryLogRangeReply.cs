using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class QueryLogRangeReply : NetworkChannelMessage
	{
		internal long FirstAvailableGeneration
		{
			get
			{
				return this.m_firstAvailableGeneration;
			}
			set
			{
				this.m_firstAvailableGeneration = value;
			}
		}

		internal long EndOfLogGeneration
		{
			get
			{
				return this.m_endOfLogGeneration;
			}
			set
			{
				this.m_endOfLogGeneration = value;
			}
		}

		internal DateTime EndOfLogUtc
		{
			get
			{
				return this.m_endOfLogUtc;
			}
			set
			{
				this.m_endOfLogUtc = value;
			}
		}

		internal QueryLogRangeReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.QueryLogRangeReply)
		{
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_firstAvailableGeneration);
			base.Packet.Append(this.m_endOfLogGeneration);
			base.Packet.Append(this.m_endOfLogUtc);
		}

		internal QueryLogRangeReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.QueryLogRangeReply, packetContent)
		{
			this.m_firstAvailableGeneration = base.Packet.ExtractInt64();
			this.m_endOfLogGeneration = base.Packet.ExtractInt64();
			this.m_endOfLogUtc = base.Packet.ExtractDateTime();
		}

		private long m_firstAvailableGeneration;

		private long m_endOfLogGeneration;

		private DateTime m_endOfLogUtc;
	}
}
