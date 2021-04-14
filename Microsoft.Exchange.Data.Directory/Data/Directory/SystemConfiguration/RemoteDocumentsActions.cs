using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RemoteDocumentsActions
	{
		[LocDescription(DirectoryStrings.IDs.Allow)]
		Allow,
		[LocDescription(DirectoryStrings.IDs.Block)]
		Block
	}
}
