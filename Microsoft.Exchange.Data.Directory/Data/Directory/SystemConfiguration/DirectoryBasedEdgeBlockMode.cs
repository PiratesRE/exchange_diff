using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DirectoryBasedEdgeBlockMode
	{
		[LocDescription(DirectoryStrings.IDs.DirectoryBasedEdgeBlockModeDefault)]
		Default,
		[LocDescription(DirectoryStrings.IDs.DirectoryBasedEdgeBlockModeOff)]
		Off,
		[LocDescription(DirectoryStrings.IDs.DirectoryBasedEdgeBlockModeOn)]
		On
	}
}
