using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum SaveMessageChangesFlags : byte
	{
		None = 0,
		IMAPIDChange = 1,
		ForceSave = 2,
		SkipMailboxQuotaCheck = 4,
		SkipFolderQuotaCheck = 8,
		SkipQuotaCheck = 12,
		NonFatalDuplicateKey = 16,
		ForceCreatedEventForCopy = 32,
		TimerEventFired = 64
	}
}
