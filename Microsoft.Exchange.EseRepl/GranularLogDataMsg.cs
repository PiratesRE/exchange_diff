using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.EseRepl
{
	internal class GranularLogDataMsg : NetworkChannelMessage
	{
		public static int CalculateSerializedLength(JET_EMITDATACTX emitContext, int cblogdata)
		{
			return 73 + cblogdata;
		}

		public static void SerializeToBuffer(int expectedTotalSize, GranularLogDataMsg.Flags msgFlags, JET_EMITDATACTX emitContext, byte[] logdata, int cblogdata, byte[] targetBuffer, int targetBufferOffsetToStart)
		{
			NetworkChannelPacket networkChannelPacket = new NetworkChannelPacket(targetBuffer, targetBufferOffsetToStart);
			networkChannelPacket.GrowthDisabled = true;
			networkChannelPacket.Append(1);
			int val = expectedTotalSize - 5;
			networkChannelPacket.Append(val);
			val = 1312903751;
			networkChannelPacket.Append(val);
			val = expectedTotalSize - 5;
			networkChannelPacket.Append(val);
			DateTime utcNow = DateTime.UtcNow;
			networkChannelPacket.Append(utcNow);
			networkChannelPacket.Append((long)msgFlags);
			long val2 = Win32StopWatch.GetSystemPerformanceCounter();
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
			if (cblogdata > 0)
			{
				networkChannelPacket.Append(logdata, 0, cblogdata);
			}
		}

		public int LogDataLength { get; private set; }

		public JET_EMITDATACTX EmitContext { get; private set; }

		protected override void Serialize()
		{
		}

		internal override void SendInternal()
		{
		}

		public GranularLogDataMsg() : base(NetworkChannelMessage.MessageType.GranularLogData)
		{
		}

		internal static GranularLogDataMsg ReadFromNet(NetworkChannel ch, byte[] workingBuf, int startOffset)
		{
			int len = 52;
			ch.Read(workingBuf, startOffset, len);
			GranularLogDataMsg granularLogDataMsg = new GranularLogDataMsg();
			BufDeserializer bufDeserializer = new BufDeserializer(workingBuf, startOffset);
			granularLogDataMsg.FlagsUsed = (GranularLogDataMsg.Flags)bufDeserializer.ExtractInt64();
			granularLogDataMsg.RequestAckCounter = bufDeserializer.ExtractInt64();
			granularLogDataMsg.LogDataLength = bufDeserializer.ExtractInt32();
			if (granularLogDataMsg.LogDataLength > 1048576)
			{
				throw new NetworkCorruptDataException(ch.PartnerNodeName);
			}
			granularLogDataMsg.EmitContext = new JET_EMITDATACTX();
			granularLogDataMsg.EmitContext.cbLogData = (long)granularLogDataMsg.LogDataLength;
			granularLogDataMsg.EmitContext.dwVersion = bufDeserializer.ExtractInt32();
			granularLogDataMsg.EmitContext.qwSequenceNum = bufDeserializer.ExtractUInt64();
			granularLogDataMsg.EmitContext.grbitOperationalFlags = (ShadowLogEmitGrbit)bufDeserializer.ExtractUInt32();
			granularLogDataMsg.EmitContext.logtimeEmit = bufDeserializer.ExtractDateTime();
			JET_LGPOS lgposLogData = default(JET_LGPOS);
			lgposLogData.lGeneration = bufDeserializer.ExtractInt32();
			lgposLogData.isec = (int)bufDeserializer.ExtractUInt16();
			lgposLogData.ib = (int)bufDeserializer.ExtractUInt16();
			granularLogDataMsg.EmitContext.lgposLogData = lgposLogData;
			return granularLogDataMsg;
		}

		private const int OffsetToStartOfLogData = 52;

		public GranularLogDataMsg.Flags FlagsUsed;

		public long RequestAckCounter;

		[Flags]
		public enum Flags : ulong
		{
			None = 0UL,
			AckRequested = 1UL,
			GranularStatus = 2UL,
			DebugChecksumPresent = 4UL,
			CompletionsDisabled = 8UL
		}
	}
}
