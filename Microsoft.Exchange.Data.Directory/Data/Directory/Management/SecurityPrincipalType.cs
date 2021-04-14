using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	public enum SecurityPrincipalType
	{
		[LocDescription(DirectoryStrings.IDs.SecurityPrincipalTypeNone)]
		None,
		[LocDescription(DirectoryStrings.IDs.SecurityPrincipalTypeUser)]
		User,
		[LocDescription(DirectoryStrings.IDs.SecurityPrincipalTypeGroup)]
		Group,
		[LocDescription(DirectoryStrings.IDs.SecurityPrincipalTypeComputer)]
		Computer,
		[LocDescription(DirectoryStrings.IDs.SecurityPrincipalTypeWellknownSecurityPrincipal)]
		WellknownSecurityPrincipal,
		[LocDescription(DirectoryStrings.IDs.SecurityPrincipalTypeUniversalSecurityGroup)]
		UniversalSecurityGroup,
		[LocDescription(DirectoryStrings.IDs.SecurityPrincipalTypeGlobalSecurityGroup)]
		GlobalSecurityGroup
	}
}
