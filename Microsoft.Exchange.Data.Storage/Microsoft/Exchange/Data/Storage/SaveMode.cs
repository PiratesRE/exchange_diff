using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum SaveMode
	{
		ResolveConflicts,
		FailOnAnyConflict,
		NoConflictResolution,
		NoConflictResolutionForceSave
	}
}
