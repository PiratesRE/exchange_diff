using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	[Flags]
	public enum MountFlags : uint
	{
		None = 0U,
		ForceDatabaseCreation = 1U,
		AllowDatabasePatch = 2U,
		AcceptDataLoss = 4U,
		LogReplay = 8U
	}
}
