using System;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	[Flags]
	public enum GetAddressInfoFlags
	{
		None = 0,
		BypassSharedCache = 1
	}
}
