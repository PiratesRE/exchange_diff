using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum VirtualDirectoryTypes
	{
		[LocDescription(DirectoryStrings.IDs.NotSpecified)]
		NotSpecified,
		[LocDescription(DirectoryStrings.IDs.Mailboxes)]
		Mailboxes,
		[LocDescription(DirectoryStrings.IDs.PublicFolders)]
		PublicFolders,
		[LocDescription(DirectoryStrings.IDs.Exchweb)]
		Exchweb,
		[LocDescription(DirectoryStrings.IDs.Exadmin)]
		Exadmin
	}
}
