using System;

namespace Microsoft.Exchange.Data.Directory
{
	public enum IPAddressFamily
	{
		[LocDescription(DirectoryStrings.IDs.IPv4Only)]
		IPv4Only,
		[LocDescription(DirectoryStrings.IDs.IPv6Only)]
		IPv6Only,
		[LocDescription(DirectoryStrings.IDs.Any)]
		Any
	}
}
