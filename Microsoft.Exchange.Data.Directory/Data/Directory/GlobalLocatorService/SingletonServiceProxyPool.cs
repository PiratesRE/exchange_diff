using System;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class SingletonServiceProxyPool<T> : IServiceProxyPool<T>
	{
		internal SingletonServiceProxyPool(T serviceProxy)
		{
			this.serviceProxy = serviceProxy;
		}

		public T Acquire()
		{
			return this.serviceProxy;
		}

		public void Release(T serviceProxy)
		{
		}

		private T serviceProxy;
	}
}
