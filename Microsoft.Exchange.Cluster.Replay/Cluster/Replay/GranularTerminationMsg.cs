using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class GranularTerminationMsg : NetworkChannelMessage
	{
		public bool IsOverflow
		{
			get
			{
				return (this.FlagsUsed & GranularTerminationMsg.Flags.Overflow) != GranularTerminationMsg.Flags.None;
			}
		}

		internal GranularTerminationMsg(NetworkChannel channel, bool isOverflow, string explanation, long nextGenToSend, long eolGen, DateTime eolUtc) : base(channel, NetworkChannelMessage.MessageType.GranularTermination)
		{
			if (isOverflow)
			{
				this.FlagsUsed = GranularTerminationMsg.Flags.Overflow;
			}
			this.NextGenerationServerWillSend = nextGenToSend;
			this.EndOfLogGeneration = eolGen;
			this.EndOfLogUtc = eolUtc;
			this.TerminationErrorString = explanation;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append((ulong)this.FlagsUsed);
			base.Packet.Append(this.RequestAckCounter);
			base.Packet.Append(this.ReplyAckCounter);
			base.Packet.Append(this.NextGenerationServerWillSend);
			base.Packet.Append(this.EndOfLogGeneration);
			base.Packet.Append(this.EndOfLogUtc);
			base.Packet.Append(this.TerminationErrorString);
		}

		internal GranularTerminationMsg(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.GranularTermination, packetContent)
		{
			this.FlagsUsed = (GranularTerminationMsg.Flags)base.Packet.ExtractInt64();
			this.RequestAckCounter = base.Packet.ExtractInt64();
			this.ReplyAckCounter = base.Packet.ExtractInt64();
			this.NextGenerationServerWillSend = base.Packet.ExtractInt64();
			this.EndOfLogGeneration = base.Packet.ExtractInt64();
			this.EndOfLogUtc = base.Packet.ExtractDateTime();
			this.TerminationErrorString = base.Packet.ExtractString();
		}

		public GranularTerminationMsg.Flags FlagsUsed;

		public long RequestAckCounter;

		public long ReplyAckCounter;

		public long NextGenerationServerWillSend;

		public long EndOfLogGeneration;

		public DateTime EndOfLogUtc;

		public string TerminationErrorString;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL,
			Overflow = 1UL
		}
	}
}
