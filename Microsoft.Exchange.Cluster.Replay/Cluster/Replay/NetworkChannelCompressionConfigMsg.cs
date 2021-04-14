using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NetworkChannelCompressionConfigMsg : NetworkChannelMessage, INetworkChannelRequest
	{
		public NetworkChannelCompressionConfigMsg.MessagePurpose Purpose { get; set; }

		public string ConfigXml { get; set; }

		internal NetworkChannelCompressionConfigMsg(NetworkChannel channel, NetworkChannelCompressionConfigMsg.MessagePurpose purpose, string configXml) : base(channel, NetworkChannelMessage.MessageType.CompressionConfig)
		{
			this.Purpose = purpose;
			this.ConfigXml = configXml;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append((uint)this.Purpose);
			base.Packet.Append(this.ConfigXml);
		}

		protected internal NetworkChannelCompressionConfigMsg(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.CompressionConfig, packetContent)
		{
			this.Purpose = (NetworkChannelCompressionConfigMsg.MessagePurpose)base.Packet.ExtractUInt32();
			this.ConfigXml = base.Packet.ExtractString();
		}

		public void Execute()
		{
			Exception ex;
			CompressionConfig encoding = CompressionConfig.Deserialize(this.ConfigXml, out ex);
			string configXml;
			if (ex != null)
			{
				ReplayCrimsonEvents.InvalidCompressionConfigReceived.LogPeriodic<string, string, Exception>(base.Channel.PartnerNodeName, DiagCore.DefaultEventSuppressionInterval, base.Channel.PartnerNodeName, this.ConfigXml, ex);
				CompressionConfig obj = new CompressionConfig();
				configXml = SerializationUtil.ObjectToXml(obj);
			}
			else
			{
				configXml = this.ConfigXml;
			}
			NetworkChannelCompressionConfigMsg networkChannelCompressionConfigMsg = new NetworkChannelCompressionConfigMsg(base.Channel, NetworkChannelCompressionConfigMsg.MessagePurpose.DeclareEncoding, configXml);
			networkChannelCompressionConfigMsg.Send();
			base.Channel.SetEncoding(encoding);
			base.Channel.KeepAlive = true;
		}

		public enum MessagePurpose
		{
			RequestEncoding = 1,
			DeclareEncoding
		}
	}
}
