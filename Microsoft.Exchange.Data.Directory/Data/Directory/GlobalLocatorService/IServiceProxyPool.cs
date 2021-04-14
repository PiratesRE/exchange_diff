using System;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal interface IServiceProxyPool<T>
	{
		T Acquire();

		void Release(T serviceProxy);
	}
}
