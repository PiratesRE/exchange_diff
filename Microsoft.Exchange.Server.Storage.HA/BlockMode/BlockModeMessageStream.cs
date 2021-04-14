using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Exchange.Server.Storage.BlockMode
{
	internal class BlockModeMessageStream
	{
		public BlockModeMessageStream(string dbName, int maxgenToKeep, BlockModeMessageStream.FreeIOBuffers bigWriteBuffers)
		{
			this.freeBuffers = bigWriteBuffers;
			this.DatabaseName = dbName;
			this.firstMessage = new BlockModeMessageStream.BlockModeMessageBase();
			this.lastMessage = this.firstMessage;
			this.maxGenerationsToKeep = maxgenToKeep;
			this.oldestBufferOrdinalReferencedBySenders = ulong.MaxValue;
			this.appendBuffer = this.freeBuffers.Allocate(1048576);
			this.appendBuffer.LifetimeOrdinal = (this.currentBufferOrdinal += 1UL);
			this.oldestBuffer = this.appendBuffer;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.BlockModeMessageStreamTracer;
			}
		}

		public bool FoundFirstLogBoundary { get; private set; }

		public string DatabaseName { get; private set; }

		public ulong OldestBufferLifetimeReferencedBySenders
		{
			get
			{
				return this.oldestBufferOrdinalReferencedBySenders;
			}
			set
			{
				if (this.oldestBufferOrdinalReferencedBySenders != value)
				{
					this.oldestBufferOrdinalReferencedBySenders = value;
					this.FreeOldBuffers();
				}
			}
		}

		public ulong LatestBufferLifetimeOrdinal
		{
			get
			{
				return this.currentBufferOrdinal;
			}
		}

		public bool CompressLogData { get; set; }

		public int Append(JET_EMITDATACTX emitContext, byte[] logdata, out int compressedLengthOfLogData)
		{
			int cblogdata = checked((int)emitContext.cbLogData);
			compressedLengthOfLogData = 0;
			BlockModeMessageStream.BlockModeDataMessage blockModeDataMessage = new BlockModeMessageStream.BlockModeDataMessage(emitContext);
			if (!this.FoundFirstLogBoundary)
			{
				if (!this.endOfALogFound)
				{
					if (blockModeDataMessage.IsLastForLog)
					{
						this.endOfALogFound = true;
					}
					return 0;
				}
				if (!blockModeDataMessage.HasDataBuffersFlag)
				{
					BlockModeMessageStream.Tracer.TraceDebug((long)this.GetHashCode(), "Ignoring msg since it doesn't have data");
					return 0;
				}
				this.FoundFirstLogBoundary = true;
				this.endOfALogFound = false;
				blockModeDataMessage.IsFirstForLog = true;
				BlockModeMessageStream.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Found first boundary at gen 0x{0:X}", blockModeDataMessage.LogGeneration);
			}
			else
			{
				if (this.endOfALogFound && blockModeDataMessage.HasDataBuffersFlag)
				{
					blockModeDataMessage.IsFirstForLog = true;
					this.endOfALogFound = false;
				}
				if (blockModeDataMessage.IsLastForLog)
				{
					this.endOfALogFound = true;
				}
			}
			bool compressLogData = this.CompressLogData;
			int num;
			if (compressLogData)
			{
				num = BlockModeCompressedDataMsg.CalculateWorstLength(emitContext, cblogdata);
			}
			else
			{
				num = GranularLogDataMsg.CalculateSerializedLength(emitContext, cblogdata);
			}
			BlockModeMessageStream.IOBuffer iobuffer = null;
			BlockModeMessageStream.IOBuffer iobuffer2 = this.appendBuffer;
			if (this.appendBuffer.RemainingSpace < num)
			{
				int bufSize = Math.Max(num, 1048576);
				using (LockManager.Lock(this.bufferManagementLock, LockManager.LockType.LeafMonitorLock))
				{
					iobuffer = this.freeBuffers.Allocate(bufSize);
					iobuffer.LifetimeOrdinal = (this.currentBufferOrdinal += 1UL);
				}
				BlockModeMessageStream.Tracer.TraceDebug<ulong, int>((long)this.GetHashCode(), "Buffers in use: {0} free: {1}", this.currentBufferOrdinal - this.oldestBuffer.LifetimeOrdinal + 1UL, this.freeBuffers.FreeBufferCount);
				iobuffer2 = iobuffer;
			}
			if (compressLogData)
			{
				num = BlockModeCompressedDataMsg.SerializeToBuffer(emitContext, logdata, cblogdata, iobuffer2.Buffer, iobuffer2.AppendOffset, out compressedLengthOfLogData);
			}
			else
			{
				GranularLogDataMsg.SerializeToBuffer(num, GranularLogDataMsg.Flags.None, emitContext, logdata, cblogdata, iobuffer2.Buffer, iobuffer2.AppendOffset);
			}
			blockModeDataMessage.IOBuffer = iobuffer2;
			blockModeDataMessage.MessageStartOffset = iobuffer2.AppendOffset;
			iobuffer2.AppendOffset += num;
			if (iobuffer != null)
			{
				this.appendBuffer.NextBuffer = iobuffer;
				this.appendBuffer = iobuffer;
			}
			this.lastMessage.NextMsg = blockModeDataMessage;
			this.lastMessage = blockModeDataMessage;
			if (blockModeDataMessage.IsLastForLog)
			{
				this.TrimOldGenerations(blockModeDataMessage.LogGeneration);
				this.FreeOldBuffers();
			}
			return num;
		}

		public BlockModeMessageStream.SenderPosition Join(uint firstGenToSend)
		{
			BlockModeMessageStream.SenderPosition result;
			using (LockManager.Lock(this.bufferManagementLock, LockManager.LockType.LeafMonitorLock))
			{
				BlockModeMessageStream.BlockModeMessageBase nextMsg = this.firstMessage;
				BlockModeMessageStream.BlockModeDataMessage blockModeDataMessage;
				for (;;)
				{
					blockModeDataMessage = (nextMsg as BlockModeMessageStream.BlockModeDataMessage);
					if (blockModeDataMessage != null && blockModeDataMessage.IsFirstForLog && (long)blockModeDataMessage.LogGeneration >= (long)((ulong)firstGenToSend))
					{
						break;
					}
					if (nextMsg.NextMsg == null)
					{
						goto Block_8;
					}
					nextMsg = nextMsg.NextMsg;
				}
				if ((long)blockModeDataMessage.LogGeneration == (long)((ulong)firstGenToSend))
				{
					if (this.oldestBufferOrdinalReferencedBySenders > blockModeDataMessage.IOBuffer.LifetimeOrdinal)
					{
						this.oldestBufferOrdinalReferencedBySenders = blockModeDataMessage.IOBuffer.LifetimeOrdinal;
					}
					return new BlockModeMessageStream.SenderPosition(blockModeDataMessage);
				}
				BlockModeMessageStream.Tracer.TraceError<string, int>((long)this.GetHashCode(), "JoinQ({0}) oldest Gen is 0x{1:X}", this.DatabaseName, blockModeDataMessage.LogGeneration);
				return null;
				Block_8:
				BlockModeMessageStream.Tracer.TraceError<string, uint>((long)this.GetHashCode(), "JoinQ({0}) couldn't find start point. Generations haven't arrived at 0x{1:X}", this.DatabaseName, firstGenToSend);
				result = null;
			}
			return result;
		}

		public void FreeOldBuffers()
		{
			bool flag = false;
			using (LockManager.Lock(this.bufferManagementLock, LockManager.LockType.LeafMonitorLock))
			{
				BlockModeMessageStream.BlockModeDataMessage blockModeDataMessage = this.firstMessage as BlockModeMessageStream.BlockModeDataMessage;
				if (blockModeDataMessage != null)
				{
					ulong num = Math.Min(this.oldestBufferOrdinalReferencedBySenders, blockModeDataMessage.IOBuffer.LifetimeOrdinal);
					Globals.AssertRetail(num >= this.oldestBuffer.LifetimeOrdinal, "oldest lifetime mismatch");
					while (num > this.oldestBuffer.LifetimeOrdinal)
					{
						Globals.AssertRetail(this.appendBuffer != this.oldestBuffer, "current lifetime mismatch");
						BlockModeMessageStream.IOBuffer iobuffer = this.oldestBuffer;
						this.oldestBuffer = iobuffer.NextBuffer;
						this.freeBuffers.Free(iobuffer);
						flag = true;
					}
					if (flag)
					{
						BlockModeMessageStream.Tracer.TraceDebug<ulong, int>((long)this.GetHashCode(), "After free: Buffers in use: {0} free: {1}", this.currentBufferOrdinal - this.oldestBuffer.LifetimeOrdinal + 1UL, this.freeBuffers.FreeBufferCount);
					}
				}
			}
		}

		private void TrimOldGenerations(int curGen)
		{
			if (curGen > this.maxGenerationsToKeep)
			{
				int num = curGen - this.maxGenerationsToKeep;
				BlockModeMessageStream.BlockModeMessageBase nextMsg = this.firstMessage;
				BlockModeMessageStream.BlockModeDataMessage blockModeDataMessage;
				for (;;)
				{
					blockModeDataMessage = (nextMsg as BlockModeMessageStream.BlockModeDataMessage);
					if (blockModeDataMessage != null && blockModeDataMessage.IsFirstForLog && blockModeDataMessage.LogGeneration >= num)
					{
						break;
					}
					if (nextMsg.NextMsg == null)
					{
						goto Block_6;
					}
					nextMsg = nextMsg.NextMsg;
				}
				if (blockModeDataMessage != this.firstMessage)
				{
					BlockModeMessageStream.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "TrimOld({0}) sets first to gen 0x{1:X}", this.DatabaseName, blockModeDataMessage.LogGeneration);
					this.firstMessage = blockModeDataMessage;
					return;
				}
				return;
				Block_6:
				BlockModeMessageStream.Tracer.TraceError<string>((long)this.GetHashCode(), "TrimOld({0}) didn't find stop point", this.DatabaseName);
				return;
			}
		}

		public const int IOBufferSize = 1048576;

		private BlockModeMessageStream.IOBuffer appendBuffer;

		private BlockModeMessageStream.IOBuffer oldestBuffer;

		private ulong oldestBufferOrdinalReferencedBySenders;

		private ulong currentBufferOrdinal;

		private object bufferManagementLock = new object();

		private BlockModeMessageStream.FreeIOBuffers freeBuffers;

		private BlockModeMessageStream.BlockModeMessageBase lastMessage;

		private BlockModeMessageStream.BlockModeMessageBase firstMessage;

		private bool endOfALogFound;

		private int maxGenerationsToKeep;

		internal class IOBuffer
		{
			public IOBuffer(int size, bool preAllocated)
			{
				this.AppendOffset = 0;
				this.NextBuffer = null;
				this.Buffer = new byte[size];
				this.PreAllocated = preAllocated;
			}

			public byte[] Buffer { get; private set; }

			public ulong LifetimeOrdinal { get; set; }

			public int AppendOffset
			{
				get
				{
					return this.appendOffset;
				}
				set
				{
					this.appendOffset = value;
				}
			}

			public BlockModeMessageStream.IOBuffer NextBuffer
			{
				get
				{
					return this.nextBuffer;
				}
				set
				{
					this.nextBuffer = value;
				}
			}

			public int RemainingSpace
			{
				get
				{
					return this.Buffer.Length - this.AppendOffset;
				}
			}

			public bool PreAllocated { get; private set; }

			private volatile int appendOffset;

			private volatile BlockModeMessageStream.IOBuffer nextBuffer;
		}

		internal class FreeIOBuffers
		{
			public FreeIOBuffers(int bufSize, int numberOfBuffersToPreAllocate)
			{
				this.bufferSize = bufSize;
				for (int i = 0; i < numberOfBuffersToPreAllocate; i++)
				{
					this.Free(new BlockModeMessageStream.IOBuffer(bufSize, true));
				}
			}

			public int FreeBufferCount
			{
				get
				{
					return this.freeBufferCount;
				}
			}

			public BlockModeMessageStream.IOBuffer Allocate(int bufSize)
			{
				if (bufSize != this.bufferSize)
				{
					return new BlockModeMessageStream.IOBuffer(bufSize, false);
				}
				BlockModeMessageStream.IOBuffer iobuffer = this.firstFreeBuf;
				if (iobuffer != null)
				{
					this.firstFreeBuf = iobuffer.NextBuffer;
					iobuffer.AppendOffset = 0;
					iobuffer.NextBuffer = null;
					this.freeBufferCount--;
					return iobuffer;
				}
				return new BlockModeMessageStream.IOBuffer(bufSize, false);
			}

			public void Free(BlockModeMessageStream.IOBuffer buf)
			{
				if (buf.Buffer.Length == this.bufferSize && (buf.PreAllocated || this.firstFreeBuf == null))
				{
					buf.NextBuffer = this.firstFreeBuf;
					if (buf.PreAllocated && buf.NextBuffer != null && !buf.NextBuffer.PreAllocated)
					{
						buf.NextBuffer = buf.NextBuffer.NextBuffer;
						this.freeBufferCount--;
					}
					this.firstFreeBuf = buf;
					this.freeBufferCount++;
				}
			}

			private readonly int bufferSize;

			private BlockModeMessageStream.IOBuffer firstFreeBuf;

			private int freeBufferCount;
		}

		internal class SenderPosition
		{
			public SenderPosition(BlockModeMessageStream.BlockModeDataMessage startMsg)
			{
				this.CurrentBuffer = startMsg.IOBuffer;
				this.NextSendOffset = startMsg.MessageStartOffset;
			}

			public BlockModeMessageStream.IOBuffer CurrentBuffer { get; set; }

			public int NextSendOffset { get; set; }
		}

		internal class BlockModeMessageBase
		{
			public BlockModeMessageStream.BlockModeMessageBase NextMsg { get; set; }
		}

		internal class BlockModeDataMessage : BlockModeMessageStream.BlockModeMessageBase
		{
			public BlockModeDataMessage(JET_EMITDATACTX emitlogdatactx)
			{
				this.EmitContext = emitlogdatactx;
			}

			public BlockModeMessageStream.IOBuffer IOBuffer { get; set; }

			public int MessageStartOffset { get; set; }

			public JET_EMITDATACTX EmitContext { get; private set; }

			public bool IsLastForLog
			{
				get
				{
					return BitMasker.IsOn((int)this.EmitContext.grbitOperationalFlags, 16);
				}
			}

			public bool IsFirstForLog { get; set; }

			public bool IsTerminationMessage
			{
				get
				{
					return BitMasker.IsOn((int)this.EmitContext.grbitOperationalFlags, 2);
				}
			}

			public bool HasDataBuffersFlag
			{
				get
				{
					return BitMasker.IsOn((int)this.EmitContext.grbitOperationalFlags, 8);
				}
			}

			public int LogGeneration
			{
				get
				{
					return this.EmitContext.lgposLogData.lGeneration;
				}
			}

			public int LogSector
			{
				get
				{
					return this.EmitContext.lgposLogData.isec;
				}
			}

			public override string ToString()
			{
				return string.Format("Gen=0x{0:X} Sector=0x{1:X} JBits=0x{2:X} EmitSeq=0x{3:X} LogDataLen=0x{4:X}", new object[]
				{
					this.LogGeneration,
					this.LogSector,
					(int)this.EmitContext.grbitOperationalFlags,
					(int)this.EmitContext.qwSequenceNum,
					(int)this.EmitContext.cbLogData
				});
			}
		}
	}
}
