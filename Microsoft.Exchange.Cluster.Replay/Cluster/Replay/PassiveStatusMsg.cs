using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PassiveStatusMsg : NetworkChannelMessage
	{
		public static byte[] SerializeToBytes(PassiveStatusMsg.Flags msgFlags, long ackCounter, uint genInNetworkBuffer, uint genWrittenToInspector, uint lastGenInspected, uint lastGenReplayed, bool isCrossSite)
		{
			NetworkChannelPacket networkChannelPacket = new NetworkChannelPacket(54);
			networkChannelPacket.GrowthDisabled = true;
			networkChannelPacket.Append(1);
			int val = 49;
			networkChannelPacket.Append(val);
			val = 1096045392;
			networkChannelPacket.Append(val);
			val = 49;
			networkChannelPacket.Append(val);
			DateTime utcNow = DateTime.UtcNow;
			networkChannelPacket.Append(utcNow);
			networkChannelPacket.Append((long)msgFlags);
			networkChannelPacket.Append(ackCounter);
			networkChannelPacket.Append(genInNetworkBuffer);
			networkChannelPacket.Append(genWrittenToInspector);
			networkChannelPacket.Append(lastGenInspected);
			networkChannelPacket.Append(lastGenReplayed);
			networkChannelPacket.Append(isCrossSite);
			return networkChannelPacket.Buffer;
		}

		protected override void Serialize()
		{
		}

		internal override void SendInternal()
		{
		}

		internal PassiveStatusMsg(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.PassiveStatus, packetContent)
		{
			this.FlagsUsed = (PassiveStatusMsg.Flags)base.Packet.ExtractInt64();
			this.AckCounter = base.Packet.ExtractInt64();
			this.GenInNetworkBuffer = base.Packet.ExtractUInt32();
			this.GenWrittenToInspector = base.Packet.ExtractUInt32();
			this.LastGenInspected = base.Packet.ExtractUInt32();
			this.LastGenReplayed = base.Packet.ExtractUInt32();
			this.IsCrossSite = base.Packet.ExtractBool();
		}

		public const int TotalMsgSize = 54;

		public PassiveStatusMsg.Flags FlagsUsed;

		public long AckCounter;

		public uint GenInNetworkBuffer;

		public uint GenWrittenToInspector;

		public uint LastGenInspected;

		public uint LastGenReplayed;

		public bool IsCrossSite;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL,
			PassiveIsRequestingAck = 1UL,
			AckEndOfGeneration = 2UL
		}
	}
}
