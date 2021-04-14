using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum MapiSaveMessageChangesFlags
	{
		None = 0,
		IMAPIDChange = 1,
		ForceSave = 2,
		SkipMailboxQuotaCheck = 4,
		SkipFolderQuotaCheck = 8,
		SkipQuotaCheck = 12,
		NonFatalDuplicateKey = 16,
		ForceCreatedEventForCopy = 32,
		Submit = 65536,
		SkipSizeCheck = 131072,
		Delivery = 262144
	}
}
