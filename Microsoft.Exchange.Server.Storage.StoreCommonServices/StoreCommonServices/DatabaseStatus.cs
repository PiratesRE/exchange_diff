using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	[Flags]
	internal enum DatabaseStatus
	{
		OffLine = 0,
		OnLine = 1,
		BackupInProgress = 2,
		InInteg = 4,
		IsPublic = 8,
		ForRecovery = 16,
		Maintenance = 32,
		MountInProgress = 64,
		AttachedReadOnly = 128
	}
}
