using System;

namespace Microsoft.Exchange.Data.Transport
{
	[Flags]
	public enum PermissionCheckResults
	{
		None = 0,
		Allow = 1,
		AdministratorDeny = 2,
		MachineDeny = 4,
		Deny = 6
	}
}
