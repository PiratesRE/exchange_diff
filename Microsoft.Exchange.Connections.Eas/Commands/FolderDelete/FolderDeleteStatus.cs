using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderDelete
{
	[Flags]
	public enum FolderDeleteStatus
	{
		Success = 1,
		SystemFolder = 4099,
		FolderNotFound = 4100,
		ServerError = 518,
		SyncKeyMismatchOrInvalid = 1033,
		IncorrectRequestFormat = 4106,
		UnknownError = 523,
		CodeUnknown = 268
	}
}
