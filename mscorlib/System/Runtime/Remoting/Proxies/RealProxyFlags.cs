using System;

namespace System.Runtime.Remoting.Proxies
{
	[Flags]
	internal enum RealProxyFlags
	{
		None = 0,
		RemotingProxy = 1,
		Initialized = 2
	}
}
