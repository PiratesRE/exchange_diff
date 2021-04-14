using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class CopyLogReply : NetworkChannelFileTransferReply
	{
		internal long ThisLogGeneration
		{
			get
			{
				return this.m_thisLogGeneration;
			}
			set
			{
				this.m_thisLogGeneration = value;
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

		internal CopyLogReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.CopyLogReply)
		{
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.ThisLogGeneration);
			base.Packet.Append(this.EndOfLogGeneration);
			base.Packet.Append(this.EndOfLogUtc);
		}

		internal CopyLogReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.CopyLogReply, packetContent)
		{
			this.m_thisLogGeneration = base.Packet.ExtractInt64();
			this.m_endOfLogGeneration = base.Packet.ExtractInt64();
			this.m_endOfLogUtc = base.Packet.ExtractDateTime();
		}

		private long m_thisLogGeneration;

		private long m_endOfLogGeneration;

		private DateTime m_endOfLogUtc;
	}
}
