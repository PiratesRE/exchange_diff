using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum FolderChangeOperationFlags
	{
		None = 0,
		IncludeAssociated = 1,
		IncludeItems = 2,
		IncludeSubFolders = 4,
		IncludeAll = 7,
		DeclineCalendarItemWithResponse = 16,
		DeclineCalendarItemWithoutResponse = 32,
		CancelCalendarItem = 64,
		EmptyFolder = 256,
		DeleteAllClutter = 512,
		ClutterActionByUserOverride = 1024
	}
}
