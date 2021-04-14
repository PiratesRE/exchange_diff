using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogCopyServerStatusMsg : NetworkChannelMessage
	{
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

		internal int EndOfLogSector
		{
			get
			{
				return this.m_endOfLogSector;
			}
			set
			{
				this.m_endOfLogSector = value;
			}
		}

		internal int EndOfLogByteOffset
		{
			get
			{
				return this.m_endOfLogByteOffset;
			}
			set
			{
				this.m_endOfLogByteOffset = value;
			}
		}

		internal LogCopyServerStatusMsg(NetworkChannel channel, FullEndOfLog eol) : base(channel, NetworkChannelMessage.MessageType.LogCopyServerStatus)
		{
			this.EndOfLogGeneration = eol.Generation;
			this.EndOfLogUtc = eol.Utc;
			this.EndOfLogSector = eol.Sector;
			this.EndOfLogByteOffset = eol.ByteOffset;
			if (eol.PositionInE00)
			{
				this.FlagsUsed |= LogCopyServerStatusMsg.Flags.GranularStatus;
			}
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append((ulong)this.FlagsUsed);
			base.Packet.Append(this.RequestAckCounter);
			base.Packet.Append(this.ReplyAckCounter);
			base.Packet.Append(this.EndOfLogGeneration);
			base.Packet.Append(this.EndOfLogUtc);
			base.Packet.Append(this.EndOfLogSector);
			base.Packet.Append(this.EndOfLogByteOffset);
		}

		internal LogCopyServerStatusMsg(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.LogCopyServerStatus, packetContent)
		{
			this.FlagsUsed = (LogCopyServerStatusMsg.Flags)base.Packet.ExtractInt64();
			this.RequestAckCounter = base.Packet.ExtractInt64();
			this.ReplyAckCounter = base.Packet.ExtractInt64();
			this.m_endOfLogGeneration = base.Packet.ExtractInt64();
			this.m_endOfLogUtc = base.Packet.ExtractDateTime();
			this.m_endOfLogSector = base.Packet.ExtractInt32();
			this.m_endOfLogByteOffset = base.Packet.ExtractInt32();
		}

		public LogCopyServerStatusMsg.Flags FlagsUsed;

		public long RequestAckCounter;

		public long ReplyAckCounter;

		private long m_endOfLogGeneration;

		private DateTime m_endOfLogUtc;

		private int m_endOfLogSector;

		private int m_endOfLogByteOffset;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL,
			AckRequested = 1UL,
			GranularStatus = 2UL,
			GranularCompletionsDisabled = 8UL
		}
	}
}
