using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MountFlags
	{
		None = 0,
		ForceDatabaseCreation = 1,
		AllowDatabasePatch = 2,
		AcceptDataLoss = 4,
		LogReplay = 8
	}
}
