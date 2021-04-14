using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TestNetwork0Request : NetworkChannelMessage, INetworkChannelRequest
	{
		public TestNetworkParms Parms { get; private set; }

		internal TestNetwork0Request(NetworkChannel channel, TestNetworkParms parms) : base(channel, NetworkChannelMessage.MessageType.TestNetwork0)
		{
			this.Parms = parms;
			this.m_serializedParms = parms.ToBytes();
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.AppendByteArray(this.m_serializedParms);
		}

		internal TestNetwork0Request(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.TestNetwork0, packetContent)
		{
			this.m_serializedParms = base.Packet.ExtractByteArray();
			this.Parms = TestNetworkParms.FromBytes(this.m_serializedParms);
		}

		public void Execute()
		{
			if (this.Parms.TimeoutInSec != 0)
			{
				base.Channel.TcpChannel.WriteTimeoutInMs = 1000 * this.Parms.TimeoutInSec;
			}
			if (this.Parms.TransferSize <= 0)
			{
				this.Parms.TransferSize = 65536;
			}
			if (this.Parms.TransferCount <= 0L)
			{
				this.Parms.TransferCount = 1L;
			}
			if (this.Parms.ReplyCount <= 0L)
			{
				this.Parms.ReplyCount = 1L;
			}
			if (this.Parms.TcpWindowSize != 0)
			{
				base.Channel.TcpChannel.BufferSize = this.Parms.TcpWindowSize;
			}
			for (long num = 0L; num < this.Parms.ReplyCount; num += 1L)
			{
				new SeedDatabaseFileReply(base.Channel)
				{
					FileSize = (long)this.Parms.TransferSize * this.Parms.TransferCount,
					LastWriteUtc = DateTime.UtcNow
				}.Send();
				byte[] buf = new byte[this.Parms.TransferSize];
				int num2 = 0;
				while ((long)num2 < this.Parms.TransferCount)
				{
					base.Channel.Write(buf, 0, this.Parms.TransferSize);
					num2++;
				}
			}
		}

		private byte[] m_serializedParms;
	}
}
