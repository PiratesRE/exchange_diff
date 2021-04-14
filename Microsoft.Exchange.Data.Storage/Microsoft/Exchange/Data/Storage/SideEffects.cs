using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum SideEffects
	{
		None = 0,
		OpenToDelete = 1,
		DeleteFB = 2,
		RemoteItem = 4,
		NoFrame = 8,
		CoerceToInbox = 16,
		OpenToCopy = 32,
		OpenToMove = 64,
		Frame = 128,
		OpenForCtxMenu = 256,
		AbortSubmit = 512,
		CannotUndoDelete = 1024,
		CannotUndoCopy = 2048,
		CannotUndoMove = 4096,
		HasScript = 8192,
		OpenToPermDelete = 16384
	}
}
