using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum AttachmentBlockingActions
	{
		[LocDescription(DirectoryStrings.IDs.Allow)]
		Allow,
		[LocDescription(DirectoryStrings.IDs.ForceSave)]
		ForceSave,
		[LocDescription(DirectoryStrings.IDs.Block)]
		Block
	}
}
