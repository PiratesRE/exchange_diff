using System;
using System.IO;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.TextConverters.Internal.RtfCompressed
{
	internal class RtfCompressConverter : RtfCompressCommon, IProducerConsumer, IDisposable
	{
		public RtfCompressConverter(Stream input, bool push, Stream output, RtfCompressionMode mode, int inputBufferSize, int outputBufferSize) : base(input, push, output, inputBufferSize, outputBufferSize)
		{
			if (mode == RtfCompressionMode.Compressed)
			{
				this.uncompressed = false;
				this.window = new byte[4113];
				this.nodes = new RtfCompressConverter.Node[4097];
				int num = this.nodes.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					this.nodes[i].Parent = -1;
				}
				this.nodes[num].LeftChild = -1;
				this.nodes[num].RightChild = -1;
				this.codes = new byte[32];
				Array.Copy(RtfCompressCommon.PreloadData, 0, this.window, 0, RtfCompressCommon.PreloadData.Length);
				this.windowCurrent = RtfCompressCommon.PreloadData.Length - 17;
				Buffer.BlockCopy(this.window, 0, this.window, 4096, 17);
				for (int j = 0; j <= this.windowCurrent; j++)
				{
					this.AddNode(j, out this.match, out this.matchLength);
				}
				this.readLength = 17;
				this.lookAhead = 17;
				this.bufferStream = null;
				if (this.pushSink != null && this.pushSink.CanSeek)
				{
					this.initialStreamPosition = this.pushSink.Position;
					this.pushSink.Position += 16L;
					this.directOutput = true;
					return;
				}
				this.bufferStream = ApplicationServices.Provider.CreateTemporaryStorage();
				this.directOutput = false;
				if (this.writeBuffer == null)
				{
					this.writeBuffer = new byte[4096];
					this.writeEnd = this.writeBuffer.Length;
					this.writeCurrent = this.writeStart;
					return;
				}
				return;
			}
			else
			{
				if (mode != RtfCompressionMode.Uncompressed)
				{
					throw new ArgumentOutOfRangeException("CompressionMode");
				}
				this.uncompressed = true;
				if (this.pushSink != null && this.pushSink.CanSeek)
				{
					this.initialStreamPosition = this.pushSink.Position;
					this.pushSink.Position += 16L;
					this.directOutput = true;
					return;
				}
				if (this.pullSource != null && this.pullSource.CanSeek)
				{
					this.initialStreamPosition = this.pullSource.Position;
					return;
				}
				this.bufferStream = ApplicationServices.Provider.CreateTemporaryStorage();
				return;
			}
		}

		public void Run()
		{
			if (this.endOfFile)
			{
				return;
			}
			if (!this.flushing)
			{
				if (this.readCurrent == this.readEnd)
				{
					if (!this.inputEndOfFile && !base.ReadMoreData())
					{
						return;
					}
					if (this.inputEndOfFile)
					{
						if (!this.uncompressed)
						{
							if (this.writeCurrent == this.writeEnd && (this.directOutput ? (!base.GetOutputSpace()) : (!this.GetTempOutputSpace())))
							{
								return;
							}
							bool flag = this.Compress(true);
							if (flag || this.writeEnd == this.writeCurrent)
							{
								if (this.directOutput)
								{
									base.FlushOutput();
								}
								else
								{
									this.FlushTempOutput();
								}
							}
							if (!flag)
							{
								return;
							}
							this.inputDataSize = this.readFileOffset;
							this.outputDataSize = this.writeFileOffset;
						}
						this.PrepareHeader();
						if (!this.directOutput)
						{
							this.flushing = true;
							this.directOutput = true;
							this.inputEndOfFile = false;
							this.flushOffset = 0;
							this.writeFileOffset = 0;
							return;
						}
						this.RewriteHeader();
						this.endOfFile = true;
						if (this.pullSink != null)
						{
							this.pullSink.ReportEndOfFile();
							return;
						}
						this.pushSink.Flush();
						return;
					}
				}
				if (this.uncompressed)
				{
					this.CopyInputChunkTo(this.directOutput ? this.pushSink : this.bufferStream);
				}
				else
				{
					if (this.writeCurrent == this.writeEnd && (this.directOutput ? (!base.GetOutputSpace()) : (!this.GetTempOutputSpace())))
					{
						return;
					}
					this.Compress(false);
					if (this.writeEnd - this.writeCurrent == 0)
					{
						if (this.directOutput)
						{
							base.FlushOutput();
						}
						else
						{
							this.FlushTempOutput();
						}
					}
				}
				base.ReportRead();
				return;
			}
			if (this.writeCurrent == this.writeEnd && !base.GetOutputSpace())
			{
				return;
			}
			this.CopyBufferedData();
			base.FlushOutput();
			if (this.inputEndOfFile)
			{
				if (this.pullSink != null)
				{
					this.pullSink.ReportEndOfFile();
				}
				else
				{
					this.pushSink.Flush();
				}
				this.endOfFile = true;
			}
		}

		public bool Flush()
		{
			if (!this.endOfFile)
			{
				this.Run();
			}
			return this.endOfFile;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.pullSource != null)
				{
					this.pullSource.Dispose();
				}
				if (this.pushSink != null)
				{
					this.pushSink.Dispose();
				}
			}
			this.pullSource = null;
			this.pushSource = null;
			this.pullSink = null;
			this.pushSink = null;
		}

		private bool Compress(bool flush)
		{
			if (this.flushCodes && !this.FlushCodes())
			{
				return false;
			}
			if (this.readCurrent == this.readEnd && (!flush || this.flushedTail))
			{
				return this.flushedTail;
			}
			int num;
			if (this.lookAhead != 0)
			{
				for (;;)
				{
					if (this.readLength-- <= 0)
					{
						if (this.lookAhead == 0)
						{
							goto IL_23E;
						}
						if (this.matchLength > this.lookAhead)
						{
							this.matchLength = this.lookAhead;
						}
						this.flags >>= 1;
						if (this.matchLength <= 1)
						{
							this.codes[this.codesEnd++] = this.window[this.windowCurrent];
							this.readLength = 1;
						}
						else
						{
							this.flags |= 128;
							num = (this.match << 4 | this.matchLength - 2);
							this.codes[this.codesEnd++] = (byte)((num & 65280) >> 8);
							this.codes[this.codesEnd++] = (byte)(num & 255);
							this.readLength = this.matchLength;
						}
						if (++this.numFlags == 8)
						{
							this.FlushCodes(this.flags);
							this.flags = 0;
							this.numFlags = 0;
						}
						if (this.writeEnd == this.writeCurrent)
						{
							goto IL_23E;
						}
					}
					else
					{
						if (this.readCurrent == this.readEnd)
						{
							if (!flush)
							{
								break;
							}
							this.lookAhead--;
						}
						else
						{
							this.DeleteNode(this.windowCurrent + 17);
							this.window[this.windowCurrent + 17] = this.readBuffer[this.readCurrent++];
						}
						if (++this.windowCurrent == 4096)
						{
							this.windowCurrent = 0;
							Buffer.BlockCopy(this.window, 4096, this.window, 0, 17);
						}
						if (this.lookAhead > 0)
						{
							this.AddNode(this.windowCurrent, out this.match, out this.matchLength);
						}
					}
				}
				this.readLength++;
				return false;
				IL_23E:
				if (!flush || this.writeEnd == this.writeCurrent)
				{
					return false;
				}
			}
			this.flags >>= 1;
			this.flags |= 128;
			if (++this.numFlags < 8)
			{
				this.flags >>= 8 - this.numFlags;
			}
			num = this.windowCurrent << 4;
			this.codes[this.codesEnd++] = (byte)((num & 65280) >> 8);
			this.codes[this.codesEnd++] = (byte)(num & 255);
			this.flushedTail = true;
			return this.FlushCodes(this.flags);
		}

		private bool FlushCodes(int flags)
		{
			this.writeBuffer[this.writeCurrent++] = (byte)flags;
			this.codesCurrent = 0;
			this.flushCodes = true;
			this.crc = CRC32.ComputeCRC(this.crc, (uint)flags);
			return this.FlushCodes();
		}

		private bool FlushCodes()
		{
			while (this.writeCurrent != this.writeEnd)
			{
				byte b = this.codes[this.codesCurrent++];
				this.writeBuffer[this.writeCurrent++] = b;
				this.crc = CRC32.ComputeCRC(this.crc, (uint)b);
				if (this.codesCurrent == this.codesEnd)
				{
					this.flushCodes = false;
					this.codesEnd = 0;
					return true;
				}
			}
			return false;
		}

		private void DeleteNode(int node)
		{
			node &= 4095;
			int parent = (int)this.nodes[node].Parent;
			if (parent < 0)
			{
				return;
			}
			if (this.nodes[node].LeftChild < 0)
			{
				int rightChild = (int)this.nodes[node].RightChild;
				if ((int)this.nodes[parent].LeftChild == node)
				{
					this.nodes[parent].LeftChild = (short)rightChild;
				}
				else
				{
					this.nodes[parent].RightChild = (short)rightChild;
				}
				if (rightChild >= 0)
				{
					this.nodes[rightChild].Parent = (short)parent;
				}
			}
			else if (this.nodes[node].RightChild < 0)
			{
				int leftChild = (int)this.nodes[node].LeftChild;
				if ((int)this.nodes[parent].LeftChild == node)
				{
					this.nodes[parent].LeftChild = (short)leftChild;
				}
				else
				{
					this.nodes[parent].RightChild = (short)leftChild;
				}
				this.nodes[leftChild].Parent = (short)parent;
			}
			else
			{
				int num = (int)this.nodes[node].RightChild;
				if (this.nodes[num].LeftChild >= 0)
				{
					do
					{
						num = (int)this.nodes[num].LeftChild;
					}
					while (this.nodes[num].LeftChild >= 0);
					this.nodes[(int)this.nodes[num].Parent].LeftChild = this.nodes[num].RightChild;
					if (this.nodes[num].RightChild >= 0)
					{
						this.nodes[(int)this.nodes[num].RightChild].Parent = this.nodes[num].Parent;
					}
					this.nodes[num].RightChild = this.nodes[node].RightChild;
					this.nodes[num].LeftChild = this.nodes[node].LeftChild;
					if (this.nodes[num].RightChild >= 0)
					{
						this.nodes[(int)this.nodes[num].RightChild].Parent = (short)num;
					}
					if (this.nodes[num].LeftChild >= 0)
					{
						this.nodes[(int)this.nodes[num].LeftChild].Parent = (short)num;
					}
				}
				else
				{
					this.nodes[num].LeftChild = this.nodes[node].LeftChild;
					this.nodes[(int)this.nodes[num].LeftChild].Parent = (short)num;
				}
				if ((int)this.nodes[parent].LeftChild == node)
				{
					this.nodes[parent].LeftChild = (short)num;
				}
				else
				{
					this.nodes[parent].RightChild = (short)num;
				}
				this.nodes[num].Parent = (short)parent;
			}
			this.nodes[node].Parent = -1;
		}

		private void AddNode(int node, out int match, out int matchLength)
		{
			matchLength = 0;
			match = -1;
			int num = 4096;
			int i = (int)this.nodes[num].LeftChild;
			bool flag = true;
			while (i >= 0)
			{
				int num2 = 0;
				int num3 = 17;
				int num4 = node;
				int num5 = i;
				while (num3-- > 0)
				{
					num2 = (int)(this.window[num4++] - this.window[num5++]);
					if (num2 != 0)
					{
						break;
					}
				}
				num3 = 17 - (num3 + 1);
				if (num3 > matchLength)
				{
					matchLength = num3;
					match = i;
					if (matchLength == 17)
					{
						this.nodes[node] = this.nodes[i];
						this.nodes[i].Parent = -1;
						if (this.nodes[node].LeftChild >= 0)
						{
							this.nodes[(int)this.nodes[node].LeftChild].Parent = (short)node;
						}
						if (this.nodes[node].RightChild >= 0)
						{
							this.nodes[(int)this.nodes[node].RightChild].Parent = (short)node;
						}
						if (flag)
						{
							this.nodes[num].LeftChild = (short)node;
							return;
						}
						this.nodes[num].RightChild = (short)node;
						return;
					}
				}
				num = i;
				flag = (num2 < 0);
				i = (int)(flag ? this.nodes[i].LeftChild : this.nodes[i].RightChild);
			}
			this.nodes[node].LeftChild = -1;
			this.nodes[node].RightChild = -1;
			this.nodes[node].Parent = (short)num;
			if (flag)
			{
				this.nodes[num].LeftChild = (short)node;
				return;
			}
			this.nodes[num].RightChild = (short)node;
		}

		private void PrepareHeader()
		{
			this.header = new byte[16];
			RtfCompressCommon.ToBytes((uint)(this.outputDataSize + 12), this.header, 0);
			RtfCompressCommon.ToBytes((uint)this.inputDataSize, this.header, 4);
			RtfCompressCommon.ToBytes(this.uncompressed ? 1095517517U : 1967544908U, this.header, 8);
			RtfCompressCommon.ToBytes(this.crc, this.header, 12);
		}

		private void RewriteHeader()
		{
			long position = this.pushSink.Position;
			this.pushSink.Position = this.initialStreamPosition;
			this.pushSink.Write(this.header, 0, 16);
			this.pushSink.Position = position;
		}

		private void CopyInputChunkTo(Stream destinationStream)
		{
			int num = this.readEnd - this.readCurrent;
			this.outputDataSize += num;
			this.inputDataSize += num;
			if (destinationStream != null)
			{
				destinationStream.Write(this.readBuffer, this.readCurrent, num);
			}
			this.readCurrent = this.readEnd;
		}

		private void CopyBufferedData()
		{
			if (this.flushOffset < 16)
			{
				int num = Math.Min(16 - this.flushOffset, this.writeEnd - this.writeCurrent);
				Buffer.BlockCopy(this.header, this.flushOffset, this.writeBuffer, this.writeCurrent, num);
				this.writeCurrent += num;
				this.flushOffset += num;
				if (this.flushOffset < 16)
				{
					return;
				}
				if (this.bufferStream == null)
				{
					this.pullSource.Position = this.initialStreamPosition;
				}
				else
				{
					this.bufferStream.Position = 0L;
				}
			}
			if (this.writeEnd != this.writeCurrent)
			{
				int num;
				if (this.bufferStream != null)
				{
					num = this.bufferStream.Read(this.writeBuffer, this.writeCurrent, this.writeEnd - this.writeCurrent);
				}
				else
				{
					num = this.pullSource.Read(this.writeBuffer, this.writeCurrent, this.writeEnd - this.writeCurrent);
				}
				if (num == 0)
				{
					this.inputEndOfFile = true;
					return;
				}
				this.writeCurrent += num;
				this.flushOffset += num;
			}
		}

		private bool GetTempOutputSpace()
		{
			this.writeCurrent = 0;
			return true;
		}

		private void FlushTempOutput()
		{
			this.writeFileOffset += this.writeCurrent - this.writeStart;
			this.bufferStream.Write(this.writeBuffer, this.writeStart, this.writeCurrent - this.writeStart);
			this.writeCurrent = this.writeEnd;
		}

		private bool uncompressed;

		private Stream bufferStream;

		private bool flushing;

		private bool directOutput;

		private int flushOffset;

		private long initialStreamPosition;

		private int outputDataSize;

		private int inputDataSize;

		private uint crc;

		private byte[] header;

		private byte[] codes;

		private int codesCurrent;

		private int codesEnd;

		private bool flushCodes;

		private bool flushedTail;

		private int match;

		private int matchLength;

		private int lookAhead;

		private int readLength;

		private int numFlags;

		private int flags;

		private RtfCompressConverter.Node[] nodes;

		private struct Node
		{
			public short Parent;

			public short LeftChild;

			public short RightChild;
		}
	}
}
