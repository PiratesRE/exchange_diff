using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class GranularLogCompleteMsg : NetworkChannelMessage
	{
		internal GranularLogCompleteMsg(NetworkChannel channel, GranularLogCloseData closeData) : base(channel, NetworkChannelMessage.MessageType.GranularLogComplete)
		{
			this.ChecksumUsed = closeData.ChecksumUsed;
			this.Generation = closeData.Generation;
			this.LastWriteUtc = closeData.LastWriteUtc;
			this.ChecksumBytes = closeData.ChecksumBytes;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append((ulong)this.FlagsUsed);
			base.Packet.Append(this.RequestAckCounter);
			base.Packet.Append(this.ReplyAckCounter);
			base.Packet.Append((uint)this.ChecksumUsed);
			base.Packet.Append(this.Generation);
			base.Packet.Append(this.LastWriteUtc);
			this.m_checksumLengthInBytes = ((this.ChecksumBytes == null) ? 0 : this.ChecksumBytes.Length);
			base.Packet.Append(this.m_checksumLengthInBytes);
			if (this.m_checksumLengthInBytes > 0)
			{
				base.Packet.Append(this.ChecksumBytes);
			}
		}

		internal GranularLogCompleteMsg(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.GranularLogComplete, packetContent)
		{
			this.FlagsUsed = (GranularLogCompleteMsg.Flags)base.Packet.ExtractInt64();
			this.RequestAckCounter = base.Packet.ExtractInt64();
			this.ReplyAckCounter = base.Packet.ExtractInt64();
			this.ChecksumUsed = (GranularLogCloseData.ChecksumAlgorithm)base.Packet.ExtractInt32();
			this.Generation = base.Packet.ExtractInt64();
			this.LastWriteUtc = base.Packet.ExtractDateTime();
			this.m_checksumLengthInBytes = base.Packet.ExtractInt32();
			if (this.m_checksumLengthInBytes > 0)
			{
				this.ChecksumBytes = base.Packet.ExtractBytes(this.m_checksumLengthInBytes);
			}
		}

		public GranularLogCompleteMsg.Flags FlagsUsed;

		public long RequestAckCounter;

		public long ReplyAckCounter;

		public GranularLogCloseData.ChecksumAlgorithm ChecksumUsed;

		public long Generation;

		public DateTime LastWriteUtc;

		private int m_checksumLengthInBytes;

		public byte[] ChecksumBytes;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL
		}
	}
}
