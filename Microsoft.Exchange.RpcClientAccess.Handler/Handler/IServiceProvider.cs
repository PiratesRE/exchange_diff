using System;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal interface IServiceProvider<T>
	{
		T Get();
	}
}
