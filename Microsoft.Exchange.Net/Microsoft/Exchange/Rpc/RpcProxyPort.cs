using System;

namespace Microsoft.Exchange.Rpc
{
	public enum RpcProxyPort
	{
		Default = 443,
		FrontEnd = 443,
		Backend,
		LegacyHttp = 80
	}
}
