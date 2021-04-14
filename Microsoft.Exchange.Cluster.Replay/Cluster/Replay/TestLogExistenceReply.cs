using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TestLogExistenceReply : NetworkChannelFileTransferReply
	{
		internal bool LogExists
		{
			get
			{
				return this.m_exists;
			}
			set
			{
				this.m_exists = value;
			}
		}

		internal TestLogExistenceReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.TestLogExistenceReply)
		{
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_exists);
		}

		internal TestLogExistenceReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.TestLogExistenceReply, packetContent)
		{
			this.m_exists = base.Packet.ExtractBool();
		}

		private bool m_exists;
	}
}
