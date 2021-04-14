using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Flags]
	internal enum ServiceControlManagerAccessFlags
	{
		Connect = 1,
		CreateService = 2,
		EnumerateService = 4,
		Lock = 8,
		QueryLockStatus = 16,
		ModifyBootConfig = 32,
		AllAccess = 63
	}
}
