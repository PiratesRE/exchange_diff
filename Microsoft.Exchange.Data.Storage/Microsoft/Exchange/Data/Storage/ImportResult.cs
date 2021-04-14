using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ImportResult
	{
		Unknown,
		Success,
		SyncClientChangeNewer,
		SyncObjectDeleted,
		SyncIgnore,
		SyncConflict,
		NotFound,
		ObjectModified,
		ObjectDeleted
	}
}
