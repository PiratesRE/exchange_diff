using System;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	[Flags]
	public enum GetMailboxInfoFlags
	{
		None = 0,
		IgnoreHomeMdb = 1,
		BypassSharedCache = 2
	}
}
