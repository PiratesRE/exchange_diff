using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderCreate
{
	[Flags]
	public enum FolderCreateStatus
	{
		Success = 1,
		FolderExists = 4098,
		ParentFolderNotFound = 4101,
		ServerError = 518,
		SyncKeyMismatchOrInvalid = 1033,
		IncorrectRequestFormat = 4106,
		UnknownError = 523,
		CodeUnknown = 268
	}
}
