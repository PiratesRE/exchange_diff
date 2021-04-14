using System;

namespace Microsoft.Exchange.Security.RightsManagement.Protectors
{
	internal enum MsoIpiStatus
	{
		Unknown,
		ProtectSuccess,
		UnprotectSuccess,
		AlreadyProtected,
		CantProtect,
		AlreadyUnprotected,
		CantUnprotect,
		NotOwner,
		NotMyFile = 9,
		FileCorrupt,
		PlatformIrmFailed,
		BadInstall
	}
}
