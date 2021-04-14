using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NetworkChannelCompressionRequest : NetworkChannelMessage, INetworkChannelRequest
	{
		internal NetworkChannelCompressionRequest(NetworkChannel channel, NetworkChannel.DataEncodingScheme requestedEncoding) : base(channel, NetworkChannelMessage.MessageType.CompressionRequest)
		{
			this.m_requestedEncoding = requestedEncoding;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append((uint)this.m_requestedEncoding);
		}

		protected internal NetworkChannelCompressionRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.CompressionRequest, packetContent)
		{
			this.m_requestedEncoding = (NetworkChannel.DataEncodingScheme)base.Packet.ExtractUInt32();
		}

		public void Execute()
		{
			NetworkChannel.DataEncodingScheme dataEncodingScheme = NetworkChannel.VerifyDataEncoding(this.m_requestedEncoding);
			base.Channel.SetEncoding(dataEncodingScheme);
			NetworkChannelCompressionReply networkChannelCompressionReply = new NetworkChannelCompressionReply(base.Channel, dataEncodingScheme);
			networkChannelCompressionReply.Send();
			base.Channel.KeepAlive = true;
		}

		private NetworkChannel.DataEncodingScheme m_requestedEncoding;
	}
}
