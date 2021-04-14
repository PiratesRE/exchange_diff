using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum FolderAdminFlags
	{
		ProvisionedFolder = 1,
		ProtectedFolder = 2,
		DisplayComment = 4,
		HasQuota = 8,
		RootFolder = 16,
		TrackFolderSize = 32,
		DumpsterFolder = 64
	}
}
