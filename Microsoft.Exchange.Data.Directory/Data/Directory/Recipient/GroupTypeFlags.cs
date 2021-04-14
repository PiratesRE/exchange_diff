using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum GroupTypeFlags
	{
		[LocDescription(DirectoryStrings.IDs.GroupTypeFlagsNone)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.GroupTypeFlagsGlobal)]
		Global = 2,
		[LocDescription(DirectoryStrings.IDs.GroupTypeFlagsDomainLocal)]
		DomainLocal = 4,
		[LocDescription(DirectoryStrings.IDs.GroupTypeFlagsBuiltinLocal)]
		BuiltinLocal = 5,
		[LocDescription(DirectoryStrings.IDs.GroupTypeFlagsUniversal)]
		Universal = 8,
		[LocDescription(DirectoryStrings.IDs.GroupTypeFlagsSecurityEnabled)]
		SecurityEnabled = -2147483648
	}
}
