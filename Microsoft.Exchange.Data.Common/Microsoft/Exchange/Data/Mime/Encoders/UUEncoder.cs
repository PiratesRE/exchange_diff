using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime.Encoders
{
	public class UUEncoder : ByteEncoder
	{
		public UUEncoder()
		{
		}

		public UUEncoder(string fileName)
		{
			this.FileName = fileName;
		}

		public string FileName
		{
			get
			{
				if (this.fileName != null)
				{
					return CTSGlobals.AsciiEncoding.GetString(this.fileName, 0, this.fileName.Length);
				}
				return string.Empty;
			}
			set
			{
				int num = 0;
				if (value != null)
				{
					num = value.Length;
				}
				if (num > 48)
				{
					throw new ArgumentOutOfRangeException("FileName", EncodersStrings.UUEncoderFileNameTooLong(48));
				}
				if (num == 0)
				{
					this.fileName = null;
					return;
				}
				this.fileName = CTSGlobals.AsciiEncoding.GetBytes(value);
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
			if (this.numLines == 0)
			{
				this.outLineReady = false;
				this.bufferSize = 0;
				Array.Clear(this.chunk, 0, this.chunk.Length);
				this.chunkIndex = 0;
				if (this.fileName != null)
				{
					ByteEncoder.BlockCopy(UUEncoder.Prologue, 0, this.buffer, 0, UUEncoder.Prologue.Length);
					this.bufferSize = UUEncoder.Prologue.Length;
					ByteEncoder.BlockCopy(this.fileName, 0, this.buffer, this.bufferSize, this.fileName.Length);
					this.bufferSize += this.fileName.Length;
					ByteEncoder.BlockCopy(ByteEncoder.LineWrap, 0, this.buffer, this.bufferSize, ByteEncoder.LineWrap.Length);
					this.bufferSize += ByteEncoder.LineWrap.Length;
					this.bufferIndex = 0;
					this.outLineReady = true;
					this.numLines++;
				}
			}
			inputUsed = inputIndex;
			outputUsed = outputIndex;
			do
			{
				if (this.outLineReady)
				{
					int num = Math.Min(this.bufferSize, outputSize);
					ByteEncoder.BlockCopy(this.buffer, this.bufferIndex, output, outputIndex, num);
					outputSize -= num;
					outputIndex += num;
					this.bufferSize -= num;
					this.bufferIndex += num;
					if (this.bufferSize != 0)
					{
						break;
					}
					this.outLineReady = false;
					if (3 == this.bufferIndex)
					{
						if (this.fileName != null)
						{
							ByteEncoder.BlockCopy(UUEncoder.Epilogue, 0, this.buffer, 0, UUEncoder.Epilogue.Length);
							this.bufferSize = UUEncoder.Epilogue.Length;
							this.bufferIndex = 0;
							this.outLineReady = true;
							continue;
						}
						break;
					}
					else
					{
						if (5 == this.bufferIndex && this.fileName != null && this.buffer[0] == 101)
						{
							break;
						}
						this.bufferIndex = 0;
					}
				}
				if (this.bufferSize == 0)
				{
					this.buffer[this.bufferSize++] = 0;
					this.bufferIndex = 0;
					this.rawCount = 0;
					this.numLines++;
				}
				while (inputSize != 0 || flush)
				{
					if (inputSize != 0)
					{
						int num2 = Math.Min(3 - this.chunkIndex, inputSize);
						if ((num2 & 2) != 0)
						{
							this.chunk[this.chunkIndex++] = input[inputIndex++];
							this.chunk[this.chunkIndex++] = input[inputIndex++];
						}
						if ((num2 & 1) != 0)
						{
							this.chunk[this.chunkIndex++] = input[inputIndex++];
						}
						inputSize -= num2;
						if (this.chunkIndex != 3)
						{
							continue;
						}
					}
					if (this.chunkIndex != 0)
					{
						if (3 != this.chunkIndex)
						{
							Array.Clear(this.chunk, this.chunkIndex, 3 - this.chunkIndex);
						}
						this.buffer[this.bufferSize++] = UUEncoder.UUEncode(this.chunk[0] >> 2);
						this.buffer[this.bufferSize++] = UUEncoder.UUEncode((int)this.chunk[0] << 4 | this.chunk[1] >> 4);
						this.buffer[this.bufferSize++] = UUEncoder.UUEncode((int)this.chunk[1] << 2 | this.chunk[2] >> 6);
						this.buffer[this.bufferSize++] = UUEncoder.UUEncode((int)this.chunk[2]);
						this.rawCount += this.chunkIndex;
						this.chunkIndex = 0;
					}
					if (this.bufferSize == 61 || (flush && inputSize == 0))
					{
						this.buffer[0] = UUEncoder.UUEncode(this.rawCount);
						this.buffer[this.bufferSize++] = 13;
						this.buffer[this.bufferSize++] = 10;
						this.outLineReady = true;
						break;
					}
				}
			}
			while (this.outLineReady);
			inputUsed = inputIndex - inputUsed;
			outputUsed = outputIndex - outputUsed;
			completed = (inputSize == 0 && (!flush || 0 == this.bufferSize));
			if (flush && completed)
			{
				this.numLines = 0;
			}
		}

		public sealed override int GetMaxByteCount(int dataCount)
		{
			int num = (dataCount + 3) / 3 * 4;
			int num2 = (num + 60) / 60;
			int num3 = num2 * 63;
			if (this.fileName != null)
			{
				num3 += UUEncoder.Prologue.Length + ByteEncoder.LineWrap.Length;
				num3 += this.fileName.Length;
				num3 += UUEncoder.Epilogue.Length;
			}
			return num3;
		}

		public sealed override void Reset()
		{
			this.numLines = 0;
			this.bufferIndex = 0;
			this.bufferSize = 0;
			this.rawCount = 0;
			this.chunkIndex = 0;
		}

		public sealed override ByteEncoder Clone()
		{
			UUEncoder uuencoder = base.MemberwiseClone() as UUEncoder;
			uuencoder.buffer = (this.buffer.Clone() as byte[]);
			uuencoder.chunk = (this.chunk.Clone() as byte[]);
			if (this.fileName != null)
			{
				uuencoder.fileName = (this.fileName.Clone() as byte[]);
			}
			return uuencoder;
		}

		private static byte UUEncode(int c)
		{
			return (byte)(((c & 63) != 0) ? ((c & 63) + 32) : 96);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private const int MaximumCharacters = 60;

		private const int DecodedBlockSize = 3;

		private const int EncodedBlockSize = 4;

		private const int MaxFileNameLength = 48;

		private static readonly byte[] Prologue = CTSGlobals.AsciiEncoding.GetBytes("begin 600 ");

		private static readonly byte[] Epilogue = CTSGlobals.AsciiEncoding.GetBytes("end\r\n");

		private bool outLineReady;

		private byte[] buffer = new byte[63];

		private int bufferIndex;

		private int bufferSize;

		private int numLines;

		private byte[] chunk = new byte[3];

		private int chunkIndex;

		private int rawCount;

		private byte[] fileName;
	}
}
