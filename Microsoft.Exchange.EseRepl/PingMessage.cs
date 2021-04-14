using System;

namespace Microsoft.Exchange.EseRepl
{
	internal class PingMessage : NetworkChannelMessage
	{
		internal PingMessage(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.Ping)
		{
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append((ulong)this.FlagsUsed);
			base.Packet.Append(this.RequestAckCounter);
			base.Packet.Append(this.ReplyAckCounter);
		}

		internal PingMessage(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.Ping, packetContent)
		{
			this.FlagsUsed = (PingMessage.Flags)base.Packet.ExtractInt64();
			this.RequestAckCounter = base.Packet.ExtractInt64();
			this.ReplyAckCounter = base.Packet.ExtractInt64();
		}

		public PingMessage() : base(NetworkChannelMessage.MessageType.Ping)
		{
		}

		internal static PingMessage ReadFromNet(NetworkChannel ch, byte[] workingBuf, int startOffset)
		{
			int len = 24;
			ch.Read(workingBuf, startOffset, len);
			PingMessage pingMessage = new PingMessage();
			BufDeserializer bufDeserializer = new BufDeserializer(workingBuf, startOffset);
			pingMessage.FlagsUsed = (PingMessage.Flags)bufDeserializer.ExtractInt64();
			pingMessage.RequestAckCounter = bufDeserializer.ExtractInt64();
			pingMessage.ReplyAckCounter = bufDeserializer.ExtractInt64();
			return pingMessage;
		}

		public const int SizeRequired = 40;

		public PingMessage.Flags FlagsUsed;

		public long RequestAckCounter;

		public long ReplyAckCounter;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL,
			Request = 1UL,
			Reply = 2UL
		}
	}
}
