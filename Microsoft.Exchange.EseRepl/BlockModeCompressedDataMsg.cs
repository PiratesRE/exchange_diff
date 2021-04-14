using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.EseRepl
{
	internal class BlockModeCompressedDataMsg : NetworkChannelMessage
	{
		internal static int CalculateBlockCount(int cblogdata)
		{
			int num = cblogdata / 65536;
			if (cblogdata % 65536 > 0)
			{
				num++;
			}
			return num;
		}

		internal static int GetOffsetToCompressedData(int uncompressedSize)
		{
			return 73 + 4 * BlockModeCompressedDataMsg.CalculateBlockCount(uncompressedSize);
		}

		public static int CalculateWorstLength(JET_EMITDATACTX emitContext, int cblogdata)
		{
			int num = BlockModeCompressedDataMsg.CalculateBlockCount(cblogdata);
			return 73 + 4 * num + cblogdata;
		}

		public static int SerializeToBuffer(JET_EMITDATACTX emitContext, byte[] logdata, int cblogdata, byte[] targetBuffer, int targetBufferOffsetToStart, out int totalCompressedSize)
		{
			NetworkChannelPacket networkChannelPacket = new NetworkChannelPacket(targetBuffer, targetBufferOffsetToStart);
			networkChannelPacket.GrowthDisabled = true;
			int num = BlockModeCompressedDataMsg.CalculateBlockCount(cblogdata);
			int[] array = new int[num];
			int num2 = 73 + 4 * num;
			int num3 = num2 + targetBufferOffsetToStart;
			int num4 = 0;
			totalCompressedSize = 0;
			int num5 = cblogdata;
			for (int i = 0; i < num; i++)
			{
				int num6 = Math.Min(num5, 65536);
				Xpress.Compress(logdata, num4, num6, targetBuffer, num3, num6, out array[i]);
				num4 += num6;
				num3 += array[i];
				totalCompressedSize += array[i];
				num5 -= num6;
			}
			networkChannelPacket.Append(1);
			int val = num2 - 5 + totalCompressedSize;
			networkChannelPacket.Append(val);
			val = 1145261378;
			networkChannelPacket.Append(val);
			val = num2 - 5;
			networkChannelPacket.Append(val);
			DateTime utcNow = DateTime.UtcNow;
			networkChannelPacket.Append(utcNow);
			long val2 = 0L;
			networkChannelPacket.Append(val2);
			val2 = Win32StopWatch.GetSystemPerformanceCounter();
			networkChannelPacket.Append(val2);
			networkChannelPacket.Append(cblogdata);
			val = emitContext.dwVersion;
			networkChannelPacket.Append(val);
			val2 = (long)emitContext.qwSequenceNum;
			networkChannelPacket.Append(val2);
			val = (int)emitContext.grbitOperationalFlags;
			networkChannelPacket.Append(val);
			DateTime time = DateTime.SpecifyKind(emitContext.logtimeEmit, DateTimeKind.Utc);
			networkChannelPacket.Append(time);
			val = emitContext.lgposLogData.lGeneration;
			networkChannelPacket.Append(val);
			ushort val3 = (ushort)emitContext.lgposLogData.isec;
			networkChannelPacket.Append(val3);
			val3 = (ushort)emitContext.lgposLogData.ib;
			networkChannelPacket.Append(val3);
			for (int j = 0; j < num; j++)
			{
				networkChannelPacket.Append(array[j]);
			}
			return num2 + totalCompressedSize;
		}

		public int LogDataLength { get; private set; }

		public int[] CompressedLengths { get; private set; }

		public JET_EMITDATACTX EmitContext { get; private set; }

		protected override void Serialize()
		{
		}

		internal override void SendInternal()
		{
		}

		internal BlockModeCompressedDataMsg(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.BlockModeCompressedData, packetContent)
		{
			this.FlagsUsed = (BlockModeCompressedDataMsg.Flags)base.Packet.ExtractInt64();
			this.RequestAckCounter = base.Packet.ExtractInt64();
			this.LogDataLength = base.Packet.ExtractInt32();
			if (this.LogDataLength > 1048576)
			{
				throw new NetworkCorruptDataException(channel.PartnerNodeName);
			}
			this.EmitContext = new JET_EMITDATACTX();
			this.EmitContext.cbLogData = (long)this.LogDataLength;
			this.EmitContext.dwVersion = base.Packet.ExtractInt32();
			this.EmitContext.qwSequenceNum = base.Packet.ExtractUInt64();
			this.EmitContext.grbitOperationalFlags = (ShadowLogEmitGrbit)base.Packet.ExtractUInt32();
			this.EmitContext.logtimeEmit = base.Packet.ExtractDateTime();
			JET_LGPOS lgposLogData = default(JET_LGPOS);
			lgposLogData.lGeneration = base.Packet.ExtractInt32();
			lgposLogData.isec = (int)base.Packet.ExtractUInt16();
			lgposLogData.ib = (int)base.Packet.ExtractUInt16();
			this.EmitContext.lgposLogData = lgposLogData;
			if (this.LogDataLength > 0)
			{
				int num = BlockModeCompressedDataMsg.CalculateBlockCount(this.LogDataLength);
				this.CompressedLengths = new int[num];
				for (int i = 0; i < num; i++)
				{
					int num2 = base.Packet.ExtractInt32();
					if (num2 <= 0 || num2 > 65536)
					{
						throw new NetworkCorruptDataException(channel.PartnerNodeName);
					}
					this.CompressedLengths[i] = num2;
				}
			}
		}

		public BlockModeCompressedDataMsg() : base(NetworkChannelMessage.MessageType.BlockModeCompressedData)
		{
		}

		internal static BlockModeCompressedDataMsg ReadFromNet(NetworkChannel ch, byte[] workingBuf, int startOffset)
		{
			int len = 52;
			ch.Read(workingBuf, startOffset, len);
			BlockModeCompressedDataMsg blockModeCompressedDataMsg = new BlockModeCompressedDataMsg();
			BufDeserializer bufDeserializer = new BufDeserializer(workingBuf, startOffset);
			blockModeCompressedDataMsg.FlagsUsed = (BlockModeCompressedDataMsg.Flags)bufDeserializer.ExtractInt64();
			blockModeCompressedDataMsg.RequestAckCounter = bufDeserializer.ExtractInt64();
			blockModeCompressedDataMsg.LogDataLength = bufDeserializer.ExtractInt32();
			if (blockModeCompressedDataMsg.LogDataLength > 1048576)
			{
				throw new NetworkCorruptDataException(ch.PartnerNodeName);
			}
			blockModeCompressedDataMsg.EmitContext = new JET_EMITDATACTX();
			blockModeCompressedDataMsg.EmitContext.cbLogData = (long)blockModeCompressedDataMsg.LogDataLength;
			blockModeCompressedDataMsg.EmitContext.dwVersion = bufDeserializer.ExtractInt32();
			blockModeCompressedDataMsg.EmitContext.qwSequenceNum = bufDeserializer.ExtractUInt64();
			blockModeCompressedDataMsg.EmitContext.grbitOperationalFlags = (ShadowLogEmitGrbit)bufDeserializer.ExtractUInt32();
			blockModeCompressedDataMsg.EmitContext.logtimeEmit = bufDeserializer.ExtractDateTime();
			JET_LGPOS lgposLogData = default(JET_LGPOS);
			lgposLogData.lGeneration = bufDeserializer.ExtractInt32();
			lgposLogData.isec = (int)bufDeserializer.ExtractUInt16();
			lgposLogData.ib = (int)bufDeserializer.ExtractUInt16();
			blockModeCompressedDataMsg.EmitContext.lgposLogData = lgposLogData;
			if (blockModeCompressedDataMsg.LogDataLength > 0)
			{
				int num = BlockModeCompressedDataMsg.CalculateBlockCount(blockModeCompressedDataMsg.LogDataLength);
				blockModeCompressedDataMsg.CompressedLengths = new int[num];
				len = num * 4;
				ch.Read(workingBuf, startOffset, len);
				bufDeserializer.Reset(workingBuf, startOffset);
				for (int i = 0; i < num; i++)
				{
					int num2 = bufDeserializer.ExtractInt32();
					if (num2 <= 0 || num2 > 65536)
					{
						throw new NetworkCorruptDataException(ch.PartnerNodeName);
					}
					blockModeCompressedDataMsg.CompressedLengths[i] = num2;
				}
			}
			return blockModeCompressedDataMsg;
		}

		private const int OffsetToStartOfBlockLengths = 52;

		internal const int BlockingSize = 65536;

		internal const int MaxDataLength = 1048576;

		public BlockModeCompressedDataMsg.Flags FlagsUsed;

		public long RequestAckCounter;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL
		}
	}
}
