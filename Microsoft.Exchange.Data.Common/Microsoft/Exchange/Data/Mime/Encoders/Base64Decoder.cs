using System;

namespace Microsoft.Exchange.Data.Mime.Encoders
{
	public class Base64Decoder : ByteEncoder
	{
		public Base64Decoder()
		{
			this.decoded = new byte[3];
			this.encoded = new byte[4];
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
			int num = inputIndex + inputSize;
			if (this.decodedSize != 0)
			{
				int num2 = Math.Min(outputSize, this.decodedSize);
				if ((num2 & 2) != 0)
				{
					output[outputIndex++] = this.decoded[this.decodedIndex++];
					output[outputIndex++] = this.decoded[this.decodedIndex++];
				}
				if ((num2 & 1) != 0)
				{
					output[outputIndex++] = this.decoded[this.decodedIndex++];
				}
				outputSize -= num2;
				this.decodedSize -= num2;
				if (this.decodedSize == 0)
				{
					this.decodedIndex = 0;
				}
			}
			while (this.decodedSize == 0 && (inputIndex != num || (flush && this.encodedSize != 0)))
			{
				while (inputIndex != num && 4 != this.encodedSize)
				{
					byte b = input[inputIndex++];
					if (b != 61 && !ByteEncoder.IsWhiteSpace(b))
					{
						b -= 32;
						if ((int)b < ByteEncoder.Tables.Base64ToByte.Length)
						{
							b = ByteEncoder.Tables.Base64ToByte[(int)b];
							if (b < 64)
							{
								this.encoded[this.encodedSize++] = b;
							}
						}
					}
				}
				if (4 == this.encodedSize && 3 <= outputSize)
				{
					output[outputIndex] = (byte)((int)this.encoded[0] << 2 | this.encoded[1] >> 4);
					output[outputIndex + 1] = (byte)((int)this.encoded[1] << 4 | this.encoded[2] >> 2);
					output[outputIndex + 2] = (byte)((int)this.encoded[2] << 6 | (int)this.encoded[3]);
					outputSize -= 3;
					outputIndex += 3;
					this.encodedSize = 0;
				}
				else
				{
					if (4 != this.encodedSize && (!flush || num != inputIndex))
					{
						break;
					}
					if (2 > this.encodedSize)
					{
						this.encodedSize = 0;
						break;
					}
					if (this.encodedSize > 1)
					{
						this.decoded[this.decodedSize++] = (byte)((int)this.encoded[0] << 2 | this.encoded[1] >> 4);
					}
					if (this.encodedSize > 2)
					{
						this.decoded[this.decodedSize++] = (byte)((int)this.encoded[1] << 4 | this.encoded[2] >> 2);
					}
					if (this.encodedSize > 3)
					{
						this.decoded[this.decodedSize++] = (byte)((int)this.encoded[2] << 6 | (int)this.encoded[3]);
					}
					this.encodedSize = 0;
					int num3 = Math.Min(outputSize, this.decodedSize);
					if ((num3 & 2) != 0)
					{
						output[outputIndex++] = this.decoded[this.decodedIndex++];
						output[outputIndex++] = this.decoded[this.decodedIndex++];
					}
					if ((num3 & 1) != 0)
					{
						output[outputIndex++] = this.decoded[this.decodedIndex++];
					}
					outputSize -= num3;
					this.decodedSize -= num3;
					if (this.decodedSize == 0)
					{
						this.decodedIndex = 0;
					}
				}
			}
			outputUsed = outputIndex - outputUsed;
			inputUsed = inputIndex - inputUsed;
			completed = (num == inputIndex && this.decodedSize == 0 && (!flush || 0 == this.encodedSize));
		}

		public sealed override int GetMaxByteCount(int dataCount)
		{
			return (dataCount + 4) / 4 * 3;
		}

		public sealed override void Reset()
		{
			this.decodedSize = 0;
			this.decodedIndex = 0;
			this.encodedSize = 0;
		}

		public sealed override ByteEncoder Clone()
		{
			Base64Decoder base64Decoder = base.MemberwiseClone() as Base64Decoder;
			base64Decoder.decoded = (this.decoded.Clone() as byte[]);
			base64Decoder.encoded = (this.encoded.Clone() as byte[]);
			return base64Decoder;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private const int EncodedBlockSize = 4;

		private const int DecodedBlockSize = 3;

		private byte[] decoded;

		private int decodedSize;

		private int decodedIndex;

		private byte[] encoded;

		private int encodedSize;
	}
}
