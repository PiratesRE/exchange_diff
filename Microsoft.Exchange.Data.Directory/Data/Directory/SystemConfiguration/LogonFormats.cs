using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum LogonFormats
	{
		[LocDescription(DirectoryStrings.IDs.FullDomain)]
		FullDomain,
		[LocDescription(DirectoryStrings.IDs.PrincipalName)]
		PrincipalName,
		[LocDescription(DirectoryStrings.IDs.UserName)]
		UserName
	}
}
