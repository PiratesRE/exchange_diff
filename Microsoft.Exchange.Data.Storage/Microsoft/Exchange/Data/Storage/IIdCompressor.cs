using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IIdCompressor
	{
		byte[] Compress(byte[] streamIn, byte compressorId, out int outBytesRequired);

		MemoryStream Decompress(byte[] input, int maxLength);
	}
}
