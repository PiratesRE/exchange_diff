using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum CopyToFlags
	{
		None = 0,
		MoveProperties = 1,
		DoNotReplaceProperties = 2,
		CopyRecipients = 4,
		CopyAttachments = 8,
		CopyHierarchy = 16,
		CopyContent = 32,
		CopyHiddenItems = 64,
		CopyEmbeddedMessage = 128,
		CopyFirstLevelEmbeddedMessage = 256
	}
}
