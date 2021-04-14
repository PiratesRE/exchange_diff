using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Flags]
	public enum ManagementRoleEntryType
	{
		Cmdlet = 1,
		Script = 2,
		ApplicationPermission = 4,
		WebService = 8,
		All = 255
	}
}
