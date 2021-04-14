using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MdbStatusFlags
	{
		Offline = 0,
		Online = 1,
		Backup = 2,
		Isinteg = 4,
		IsPublic = 8,
		InRecoverySG = 16,
		Maintenance = 32,
		MountInProgress = 64,
		AttachedReadOnly = 128
	}
}
