using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	public enum ADTrustType
	{
		[LocDescription(DirectoryStrings.IDs.NotTrust)]
		None,
		[LocDescription(DirectoryStrings.IDs.ExternalTrust)]
		External,
		[LocDescription(DirectoryStrings.IDs.ForestTrust)]
		Forest
	}
}
