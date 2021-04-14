using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class NetworkChannelFileTransferReply : NetworkChannelMessage
	{
		internal string DestinationFileName
		{
			get
			{
				return this.m_destFileName;
			}
			set
			{
				this.m_destFileName = value;
			}
		}

		internal long FileSize
		{
			get
			{
				return this.m_fileSize;
			}
			set
			{
				this.m_fileSize = value;
			}
		}

		internal DateTime LastWriteUtc
		{
			get
			{
				return this.m_lastWriteUtc;
			}
			set
			{
				this.m_lastWriteUtc = value;
			}
		}

		internal NetworkChannel.DataEncodingScheme DataEncoding
		{
			get
			{
				return this.m_dataEncoding;
			}
			set
			{
				this.m_dataEncoding = value;
			}
		}

		internal NetworkChannelFileTransferReply(NetworkChannel channel, NetworkChannelMessage.MessageType msgType) : base(channel, msgType)
		{
			this.m_dataEncoding = NetworkChannel.DataEncodingScheme.Uncompressed;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_fileSize);
			base.Packet.Append(this.m_lastWriteUtc);
			base.Packet.Append((uint)this.DataEncoding);
		}

		internal NetworkChannelFileTransferReply(NetworkChannel channel, NetworkChannelMessage.MessageType msgType, byte[] packetContent) : base(channel, msgType, packetContent)
		{
			this.FileSize = base.Packet.ExtractInt64();
			this.LastWriteUtc = base.Packet.ExtractDateTime();
			this.DataEncoding = (NetworkChannel.DataEncodingScheme)base.Packet.ExtractUInt32();
		}

		internal void ReceiveFile(string fullDestinationFileName, IReplicaSeederCallback callback, IPerfmonCounters copyPerfCtrs, CheckSummer summer)
		{
			this.DestinationFileName = fullDestinationFileName;
			this.m_channel.ReceiveFile(this, callback, copyPerfCtrs, summer);
		}

		internal void ReceiveFile(string fullDestinationFileName, IPerfmonCounters copyPerfCtrs, CheckSummer summer)
		{
			this.ReceiveFile(fullDestinationFileName, null, copyPerfCtrs, summer);
		}

		private long m_fileSize;

		private DateTime m_lastWriteUtc;

		private NetworkChannel.DataEncodingScheme m_dataEncoding;

		private string m_destFileName;
	}
}
