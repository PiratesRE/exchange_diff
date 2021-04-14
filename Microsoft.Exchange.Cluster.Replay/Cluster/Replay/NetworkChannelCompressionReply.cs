using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NetworkChannelCompressionReply : NetworkChannelMessage
	{
		internal NetworkChannel.DataEncodingScheme AcceptedEncoding
		{
			get
			{
				return this.m_acceptedEncoding;
			}
		}

		internal NetworkChannelCompressionReply(NetworkChannel channel, NetworkChannel.DataEncodingScheme acceptedEncoding) : base(channel, NetworkChannelMessage.MessageType.CompressionReply)
		{
			this.m_acceptedEncoding = acceptedEncoding;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append((uint)this.m_acceptedEncoding);
		}

		internal NetworkChannelCompressionReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.CompressionReply, packetContent)
		{
			this.m_acceptedEncoding = (NetworkChannel.DataEncodingScheme)base.Packet.ExtractUInt32();
		}

		private NetworkChannel.DataEncodingScheme m_acceptedEncoding;
	}
}
