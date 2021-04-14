using System;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal interface IRpcClient : IDisposable
	{
		string BindingString { get; }
	}
}
