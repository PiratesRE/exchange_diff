using System;
using System.IO;
using System.IO.Compression;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SerializationHelper
	{
		public static void Compress(MemoryStream readStream, MemoryStream writeStream)
		{
			if (readStream == null || readStream.Length == 0L)
			{
				return;
			}
			readStream.Seek(0L, SeekOrigin.Begin);
			using (GZipStream gzipStream = new GZipStream(writeStream, CompressionMode.Compress, true))
			{
				readStream.CopyTo(gzipStream);
			}
		}

		public static void Decompress(MemoryStream readStream, MemoryStream writeStream, byte[] transferBuffer)
		{
			if (readStream == null || readStream.Length == 0L)
			{
				return;
			}
			using (GZipStream gzipStream = new GZipStream(readStream, CompressionMode.Decompress, true))
			{
				try
				{
					Util.StreamHandler.CopyStreamData(gzipStream, writeStream, null, 0, transferBuffer);
				}
				catch (InvalidDataException innerException)
				{
					throw new CustomSerializationInvalidDataException(innerException);
				}
			}
		}
	}
}
