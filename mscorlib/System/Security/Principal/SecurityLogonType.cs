using System;

namespace System.Security.Principal
{
	[Serializable]
	internal enum SecurityLogonType
	{
		Interactive = 2,
		Network,
		Batch,
		Service,
		Proxy,
		Unlock
	}
}
