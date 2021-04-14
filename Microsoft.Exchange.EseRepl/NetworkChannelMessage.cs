using System;
using Microsoft.Exchange.Cluster.Common.Extensions;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.EseRepl
{
	internal abstract class NetworkChannelMessage
	{
		protected NetworkChannel Channel
		{
			get
			{
				return this.m_channel;
			}
		}

		internal NetworkChannelMessage.MessageType Type
		{
			get
			{
				return this.m_msgType;
			}
		}

		protected NetworkChannelPacket Packet
		{
			get
			{
				if (this.m_packet == null)
				{
					this.m_packet = new NetworkChannelPacket();
				}
				return this.m_packet;
			}
		}

		internal DateTime MessageUtc
		{
			get
			{
				return this.m_messageUtc;
			}
			set
			{
				this.m_messageUtc = value;
			}
		}

		public override string ToString()
		{
			return string.Format("NetworkChannelMessage({0})", this.Type);
		}

		internal static bool IsValidType(NetworkChannelMessage.MessageType msgType)
		{
			if (msgType <= NetworkChannelMessage.MessageType.CompressionRequest)
			{
				if (msgType <= NetworkChannelMessage.MessageType.CancelCiFileRequest)
				{
					if (msgType <= NetworkChannelMessage.MessageType.Ping)
					{
						if (msgType <= NetworkChannelMessage.MessageType.PassiveStatus)
						{
							if (msgType != NetworkChannelMessage.MessageType.TestNetwork0 && msgType != NetworkChannelMessage.MessageType.PassiveStatus)
							{
								return false;
							}
						}
						else if (msgType != NetworkChannelMessage.MessageType.CompressionConfig && msgType != NetworkChannelMessage.MessageType.BlockModeCompressedData && msgType != NetworkChannelMessage.MessageType.Ping)
						{
							return false;
						}
					}
					else if (msgType <= NetworkChannelMessage.MessageType.GranularLogData)
					{
						if (msgType != NetworkChannelMessage.MessageType.GranularTermination && msgType != NetworkChannelMessage.MessageType.GranularLogData)
						{
							return false;
						}
					}
					else if (msgType != NetworkChannelMessage.MessageType.EnterBlockMode && msgType != NetworkChannelMessage.MessageType.GetE00GenerationRequest && msgType != NetworkChannelMessage.MessageType.CancelCiFileRequest)
					{
						return false;
					}
				}
				else if (msgType <= NetworkChannelMessage.MessageType.TestLogExistenceRequest)
				{
					if (msgType <= NetworkChannelMessage.MessageType.PassiveDatabaseFileRequest)
					{
						if (msgType != NetworkChannelMessage.MessageType.SeedLogCopyRequest && msgType != NetworkChannelMessage.MessageType.PassiveDatabaseFileRequest)
						{
							return false;
						}
					}
					else if (msgType != NetworkChannelMessage.MessageType.SeedDatabaseFileRequest)
					{
						switch (msgType)
						{
						case NetworkChannelMessage.MessageType.ContinuousLogCopyRequest:
						case NetworkChannelMessage.MessageType.NotifyEndOfLogRequest:
							break;
						case (NetworkChannelMessage.MessageType)1363627076:
							return false;
						default:
							if (msgType != NetworkChannelMessage.MessageType.TestLogExistenceRequest)
							{
								return false;
							}
							break;
						}
					}
				}
				else if (msgType <= NetworkChannelMessage.MessageType.TestHealthRequest)
				{
					if (msgType != NetworkChannelMessage.MessageType.CopyLogRequest && msgType != NetworkChannelMessage.MessageType.QueryLogRangeRequest && msgType != NetworkChannelMessage.MessageType.TestHealthRequest)
					{
						return false;
					}
				}
				else if (msgType != NetworkChannelMessage.MessageType.SeedPageReaderRollLogFileRequest && msgType != NetworkChannelMessage.MessageType.ProgressCiFileRequest && msgType != NetworkChannelMessage.MessageType.CompressionRequest)
				{
					return false;
				}
			}
			else if (msgType <= NetworkChannelMessage.MessageType.SeedDatabaseFileReply)
			{
				if (msgType <= NetworkChannelMessage.MessageType.ContinuousLogCopyRequest2)
				{
					if (msgType <= NetworkChannelMessage.MessageType.SeedCiFileRequest)
					{
						if (msgType != NetworkChannelMessage.MessageType.SeedPageReaderSinglePageRequest && msgType != NetworkChannelMessage.MessageType.SeedCiFileRequest)
						{
							return false;
						}
					}
					else if (msgType != NetworkChannelMessage.MessageType.SeedCiFileRequest2 && msgType != NetworkChannelMessage.MessageType.SeedPageReaderPageSizeRequest && msgType != NetworkChannelMessage.MessageType.ContinuousLogCopyRequest2)
					{
						return false;
					}
				}
				else if (msgType <= NetworkChannelMessage.MessageType.GetE00GenerationReply)
				{
					if (msgType != NetworkChannelMessage.MessageType.LogCopyServerStatus && msgType != NetworkChannelMessage.MessageType.GranularLogComplete && msgType != NetworkChannelMessage.MessageType.GetE00GenerationReply)
					{
						return false;
					}
				}
				else if (msgType != NetworkChannelMessage.MessageType.NotifyEndOfLogAsyncReply && msgType != NetworkChannelMessage.MessageType.CancelCiFileReply && msgType != NetworkChannelMessage.MessageType.SeedDatabaseFileReply)
				{
					return false;
				}
			}
			else if (msgType <= NetworkChannelMessage.MessageType.TestHealthReply)
			{
				if (msgType <= NetworkChannelMessage.MessageType.TestLogExistenceReply)
				{
					if (msgType != NetworkChannelMessage.MessageType.NotifyEndOfLogReply && msgType != NetworkChannelMessage.MessageType.TestLogExistenceReply)
					{
						return false;
					}
				}
				else if (msgType != NetworkChannelMessage.MessageType.CopyLogReply && msgType != NetworkChannelMessage.MessageType.QueryLogRangeReply && msgType != NetworkChannelMessage.MessageType.TestHealthReply)
				{
					return false;
				}
			}
			else if (msgType <= NetworkChannelMessage.MessageType.CompressionReply)
			{
				if (msgType != NetworkChannelMessage.MessageType.SeedPageReaderRollLogFileReply && msgType != NetworkChannelMessage.MessageType.ProgressCiFileReply && msgType != NetworkChannelMessage.MessageType.CompressionReply)
				{
					return false;
				}
			}
			else if (msgType != NetworkChannelMessage.MessageType.SeedPageReaderSinglePageReply && msgType != NetworkChannelMessage.MessageType.SeedCiFileReply && msgType != NetworkChannelMessage.MessageType.SeedPageReaderPageSizeReply)
			{
				return false;
			}
			return true;
		}

		public static NetworkChannelMessageHeader ReadHeaderFromNet(NetworkChannel netChan, byte[] workingBuf, int startOffset)
		{
			netChan.Read(workingBuf, startOffset, 16);
			BufDeserializer bufDeserializer = new BufDeserializer(workingBuf, 0);
			NetworkChannelMessageHeader result;
			result.MessageType = (NetworkChannelMessage.MessageType)bufDeserializer.ExtractUInt32();
			result.MessageLength = bufDeserializer.ExtractInt32();
			result.MessageUtc = bufDeserializer.ExtractDateTime();
			return result;
		}

		internal static NetworkChannelMessage ReadMessage(NetworkChannel channel, byte[] headerBuf)
		{
			int num = 0;
			NetworkChannelMessage.MessageType messageType = (NetworkChannelMessage.MessageType)Serialization.DeserializeUInt32(headerBuf, ref num);
			if (!NetworkChannelMessage.IsValidType(messageType))
			{
				throw new NetworkUnexpectedMessageException(channel.PartnerNodeName, string.Format("Unknown Type{0}", messageType));
			}
			int num2 = (int)Serialization.DeserializeUInt32(headerBuf, ref num);
			if (num2 < 16 || num2 > 1052672)
			{
				throw new NetworkUnexpectedMessageException(channel.PartnerNodeName, string.Format("Invalid msgLen: {0}", num2));
			}
			ExTraceGlobals.NetworkChannelTracer.TraceDebug<NetworkChannelMessage.MessageType, string, string>((long)channel.GetHashCode(), "ReadMessage: Received {0} from {1} on {2}", messageType, channel.RemoteEndPointString, channel.LocalEndPointString);
			byte[] array = new byte[num2];
			Array.Copy(headerBuf, 0, array, 0, 16);
			int len = num2 - 16;
			channel.Read(array, 16, len);
			NetworkChannelMessage.MessageType messageType2 = messageType;
			if (messageType2 <= NetworkChannelMessage.MessageType.BlockModeCompressedData)
			{
				if (messageType2 == NetworkChannelMessage.MessageType.PassiveStatus)
				{
					return new PassiveStatusMsg(channel, array);
				}
				if (messageType2 != NetworkChannelMessage.MessageType.BlockModeCompressedData)
				{
					goto IL_118;
				}
			}
			else
			{
				if (messageType2 == NetworkChannelMessage.MessageType.Ping)
				{
					return new PingMessage(channel, array);
				}
				if (messageType2 != NetworkChannelMessage.MessageType.GranularLogData)
				{
					if (messageType2 != NetworkChannelMessage.MessageType.EnterBlockMode)
					{
						goto IL_118;
					}
					return new EnterBlockModeMsg(channel, array);
				}
			}
			throw new NetworkUnexpectedMessageException(channel.PartnerNodeName, string.Format("ReadMessage() does not support message type: {0}.", messageType));
			IL_118:
			throw new NetworkUnexpectedMessageException(channel.PartnerNodeName, string.Format("Unknown message type: 0x{0:X}", (int)messageType));
		}

		internal virtual void Send()
		{
			this.MessageUtc = DateTime.UtcNow;
			this.Packet.PrepareToWrite();
			this.Serialize();
			int num = 4;
			Serialization.SerializeUInt32(this.Packet.Buffer, ref num, (uint)this.Packet.Length);
			ExTraceGlobals.NetworkChannelTracer.TraceDebug<NetworkChannelMessage.MessageType, string, string>((long)this.m_channel.GetHashCode(), "SendingMessage: {0} to {1} on {2}", this.Type, this.m_channel.RemoteEndPointString, this.m_channel.LocalEndPointString);
			this.SendInternal();
		}

		internal virtual void SendInternal()
		{
			this.m_channel.SendMessage(this.m_packet.Buffer, 0, this.m_packet.Length);
		}

		protected virtual void Serialize()
		{
			this.Packet.Append((uint)this.m_msgType);
			this.Packet.Append(16U);
			this.Packet.Append(this.m_messageUtc);
		}

		internal NetworkChannelMessage(NetworkChannel channel, NetworkChannelMessage.MessageType msgType)
		{
			this.m_channel = channel;
			this.m_msgType = msgType;
		}

		internal NetworkChannelMessage(NetworkChannel channel, NetworkChannelMessage.MessageType msgType, byte[] packetContent)
		{
			this.m_channel = channel;
			this.m_msgType = msgType;
			this.m_packet = new NetworkChannelPacket(packetContent);
			this.Packet.ExtractUInt32();
			this.Packet.ExtractUInt32();
			this.MessageUtc = this.Packet.ExtractDateTime();
		}

		internal NetworkChannelMessage(NetworkChannelMessage.MessageType msgType)
		{
			this.m_msgType = msgType;
		}

		public static void SerializeHeaderToBuffer(NetworkChannelMessage.MessageType msgType, int totalSize, byte[] targetBuffer, int targetBufferOffsetToStart)
		{
			NetworkChannelPacket networkChannelPacket = new NetworkChannelPacket(targetBuffer, targetBufferOffsetToStart);
			networkChannelPacket.GrowthDisabled = true;
			networkChannelPacket.Append(1);
			int val = totalSize - 5;
			networkChannelPacket.Append(val);
			networkChannelPacket.Append((int)msgType);
			networkChannelPacket.Append(val);
			networkChannelPacket.Append(DateTime.UtcNow);
		}

		public const int GuidSize = 16;

		internal const int MessageHeaderSize = 16;

		private const int OffsetOfMessageLengthField = 4;

		internal const int MaxMessageSize = 1052672;

		protected NetworkChannel m_channel;

		protected NetworkChannelMessage.MessageType m_msgType;

		protected NetworkChannelPacket m_packet;

		protected DateTime m_messageUtc;

		internal enum MessageType
		{
			Invalid,
			CopyLogRequest = 1363627852,
			CopyLogReply = 1497845580,
			NotifyEndOfLogRequest = 1363627077,
			NotifyEndOfLogReply = 1497844805,
			NotifyEndOfLogAsyncReply = 1497451589,
			QueryLogRangeRequest = 1363628620,
			QueryLogRangeReply = 1497846348,
			GetE00GenerationRequest = 1362112581,
			GetE00GenerationReply = 1496330309,
			TestLogExistenceRequest = 1363627092,
			TestLogExistenceReply = 1497844820,
			TestHealthRequest = 1363694164,
			TestHealthReply = 1497911892,
			CancelCiFileRequest = 1363364163,
			CancelCiFileReply = 1497581891,
			ProgressCiFileRequest = 1364216131,
			ProgressCiFileReply = 1498433859,
			SeedCiFileRequest = 1364412739,
			SeedCiFileRequest2 = 1364478275,
			SeedCiFileReply = 1498630467,
			CompressionRequest = 1364217155,
			CompressionReply = 1498434883,
			CompressionConfig = 1129336131,
			SeedDatabaseFileRequest = 1363559507,
			SeedDatabaseFileReply = 1497777235,
			PassiveDatabaseFileRequest = 1363559504,
			SeedLogCopyRequest = 1363364947,
			SeedPageReaderSinglePageRequest = 1364349011,
			SeedPageReaderSinglePageReply = 1498566739,
			SeedPageReaderPageSizeRequest = 1364870227,
			SeedPageReaderPageSizeReply = 1499087955,
			SeedPageReaderRollLogFileRequest = 1363956307,
			SeedPageReaderRollLogFileReply = 1498174035,
			ContinuousLogCopyRequest = 1363627075,
			ContinuousLogCopyRequest2 = 1380139852,
			LogCopyServerStatus = 1397965644,
			GranularLogData = 1312903751,
			GranularLogComplete = 1481527879,
			GranularTermination = 1297371719,
			BlockModeCompressedData = 1145261378,
			Ping = 1196312912,
			TestNetwork0 = 810832724,
			EnterBlockMode = 1313164610,
			PassiveStatus = 1096045392
		}
	}
}
