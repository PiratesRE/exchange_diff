using System;
using Microsoft.Exchange.Cluster.Common;

namespace Microsoft.Exchange.EseRepl
{
	internal class EnterBlockModeMsg : NetworkChannelMessage, INetworkChannelRequest
	{
		internal EnterBlockModeMsg(NetworkChannel channel, EnterBlockModeMsg.Flags flags, Guid dbGuid, long firstGenerationToExpect) : base(channel, NetworkChannelMessage.MessageType.EnterBlockMode)
		{
			this.FlagsUsed = flags;
			this.AckCounter = Win32StopWatch.GetSystemPerformanceCounter();
			this.FirstGenerationToExpect = firstGenerationToExpect;
			this.DatabaseGuid = dbGuid;
			this.ActiveNodeName = Environment.MachineName;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append((ulong)this.FlagsUsed);
			base.Packet.Append(this.AckCounter);
			base.Packet.Append(this.FirstGenerationToExpect);
			base.Packet.Append(this.DatabaseGuid);
			base.Packet.Append(this.ActiveNodeName);
		}

		internal EnterBlockModeMsg(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.EnterBlockMode, packetContent)
		{
			this.FlagsUsed = (EnterBlockModeMsg.Flags)base.Packet.ExtractInt64();
			this.AckCounter = base.Packet.ExtractInt64();
			this.FirstGenerationToExpect = base.Packet.ExtractInt64();
			this.DatabaseGuid = base.Packet.ExtractGuid();
			this.ActiveNodeName = base.Packet.ExtractString();
		}

		public void Execute()
		{
		}

		public EnterBlockModeMsg.Flags FlagsUsed;

		public long AckCounter;

		public long FirstGenerationToExpect;

		public Guid DatabaseGuid;

		public string ActiveNodeName;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL,
			PrepareToEnter = 1UL,
			PassiveIsReady = 2UL,
			PassiveReject = 4UL
		}
	}
}
