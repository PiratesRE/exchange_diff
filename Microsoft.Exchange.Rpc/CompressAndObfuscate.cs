using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.Rpc
{
	public class CompressAndObfuscate : ICompressAndObfuscate
	{
		private CompressAndObfuscate()
		{
		}

		public static ICompressAndObfuscate Instance
		{
			get
			{
				if (CompressAndObfuscate.instance == null)
				{
					CompressAndObfuscate.instance = new CompressAndObfuscate();
				}
				return CompressAndObfuscate.instance;
			}
		}

		public virtual int MaxCompressionSize
		{
			get
			{
				return EmsmdbConstants.MaxRopBufferSize;
			}
		}

		public virtual int MinCompressionSize
		{
			get
			{
				return EmsmdbConstants.MinCompressionSize;
			}
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public unsafe virtual bool TryCompress(ArraySegment<byte> source, ArraySegment<byte> destination, ref int compressedSize)
		{
			compressedSize = 0;
			if (source.Count > destination.Count)
			{
				throw new ArgumentException("source is larger than destination");
			}
			if (source.Count < EmsmdbConstants.MinCompressionSize)
			{
				throw new ArgumentException("source is too small to compress");
			}
			uint count = source.Count;
			uint num = count;
			ref byte pbOrig = ref source.Array[source.Offset];
			try
			{
				ref byte pbComp = ref destination.Array[destination.Offset];
				try
				{
					<Module>.Compress(ref pbOrig, count, ref pbComp, &num);
				}
				catch
				{
					throw;
				}
			}
			catch
			{
				throw;
			}
			byte condition = (num <= count) ? 1 : 0;
			ExAssert.Assert(condition != 0, "Compress should never return anything larger than the max destination buffer");
			if (num >= count)
			{
				return false;
			}
			compressedSize = num;
			return true;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public virtual bool TryExpand(ArraySegment<byte> source, ArraySegment<byte> destination)
		{
			uint count = source.Count;
			uint count2 = destination.Count;
			ref byte pbComp = ref source.Array[source.Offset];
			try
			{
				ref byte pbOrig = ref destination.Array[destination.Offset];
				try
				{
					if (<Module>.Decompress(ref pbOrig, count2, ref pbComp, count) != null)
					{
						goto IL_59;
					}
				}
				catch
				{
					throw;
				}
			}
			catch
			{
				throw;
			}
			return false;
			IL_59:
			try
			{
			}
			catch
			{
				throw;
			}
			return true;
		}

		public unsafe virtual void Obfuscate(ArraySegment<byte> buffer)
		{
			uint count = buffer.Count;
			ref byte byte& = ref buffer.Array[buffer.Offset];
			try
			{
				byte* ptr = ref byte&;
				if (0 < count)
				{
					uint num = count;
					do
					{
						*ptr ^= 165;
						ptr += 1L;
						num += -1;
					}
					while (num > 0);
				}
			}
			catch
			{
				throw;
			}
		}

		private static CompressAndObfuscate instance = null;
	}
}
