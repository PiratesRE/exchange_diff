using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime.Encoders
{
	public class BinHexDecoder : ByteEncoder
	{
		public BinHexDecoder()
		{
			this.dataForkOnly = true;
		}

		public BinHexDecoder(bool dataForkOnly)
		{
			this.DataForkOnly = dataForkOnly;
		}

		public bool DataForkOnly
		{
			get
			{
				return this.dataForkOnly;
			}
			set
			{
				this.dataForkOnly = value;
			}
		}

		public MacBinaryHeader MacBinaryHeader
		{
			get
			{
				if (this.header == null)
				{
					return new MacBinaryHeader();
				}
				return this.header;
			}
		}

		public sealed override void Convert(byte[] input, int inputIndex, int inputSize, byte[] output, int outputIndex, int outputSize, bool flush, out int inputUsed, out int outputUsed, out bool completed)
		{
			if (inputSize != 0)
			{
				if (input == null)
				{
					throw new ArgumentNullException("input");
				}
				if (inputIndex < 0 || inputIndex >= input.Length)
				{
					throw new ArgumentOutOfRangeException("inputIndex");
				}
				if (inputSize < 0 || inputSize > input.Length - inputIndex)
				{
					throw new ArgumentOutOfRangeException("inputSize");
				}
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (outputIndex < 0 || outputIndex >= output.Length)
			{
				throw new ArgumentOutOfRangeException("outputIndex");
			}
			if (outputSize < 1 || outputSize > output.Length - outputIndex)
			{
				throw new ArgumentOutOfRangeException("outputSize");
			}
			inputUsed = inputIndex;
			outputUsed = outputIndex;
			for (;;)
			{
				if (this.extraSize != 0)
				{
					int num = Math.Min(this.extraSize, outputSize);
					Buffer.BlockCopy(this.extra, this.extraIndex, output, outputIndex, num);
					outputSize -= num;
					outputIndex += num;
					this.extraSize -= num;
					if (this.extraSize != 0)
					{
						this.extraIndex += num;
						goto IL_2F0;
					}
					this.extraIndex = 0;
				}
				if (this.repeatCount != 0)
				{
					switch (this.state)
					{
					case BinHexUtils.State.Data:
					case BinHexUtils.State.Resource:
						this.OutputChunk(output, ref outputIndex, ref outputSize);
						break;
					case BinHexUtils.State.DataCRC:
						goto IL_123;
					default:
						goto IL_123;
					}
					IL_13B:
					if (this.bytesNeeded == 0)
					{
						this.TransitionState();
					}
					if (outputSize == 0)
					{
						goto IL_2F0;
					}
					continue;
					IL_123:
					this.OutputChunk(this.scratch, ref this.scratchIndex, ref this.scratchSize);
					goto IL_13B;
				}
				if (this.lineReady)
				{
					byte b = this.encoded[this.encodedIndex++];
					this.encodedSize--;
					if (this.encodedSize == 0)
					{
						this.lineReady = false;
					}
					if (ByteEncoder.IsWhiteSpace(b))
					{
						continue;
					}
					if (this.state == BinHexUtils.State.Started)
					{
						if (58 == b)
						{
							this.state = BinHexUtils.State.HdrFileSize;
							this.bytesNeeded = 1;
							this.scratchSize = this.scratch.Length;
							this.scratchIndex = 0;
							this.runningCRC = 0;
							continue;
						}
						break;
					}
					else
					{
						b -= 32;
						byte b2 = ((int)b >= BinHexDecoder.Dictionary.Length) ? 127 : BinHexDecoder.Dictionary[(int)b];
						if (b2 == 127)
						{
							goto Block_20;
						}
						if (this.accumCount == 0)
						{
							this.accum = (int)b2;
							this.accumCount++;
							continue;
						}
						this.accum = (this.accum << 6 | (int)b2);
						b2 = (byte)(this.accum >> BinHexDecoder.ShiftTabe[this.accumCount] & 255);
						this.accumCount++;
						this.accumCount %= Marshal.SizeOf(this.accum);
						if (this.repeatCheck)
						{
							this.repeatCheck = false;
							if (b2 == 0)
							{
								this.decodedByte = 144;
								this.repeatCount = 1;
								continue;
							}
							this.repeatCount = (int)(b2 - 1);
							continue;
						}
						else
						{
							if (144 == b2)
							{
								this.repeatCheck = true;
								continue;
							}
							this.decodedByte = b2;
							this.repeatCount = 1;
							continue;
						}
					}
				}
				IL_2F0:
				if (this.lineReady)
				{
					goto IL_316;
				}
				this.lineReady = this.ReadLine(input, ref inputIndex, ref inputSize, flush);
				if (!this.lineReady)
				{
					goto IL_316;
				}
			}
			throw new ByteEncoderException(EncodersStrings.BinHexDecoderFirstNonWhitespaceMustBeColon);
			Block_20:
			throw new ByteEncoderException(EncodersStrings.BinHexDecoderFoundInvalidCharacter);
			IL_316:
			outputUsed = outputIndex - outputUsed;
			inputUsed = inputIndex - inputUsed;
			completed = (inputSize == 0 && this.extraSize == 0 && this.repeatCount == 0 && !this.lineReady);
			if (flush && completed)
			{
				if (this.state != BinHexUtils.State.Ending)
				{
					throw new ByteEncoderException(EncodersStrings.BinHexDecoderDataCorrupt);
				}
				this.Reset();
			}
		}

		public sealed override int GetMaxByteCount(int dataCount)
		{
			throw new NotImplementedException();
		}

		public sealed override void Reset()
		{
			this.state = BinHexUtils.State.Starting;
			this.lineReady = false;
			this.encodedSize = 0;
			this.repeatCount = 0;
			this.extraIndex = 0;
			this.extraSize = 0;
		}

		public sealed override ByteEncoder Clone()
		{
			BinHexDecoder binHexDecoder = base.MemberwiseClone() as BinHexDecoder;
			binHexDecoder.encoded = (this.encoded.Clone() as byte[]);
			binHexDecoder.scratch = (this.scratch.Clone() as byte[]);
			binHexDecoder.extra = (this.extra.Clone() as byte[]);
			binHexDecoder.header = ((this.header != null) ? this.header.Clone() : null);
			return binHexDecoder;
		}

		private void OutputChunk(byte[] bytes, ref int index, ref int size)
		{
			int num = Math.Min(this.bytesNeeded, this.repeatCount);
			num = Math.Min(size, num);
			if (bytes != null)
			{
				for (int i = 0; i < num; i++)
				{
					bytes[index++] = this.decodedByte;
				}
				size -= num;
			}
			this.runningCRC = BinHexUtils.CalculateCrc(this.decodedByte, num, this.runningCRC);
			this.bytesNeeded -= num;
			this.repeatCount -= num;
		}

		private bool ReadLine(byte[] input, ref int inputIndex, ref int inputSize, bool flush)
		{
			if (this.state == BinHexUtils.State.Ending)
			{
				inputIndex += inputSize;
				inputSize = 0;
				return false;
			}
			bool result = false;
			while (inputSize != 0 || (flush && this.encodedSize != 0))
			{
				byte b = 10;
				if (inputSize != 0)
				{
					inputSize--;
					b = input[inputIndex++];
				}
				if (b != 10)
				{
					if (this.encodedSize >= this.encoded.Length)
					{
						throw new ByteEncoderException(EncodersStrings.BinHexDecoderLineTooLong);
					}
					this.encoded[this.encodedSize++] = b;
				}
				else if (this.state == BinHexUtils.State.Starting)
				{
					uint num = 0U;
					while ((ulong)num < (ulong)((long)this.encodedSize))
					{
						if (!ByteEncoder.IsWhiteSpace(this.encoded[(int)((UIntPtr)num)]))
						{
							if ((ulong)((uint)((long)this.encodedSize - (long)((ulong)num))) >= (ulong)((long)BinHexDecoder.BinHexPrologue.Length) && ByteEncoder.BeginsWithNI(this.encoded, (int)num, BinHexDecoder.BinHexPrologue, BinHexDecoder.BinHexPrologue.Length))
							{
								this.state = BinHexUtils.State.Started;
								this.repeatCheck = false;
								this.accumCount = 0;
								this.accum = 0;
								this.header = null;
								break;
							}
							throw new ByteEncoderException(EncodersStrings.BinHexDecoderLineCorrupt);
						}
						else
						{
							num += 1U;
						}
					}
					this.encodedSize = 0;
				}
				else
				{
					while (this.encodedSize != 0 && (this.encoded[this.encodedSize - 1] == 13 || this.encoded[this.encodedSize - 1] == 32 || this.encoded[this.encodedSize - 1] == 9))
					{
						this.encodedSize--;
					}
					if (this.encodedSize != 0)
					{
						result = true;
						this.encodedIndex = 0;
						break;
					}
					if (this.state == BinHexUtils.State.Started)
					{
					}
				}
			}
			return result;
		}

		private void TransitionState()
		{
			switch (this.state)
			{
			case BinHexUtils.State.HdrFileSize:
				if (this.scratch[0] > 63)
				{
					throw new ByteEncoderException(EncodersStrings.BinHexDecoderFileNameTooLong);
				}
				this.state = BinHexUtils.State.Header;
				this.bytesNeeded = (int)(this.scratch[0] + 21);
				return;
			case BinHexUtils.State.Header:
				this.header = new BinHexHeader(this.scratch);
				if (!this.dataForkOnly)
				{
					byte[] bytes = this.header.GetBytes();
					Buffer.BlockCopy(bytes, 0, this.extra, 0, bytes.Length);
					this.extraIndex = 0;
					this.extraSize = bytes.Length;
				}
				if (0L != this.header.DataForkLength)
				{
					this.state = BinHexUtils.State.Data;
					this.bytesNeeded = (int)this.header.DataForkLength;
					this.runningCRC = 0;
					return;
				}
				this.checksum = this.runningCRC;
				this.checksum = BinHexUtils.CalculateCrc(this.checksum);
				this.state = BinHexUtils.State.DataCRC;
				this.bytesNeeded = 2;
				this.scratchSize = this.scratch.Length;
				this.scratchIndex = 0;
				return;
			case BinHexUtils.State.Data:
				this.checksum = this.runningCRC;
				this.checksum = BinHexUtils.CalculateCrc(this.checksum);
				this.state = BinHexUtils.State.DataCRC;
				this.bytesNeeded = 2;
				this.scratchSize = this.scratch.Length;
				this.scratchIndex = 0;
				return;
			case BinHexUtils.State.DataCRC:
			{
				if ((this.checksum & 65280) >> 8 != (int)this.scratch[0] || (this.checksum & 255) != (ushort)this.scratch[1])
				{
					throw new ByteEncoderException(EncodersStrings.BinHexDecoderBadCrc);
				}
				if (this.dataForkOnly)
				{
					this.state = BinHexUtils.State.Ending;
					this.repeatCount = 0;
					this.encodedSize = 0;
					this.lineReady = false;
					return;
				}
				int num = (this.header.DataForkLength % 128L != 0L) ? ((int)(128L - this.header.DataForkLength % 128L)) : 0;
				if (num != 0)
				{
					Array.Clear(this.extra, 0, num);
					this.extraSize = num;
				}
				if (this.header.ResourceForkLength > 0L)
				{
					this.state = BinHexUtils.State.Resource;
					this.bytesNeeded = (int)this.header.ResourceForkLength;
					this.runningCRC = 0;
					return;
				}
				this.checksum = 0;
				this.checksum = BinHexUtils.CalculateCrc(this.checksum);
				this.state = BinHexUtils.State.ResourceCRC;
				this.bytesNeeded = 2;
				this.scratchSize = this.scratch.Length;
				this.scratchIndex = 0;
				return;
			}
			case BinHexUtils.State.Resource:
				this.checksum = this.runningCRC;
				this.checksum = BinHexUtils.CalculateCrc(this.checksum);
				this.state = BinHexUtils.State.ResourceCRC;
				this.bytesNeeded = 2;
				this.scratchSize = this.scratch.Length;
				this.scratchIndex = 0;
				return;
			case BinHexUtils.State.ResourceCRC:
			{
				if ((this.checksum & 65280) >> 8 != (int)this.scratch[0] || (this.checksum & 255) != (ushort)this.scratch[1])
				{
					throw new ByteEncoderException(EncodersStrings.BinHexDecoderBadResourceForkCrc);
				}
				int num = (this.header.ResourceForkLength % 128L != 0L) ? ((int)(128L - this.header.ResourceForkLength % 128L)) : 0;
				if (num != 0)
				{
					Array.Clear(this.extra, 0, num);
					this.extraSize = num;
				}
				this.state = BinHexUtils.State.Ending;
				this.repeatCount = 0;
				this.encodedSize = 0;
				this.lineReady = false;
				return;
			}
			}
			throw new Exception(EncodersStrings.BinHexDecoderInternalError);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static BinHexDecoder()
		{
			int[] array = new int[4];
			array[1] = 4;
			array[2] = 2;
			BinHexDecoder.ShiftTabe = array;
			BinHexDecoder.BinHexPrologue = CTSGlobals.AsciiEncoding.GetBytes("(This file must be converted with BinHex");
			BinHexDecoder.Dictionary = new byte[]
			{
				127,
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				127,
				127,
				13,
				14,
				15,
				16,
				17,
				18,
				19,
				127,
				20,
				21,
				22,
				127,
				127,
				127,
				127,
				127,
				22,
				23,
				24,
				25,
				26,
				27,
				28,
				29,
				30,
				31,
				32,
				33,
				34,
				35,
				36,
				127,
				37,
				38,
				39,
				40,
				41,
				42,
				43,
				127,
				44,
				45,
				46,
				47,
				127,
				127,
				127,
				127,
				48,
				49,
				50,
				51,
				52,
				53,
				54,
				127,
				55,
				56,
				57,
				58,
				59,
				60,
				127,
				127,
				61,
				62,
				63,
				127,
				127,
				127,
				127,
				127
			};
		}

		private static readonly int[] ShiftTabe;

		private static readonly byte[] BinHexPrologue;

		private static readonly byte[] Dictionary;

		private BinHexUtils.State state;

		private BinHexHeader header;

		private byte[] encoded = new byte[128];

		private int encodedIndex;

		private int encodedSize;

		private bool lineReady;

		private bool repeatCheck;

		private byte decodedByte;

		private int repeatCount;

		private int accumCount;

		private int accum;

		private byte[] extra = new byte[128];

		private int extraSize;

		private int extraIndex;

		private int bytesNeeded;

		private ushort runningCRC;

		private ushort checksum;

		private byte[] scratch = new byte[128];

		private int scratchIndex;

		private int scratchSize;

		private bool dataForkOnly;
	}
}
