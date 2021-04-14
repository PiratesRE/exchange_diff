using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum RpcClientAccessResponsibility
	{
		None = 0,
		Mailboxes = 1,
		PublicFolders = 2
	}
}
