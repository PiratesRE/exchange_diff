using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderUpdate
{
	[Flags]
	public enum FolderUpdateStatus
	{
		Success = 1,
		FolderExists = 4098,
		FolderNotFound = 4100,
		ParentFolderNotFound = 4101,
		ServerError = 518,
		SyncKeyMismatchOrInvalid = 1033,
		IncorrectRequestFormat = 4106,
		UnknownError = 523,
		CodeUnknown = 268
	}
}
