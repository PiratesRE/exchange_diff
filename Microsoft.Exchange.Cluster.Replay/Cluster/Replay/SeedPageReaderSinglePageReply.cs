using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedPageReaderSinglePageReply : NetworkChannelMessage
	{
		internal long PageNumber
		{
			get
			{
				return this.m_pageno;
			}
			set
			{
				this.m_pageno = value;
			}
		}

		internal long LowGeneration
		{
			get
			{
				return this.m_lowGeneration;
			}
			set
			{
				this.m_lowGeneration = value;
			}
		}

		internal long HighGeneration
		{
			get
			{
				return this.m_highGeneration;
			}
			set
			{
				this.m_highGeneration = value;
			}
		}

		internal byte[] PageBytes
		{
			get
			{
				return this.m_pageBytes;
			}
			set
			{
				this.m_pageBytes = value;
			}
		}

		internal SeedPageReaderSinglePageReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderSinglePageReply)
		{
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_pageno);
			base.Packet.Append(this.m_lowGeneration);
			base.Packet.Append(this.m_highGeneration);
			int val = this.m_pageBytes.Length;
			base.Packet.Append(val);
			base.Packet.Append(this.m_pageBytes);
		}

		internal SeedPageReaderSinglePageReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderSinglePageReply, packetContent)
		{
			this.m_pageno = base.Packet.ExtractInt64();
			this.m_lowGeneration = base.Packet.ExtractInt64();
			this.m_highGeneration = base.Packet.ExtractInt64();
			int len = base.Packet.ExtractInt32();
			this.m_pageBytes = base.Packet.ExtractBytes(len);
		}

		private long m_pageno;

		private long m_lowGeneration;

		private long m_highGeneration;

		private byte[] m_pageBytes;
	}
}
