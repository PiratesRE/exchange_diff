using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc
{
	internal class RpcBufferPool
	{
		private RpcBufferPool()
		{
			this.BufferPool256k = new BufferPool(RpcBufferPool.Buffer256k, 20, true, true);
			this.BufferPool96k = new BufferPool(RpcBufferPool.Buffer96k, 20, true, true);
			this.BufferPool32k = new BufferPool(RpcBufferPool.Buffer32k, 20, true, true);
			this.BufferPool4k = new BufferPool(RpcBufferPool.Buffer4k, 20, true, true);
			this.BufferPool1k = new BufferPool(RpcBufferPool.Buffer1k, 20, true, true);
		}

		public static byte[] GetBuffer(int requestedBufferSize)
		{
			if (requestedBufferSize < 0)
			{
				throw new FailRpcException("Buffer size cannot be negative", -2147024809);
			}
			if (requestedBufferSize == 0)
			{
				return RpcBufferPool.EmptyBuffer;
			}
			if (requestedBufferSize <= RpcBufferPool.Buffer1k)
			{
				return RpcBufferPool.Pool.BufferPool1k.Acquire();
			}
			if (requestedBufferSize <= RpcBufferPool.Buffer4k)
			{
				return RpcBufferPool.Pool.BufferPool4k.Acquire();
			}
			if (requestedBufferSize <= RpcBufferPool.Buffer32k)
			{
				return RpcBufferPool.Pool.BufferPool32k.Acquire();
			}
			if (requestedBufferSize <= RpcBufferPool.Buffer96k)
			{
				return RpcBufferPool.Pool.BufferPool96k.Acquire();
			}
			if (requestedBufferSize <= RpcBufferPool.Buffer256k)
			{
				return RpcBufferPool.Pool.BufferPool256k.Acquire();
			}
			throw new FailRpcException("Buffer size too large", -2147024809);
		}

		public static void ReleaseBuffer(byte[] buffer)
		{
			if (buffer != null)
			{
				if (buffer.Length == RpcBufferPool.Buffer256k)
				{
					RpcBufferPool.Pool.BufferPool256k.Release(buffer);
				}
				else if (buffer.Length == RpcBufferPool.Buffer96k)
				{
					RpcBufferPool.Pool.BufferPool96k.Release(buffer);
				}
				else if (buffer.Length == RpcBufferPool.Buffer32k)
				{
					RpcBufferPool.Pool.BufferPool32k.Release(buffer);
				}
				else if (buffer.Length == RpcBufferPool.Buffer4k)
				{
					RpcBufferPool.Pool.BufferPool4k.Release(buffer);
				}
				else
				{
					int num = buffer.Length;
					if (num == RpcBufferPool.Buffer1k)
					{
						RpcBufferPool.Pool.BufferPool1k.Release(buffer);
					}
					else if (num != 0)
					{
						throw new ArgumentException("buffer being released doesn't match any buffer pool length");
					}
				}
			}
		}

		private static readonly int ExtendedHeaderSize = 8;

		private static readonly int Buffer256k = 262144;

		private static readonly int Buffer96k = RpcBufferPool.ExtendedHeaderSize + 98304;

		private static readonly int Buffer32k = RpcBufferPool.ExtendedHeaderSize + 32768;

		private static readonly int Buffer4k = RpcBufferPool.ExtendedHeaderSize + 4096;

		private static readonly int Buffer1k = RpcBufferPool.ExtendedHeaderSize + 1024;

		private static readonly byte[] EmptyBuffer = new byte[0];

		private readonly BufferPool BufferPool256k;

		private readonly BufferPool BufferPool96k;

		private readonly BufferPool BufferPool32k;

		private readonly BufferPool BufferPool4k;

		private readonly BufferPool BufferPool1k;

		private static readonly RpcBufferPool Pool = new RpcBufferPool();
	}
}
