using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PassthruCompressor : IIdCompressor
	{
		public byte[] Compress(byte[] streamIn, byte compressorId, out int outBytesRequired)
		{
			outBytesRequired = 0;
			return null;
		}

		public MemoryStream Decompress(byte[] input, int maxLength)
		{
			if (input.Length > maxLength)
			{
				throw new InvalidIdMalformedException();
			}
			MemoryStream memoryStream = new MemoryStream(input.Length - 1);
			memoryStream.Write(input, 1, input.Length - 1);
			memoryStream.Position = 0L;
			return memoryStream;
		}
	}
}
