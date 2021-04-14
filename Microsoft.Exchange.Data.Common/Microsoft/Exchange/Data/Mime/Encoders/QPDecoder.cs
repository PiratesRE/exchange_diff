using System;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime.Encoders
{
	public class QPDecoder : ByteEncoder
	{
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
			outputUsed = outputIndex;
			inputUsed = inputIndex;
			int num = inputIndex + inputSize;
			while (inputIndex != num && outputSize != 0)
			{
				if (this.encodedIndex != 0)
				{
					for (;;)
					{
						byte b = input[inputIndex++];
						if (32 != b && 9 != b)
						{
							this.encoded[this.encodedIndex++] = b;
							if (this.encodedIndex == 3)
							{
								break;
							}
						}
						if (inputIndex == num)
						{
							goto IL_164;
						}
					}
					this.encodedIndex = 0;
					byte b2 = this.encoded[1];
					byte b3 = this.encoded[2];
					if (b2 != 13 || b3 != 10)
					{
						byte b4 = ByteEncoder.Tables.NumFromHex[(int)b2];
						byte b5 = ByteEncoder.Tables.NumFromHex[(int)b3];
						byte b = (b4 == byte.MaxValue || b5 == byte.MaxValue) ? 61 : ((byte)((int)b4 << 4 | (int)b5));
						output[outputIndex++] = b;
						if (--outputSize == 0)
						{
							inputUsed = inputIndex - inputUsed;
							outputUsed = outputIndex - outputUsed;
							completed = (inputIndex == num);
							return;
						}
					}
				}
				IL_164:
				if (inputIndex != num)
				{
					int num2 = Math.Min(num - inputIndex, outputSize);
					int num3 = ByteString.IndexOf(input, 61, inputIndex, num2);
					if (input.GetLowerBound(0) - 1 == num3)
					{
						num3 = inputIndex + num2;
					}
					else
					{
						num2 = num3 - inputIndex;
						this.encoded[this.encodedIndex++] = 61;
						num3++;
					}
					if (0 < num2)
					{
						ByteEncoder.BlockCopy(input, inputIndex, output, outputIndex, num2);
						outputSize -= num2;
						outputIndex += num2;
					}
					inputIndex = num3;
				}
			}
			if (flush && this.encodedIndex != 0 && inputIndex == num)
			{
				if ((this.encodedIndex == 1 && 61 == this.encoded[0]) || this.encoded[1] == 13)
				{
					this.encodedIndex = 0;
				}
				else if (0 < outputSize)
				{
					output[outputIndex++] = this.encoded[0];
					if (0 < --outputSize)
					{
						if (1 < this.encodedIndex)
						{
							output[outputIndex++] = this.encoded[1];
						}
						this.encodedIndex = 0;
					}
					else if (1 < this.encodedIndex)
					{
						this.encoded[0] = this.encoded[1];
						this.encodedIndex--;
					}
				}
			}
			inputUsed = inputIndex - inputUsed;
			outputUsed = outputIndex - outputUsed;
			completed = (inputIndex == num && (!flush || 0 == this.encodedIndex));
		}

		public sealed override int GetMaxByteCount(int dataCount)
		{
			return dataCount;
		}

		public sealed override void Reset()
		{
			this.encodedIndex = 0;
		}

		public sealed override ByteEncoder Clone()
		{
			QPDecoder qpdecoder = base.MemberwiseClone() as QPDecoder;
			qpdecoder.encoded = (this.encoded.Clone() as byte[]);
			return qpdecoder;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private byte[] encoded = new byte[3];

		private int encodedIndex;
	}
}
