using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ElcFolderCategory
	{
		[LocDescription(DirectoryStrings.IDs.DefaultFolder)]
		ManagedDefaultFolder,
		[LocDescription(DirectoryStrings.IDs.OrganizationalFolder)]
		ManagedCustomFolder
	}
}
