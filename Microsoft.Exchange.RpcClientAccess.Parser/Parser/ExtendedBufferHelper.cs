using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExtendedBufferHelper
	{
		public static ArraySegment<byte> Wrap(ICompressAndObfuscate compressAndObfuscate, ArraySegment<byte> extendedBuffer, ArraySegment<byte> buffer, bool compress, bool xorMagic)
		{
			long num;
			long num2;
			return ExtendedBufferHelper.Wrap(compressAndObfuscate, extendedBuffer, buffer, compress, xorMagic, out num, out num2);
		}

		public static ArraySegment<byte> Wrap(ICompressAndObfuscate compressAndObfuscate, ArraySegment<byte> extendedBuffer, ArraySegment<byte> buffer, bool compress, bool xorMagic, out long rawPayloadSize, out long uncompressedPayloadSize)
		{
			bool flag = false;
			bool flag2 = false;
			Util.ThrowOnNullArgument(compressAndObfuscate, "compressAndObfuscate");
			Util.ThrowOnNullArgument(extendedBuffer.Array, "extendedBuffer.Array");
			Util.ThrowOnNullArgument(buffer.Array, "buffer.Array");
			uncompressedPayloadSize = (long)buffer.Count;
			if (extendedBuffer.Count < 8)
			{
				throw new BufferParseException(string.Format("extendedBuffer not even large enough for extended header; extendedBuffer.Count = {0}", extendedBuffer.Count));
			}
			if (extendedBuffer.Count - 8 < buffer.Count)
			{
				throw new BufferParseException(string.Format("buffer is too large for extendedBuffer; extendedBuffer.Count = {0}, buffer.Count = {1}", extendedBuffer.Count, buffer.Count));
			}
			if (buffer.Count > compressAndObfuscate.MaxCompressionSize)
			{
				throw new BufferParseException(string.Format("buffer is not a valid size; buffer.Count = {0}", buffer.Count));
			}
			ArraySegment<byte>? arraySegment = null;
			int count;
			if (compress && buffer.Count >= compressAndObfuscate.MinCompressionSize && compressAndObfuscate.TryCompress(buffer, extendedBuffer.SubSegmentToEnd(8), out count))
			{
				flag = true;
				arraySegment = new ArraySegment<byte>?(new ArraySegment<byte>(extendedBuffer.Array, extendedBuffer.Offset, count));
			}
			if (arraySegment == null)
			{
				if (buffer.Count > 0 && xorMagic)
				{
					compressAndObfuscate.Obfuscate(buffer);
					flag2 = true;
				}
				arraySegment = new ArraySegment<byte>?(buffer);
			}
			rawPayloadSize = (long)arraySegment.Value.Count;
			using (BufferWriter bufferWriter = new BufferWriter(extendedBuffer))
			{
				ExtendedBufferFlag extendedBufferFlag = ExtendedBufferFlag.Last;
				if (flag2)
				{
					extendedBufferFlag |= ExtendedBufferFlag.Obfuscated;
				}
				if (flag)
				{
					extendedBufferFlag |= ExtendedBufferFlag.Compressed;
				}
				ExtendedBufferHeader extendedBufferHeader = new ExtendedBufferHeader(extendedBufferFlag, (ushort)arraySegment.Value.Count, (ushort)buffer.Count);
				extendedBufferHeader.Serialize(bufferWriter);
				if (!flag && arraySegment.Value.Count > 0)
				{
					bufferWriter.WriteBytesSegment(arraySegment.Value);
				}
			}
			return extendedBuffer.SubSegment(0, arraySegment.Value.Count + 8);
		}

		public static ArraySegment<byte> Unwrap(ICompressAndObfuscate compressAndObfuscate, ArraySegment<byte> bufferArraySegment, ArraySegment<byte> uncompressedPayloadArraySegment, out ArraySegment<byte>? nextBufferArraySegment)
		{
			long num;
			long num2;
			return ExtendedBufferHelper.Unwrap(compressAndObfuscate, bufferArraySegment, uncompressedPayloadArraySegment, out nextBufferArraySegment, out num, out num2);
		}

		internal static ArraySegment<byte> Unwrap(ICompressAndObfuscate compressAndObfuscate, ArraySegment<byte> bufferArraySegment, ArraySegment<byte> uncompressedPayloadArraySegment, out ArraySegment<byte>? nextBufferArraySegment, out long rawPayloadSize, out long uncompressedPayloadSize)
		{
			nextBufferArraySegment = null;
			rawPayloadSize = 0L;
			uncompressedPayloadSize = 0L;
			Util.ThrowOnNullArgument(compressAndObfuscate, "compressAndObfuscate");
			Util.ThrowOnNullArgument(bufferArraySegment.Array, "bufferArraySegment.Array");
			Util.ThrowOnNullArgument(uncompressedPayloadArraySegment.Array, "uncompressedPayloadArraySegment.Array");
			ArraySegment<byte> result;
			using (BufferReader bufferReader = Reader.CreateBufferReader(bufferArraySegment))
			{
				ExtendedBufferHeader extendedBufferHeader = new ExtendedBufferHeader(bufferReader);
				if ((int)extendedBufferHeader.PayloadSize > bufferArraySegment.Count - 8)
				{
					throw new BufferParseException(string.Format("PayloadSize too large; {0}, bufferArraySegment={1}.", extendedBufferHeader.ToString(), bufferArraySegment.Count));
				}
				if ((int)extendedBufferHeader.PayloadSize < bufferArraySegment.Count - 8)
				{
					if (extendedBufferHeader.IsLast)
					{
						throw new BufferParseException("Data following last buffer in chain.");
					}
					nextBufferArraySegment = new ArraySegment<byte>?(bufferArraySegment.SubSegmentToEnd((int)(8 + extendedBufferHeader.PayloadSize)));
				}
				else if (!extendedBufferHeader.IsLast)
				{
					throw new BufferParseException("Must be a last in chain");
				}
				if (extendedBufferHeader.PayloadSize > extendedBufferHeader.UncompressedSize)
				{
					throw new BufferParseException(string.Format("PayloadSize > UncompressedSize; {0}.", extendedBufferHeader.ToString()));
				}
				if ((int)extendedBufferHeader.UncompressedSize > compressAndObfuscate.MaxCompressionSize)
				{
					throw new BufferParseException(string.Format("UncompressedSize too large; {0}.", extendedBufferHeader.ToString()));
				}
				if (extendedBufferHeader.IsCompressed && extendedBufferHeader.PayloadSize == extendedBufferHeader.UncompressedSize)
				{
					throw new BufferParseException(string.Format("Data is compressed, but uncompressed size is same as payload size; {0}.", extendedBufferHeader.ToString()));
				}
				if (extendedBufferHeader.IsCompressed && extendedBufferHeader.PayloadSize == 0)
				{
					throw new BufferParseException("Data is compressed, but payload length is zero.");
				}
				if (!extendedBufferHeader.IsCompressed && extendedBufferHeader.PayloadSize != extendedBufferHeader.UncompressedSize)
				{
					throw new BufferParseException(string.Format("Data is not compressed, but payload size and uncompressed size are not the same; {0}.", extendedBufferHeader.ToString()));
				}
				if (extendedBufferHeader.IsObfuscated && extendedBufferHeader.PayloadSize > 0)
				{
					compressAndObfuscate.Obfuscate(bufferArraySegment.SubSegment(8, (int)extendedBufferHeader.PayloadSize));
				}
				if (extendedBufferHeader.IsCompressed)
				{
					if (uncompressedPayloadArraySegment.Count < (int)extendedBufferHeader.UncompressedSize)
					{
						throw new BufferParseException("Uncompressed size is too large for payload buffer");
					}
					if (!compressAndObfuscate.TryExpand(bufferArraySegment.SubSegment(8, (int)extendedBufferHeader.PayloadSize), uncompressedPayloadArraySegment.SubSegment(0, (int)extendedBufferHeader.UncompressedSize)))
					{
						throw new BufferParseException("Unable to unpack compressed payload.");
					}
				}
				else
				{
					uncompressedPayloadArraySegment = bufferArraySegment.SubSegment(8, (int)extendedBufferHeader.PayloadSize);
				}
				rawPayloadSize = (long)((ulong)extendedBufferHeader.PayloadSize);
				uncompressedPayloadSize = (long)((ulong)extendedBufferHeader.UncompressedSize);
				result = uncompressedPayloadArraySegment.SubSegment(0, (int)extendedBufferHeader.UncompressedSize);
			}
			return result;
		}

		internal static void ClearLastFlag(ArraySegment<byte> buffer)
		{
			if (buffer.Array == null)
			{
				throw new ArgumentNullException("buffer.Array");
			}
			if (buffer.Count < 8)
			{
				throw new BufferParseException(string.Format("Buffer not even large enough for an extended header; buffer={0}", buffer.Count));
			}
			ExtendedBufferHeader extendedBufferHeader;
			using (BufferReader bufferReader = Reader.CreateBufferReader(buffer))
			{
				extendedBufferHeader = new ExtendedBufferHeader(bufferReader);
			}
			extendedBufferHeader.IsLast = false;
			using (BufferWriter bufferWriter = new BufferWriter(buffer))
			{
				extendedBufferHeader.Serialize(bufferWriter);
			}
		}
	}
}
