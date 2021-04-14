using System;

namespace Microsoft.Exchange.EseRepl
{
	internal class SocketStreamAsyncArgsPool : Pool<SocketStreamAsyncArgs>, IDisposable
	{
		public SocketStreamAsyncArgsPool(int preAllocCount) : base(preAllocCount)
		{
			for (int i = 0; i < preAllocCount; i++)
			{
				SocketStreamAsyncArgs o = new SocketStreamAsyncArgs(true);
				this.TryReturnObject(o);
			}
		}

		public void Dispose()
		{
			this.EmptyPool();
		}

		public void EmptyPool()
		{
			SocketStreamAsyncArgs socketStreamAsyncArgs;
			while ((socketStreamAsyncArgs = this.TryGetObject()) != null)
			{
				socketStreamAsyncArgs.Dispose();
			}
		}
	}
}
