using System;

namespace Microsoft.Mapi
{
	internal enum MapiCollectorStatus
	{
		Success,
		SyncClientChangeNewer = 264225,
		SyncObjectDeleted = -2147219456,
		SyncIgnore,
		SyncConflict,
		NotFound = -2147221233,
		ObjectModified = -2147221239,
		ObjectDeleted,
		Failed = -2147467259
	}
}
