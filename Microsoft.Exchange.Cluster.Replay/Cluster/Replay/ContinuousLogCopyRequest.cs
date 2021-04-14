using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ContinuousLogCopyRequest : NetworkChannelDatabaseRequest
	{
		public long FirstGeneration
		{
			get
			{
				return this.m_firstGeneration;
			}
		}

		public long LastGeneration
		{
			get
			{
				return this.m_lastGeneration;
			}
		}

		public ContinuousLogCopyRequest.Flags ReplicationFlags
		{
			get
			{
				return this.FlagsUsed;
			}
			set
			{
				this.FlagsUsed = value;
			}
		}

		public bool ForAcll
		{
			get
			{
				return (this.FlagsUsed & ContinuousLogCopyRequest.Flags.ForAcll) != ContinuousLogCopyRequest.Flags.None;
			}
		}

		internal ContinuousLogCopyRequest(NetworkChannel channel, Guid dbGuid, long firstLogNum, long lastLogNum, ContinuousLogCopyRequest.Flags flags) : base(channel, NetworkChannelMessage.MessageType.ContinuousLogCopyRequest, dbGuid)
		{
			this.m_firstGeneration = firstLogNum;
			this.m_lastGeneration = lastLogNum;
			this.FlagsUsed = flags;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_firstGeneration);
			base.Packet.Append(this.m_lastGeneration);
			base.Packet.Append((ulong)this.FlagsUsed);
		}

		internal ContinuousLogCopyRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.ContinuousLogCopyRequest, packetContent)
		{
			this.m_firstGeneration = base.Packet.ExtractInt64();
			this.m_lastGeneration = base.Packet.ExtractInt64();
			this.FlagsUsed = (ContinuousLogCopyRequest.Flags)base.Packet.ExtractInt64();
		}

		public override void Execute()
		{
			LogCopyServerContext.StartContinuousLogTransmission(base.Channel, this);
		}

		private long m_firstGeneration;

		private long m_lastGeneration;

		public ContinuousLogCopyRequest.Flags FlagsUsed;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL,
			ForAcll = 2UL
		}
	}
}
