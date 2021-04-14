using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RleCompressor : IIdCompressor
	{
		public byte[] Compress(byte[] streamIn, byte compressorId, out int outBytesRequired)
		{
			byte[] array = new byte[streamIn.Length];
			outBytesRequired = streamIn.Length;
			int num = 0;
			array[num++] = compressorId;
			if (num == streamIn.Length)
			{
				return streamIn;
			}
			int num3;
			for (int i = 1; i < streamIn.Length; i += num3)
			{
				array[num++] = streamIn[i];
				if (num == streamIn.Length)
				{
					return streamIn;
				}
				int num2 = Math.Min(257, streamIn.Length - i);
				num3 = 1;
				while (num3 < num2 && streamIn[i] == streamIn[i + num3])
				{
					num3++;
				}
				if (num3 > 1)
				{
					array[num++] = streamIn[i];
					if (num == streamIn.Length)
					{
						return streamIn;
					}
					array[num++] = (byte)(num3 - 2);
					if (num == streamIn.Length)
					{
						return streamIn;
					}
				}
			}
			outBytesRequired = num;
			return array;
		}

		public MemoryStream Decompress(byte[] input, int maxLength)
		{
			int capacity = Math.Min(input.Length, maxLength);
			MemoryStream memoryStream = new MemoryStream(capacity);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			for (int i = 1; i < input.Length; i++)
			{
				if (i == input.Length - 1 || input[i] != input[i + 1])
				{
					binaryWriter.Write(input[i]);
				}
				else
				{
					if (i == input.Length - 2)
					{
						ExTraceGlobals.StorageTracer.TraceDebug(0L, "RleCompressor.Decompress: Compressed bytes are invalid - missing character count");
						throw new InvalidIdMalformedException();
					}
					byte b = input[i + 2];
					for (int j = 0; j < (int)(b + 2); j++)
					{
						binaryWriter.Write(input[i]);
					}
					i += 2;
				}
				if (memoryStream.Length > (long)maxLength)
				{
					throw new InvalidIdMalformedException();
				}
			}
			binaryWriter.Flush();
			memoryStream.Position = 0L;
			return memoryStream;
		}
	}
}
