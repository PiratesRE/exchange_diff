using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ContinuousLogCopyRequest2 : NetworkChannelDatabaseRequest
	{
		public bool UseGranular
		{
			get
			{
				return (this.FlagsUsed & ContinuousLogCopyRequest2.Flags.UseGranular) != ContinuousLogCopyRequest2.Flags.None;
			}
		}

		public bool ForAcll
		{
			get
			{
				return (this.FlagsUsed & ContinuousLogCopyRequest2.Flags.ForAcll) != ContinuousLogCopyRequest2.Flags.None;
			}
		}

		internal ContinuousLogCopyRequest2(string clientNodeName, NetworkChannel channel, Guid dbGuid, long firstLogNum, ContinuousLogCopyRequest2.Flags flags) : base(channel, NetworkChannelMessage.MessageType.ContinuousLogCopyRequest2, dbGuid)
		{
			this.FlagsUsed = flags;
			this.FirstGeneration = firstLogNum;
			this.ClientNodeName = clientNodeName;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append((ulong)this.FlagsUsed);
			base.Packet.Append(this.RequestAckCounter);
			base.Packet.Append(this.ReplyAckCounter);
			base.Packet.Append(this.FirstGeneration);
			base.Packet.Append(this.LastGeneration);
			base.Packet.Append(this.ClientNodeName);
		}

		internal ContinuousLogCopyRequest2(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.ContinuousLogCopyRequest2, packetContent)
		{
			this.FlagsUsed = (ContinuousLogCopyRequest2.Flags)base.Packet.ExtractInt64();
			this.RequestAckCounter = base.Packet.ExtractInt64();
			this.ReplyAckCounter = base.Packet.ExtractInt64();
			this.FirstGeneration = base.Packet.ExtractInt64();
			this.LastGeneration = base.Packet.ExtractInt64();
			this.ClientNodeName = base.Packet.ExtractString();
		}

		public override void Execute()
		{
			LogCopyServerContext.StartContinuousLogTransmission(base.Channel, this);
		}

		public ContinuousLogCopyRequest2.Flags FlagsUsed;

		public long RequestAckCounter;

		public long ReplyAckCounter;

		public long FirstGeneration;

		public long LastGeneration;

		public string ClientNodeName;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL,
			UseGranular = 1UL,
			ForAcll = 2UL,
			Pause = 4UL,
			Resume = 8UL,
			Stop = 16UL
		}
	}
}
