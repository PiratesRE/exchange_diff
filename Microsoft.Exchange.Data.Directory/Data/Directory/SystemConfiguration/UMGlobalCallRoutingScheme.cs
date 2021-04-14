using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum UMGlobalCallRoutingScheme
	{
		[LocDescription(DirectoryStrings.IDs.None)]
		None,
		[LocDescription(DirectoryStrings.IDs.E164)]
		E164,
		[LocDescription(DirectoryStrings.IDs.GatewayGuid)]
		GatewayGuid,
		[LocDescription(DirectoryStrings.IDs.Reserved1)]
		Reserved1,
		[LocDescription(DirectoryStrings.IDs.Reserved2)]
		Reserved2,
		[LocDescription(DirectoryStrings.IDs.Reserved3)]
		Reserved3
	}
}
