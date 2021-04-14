using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderSync
{
	[Flags]
	public enum FolderSyncStatus
	{
		Success = 1,
		ServerError = 518,
		SyncKeyMismatchOrInvalid = 1033,
		IncorrectRequestFormat = 4106,
		UnknownError = 523,
		CodeUnknown = 268,
		ServerBusy = 8302
	}
}
