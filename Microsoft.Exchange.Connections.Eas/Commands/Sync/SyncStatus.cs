using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.Sync
{
	[Flags]
	public enum SyncStatus
	{
		Success = 1,
		InvalidSyncKey = 1027,
		ProtocolError = 4100,
		ServerError = 517,
		ErrorInClientServerConversion = 4102,
		Conflict = 263,
		SyncItemNotFound = 4104,
		OutOfDisk = 265,
		FolderHierarchyChanged = 2060,
		IncompleteSyncCommand = 4109,
		InvalidWaitTime = 4110,
		SyncTooManyFolders = 4111,
		Retry = 272,
		ServerBusy = 8302,
		CompositeStatusError = 510
	}
}
