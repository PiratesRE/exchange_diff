using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	public interface ICompressAndObfuscate
	{
		int MaxCompressionSize { get; }

		int MinCompressionSize { get; }

		bool TryCompress(ArraySegment<byte> sources, ArraySegment<byte> destination, out int compressedSize);

		bool TryExpand(ArraySegment<byte> sources, ArraySegment<byte> destination);

		void Obfuscate(ArraySegment<byte> buffer);
	}
}
