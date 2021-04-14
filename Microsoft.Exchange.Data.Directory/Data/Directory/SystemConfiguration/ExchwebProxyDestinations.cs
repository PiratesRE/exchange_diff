using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ExchwebProxyDestinations
	{
		[LocDescription(DirectoryStrings.IDs.NotSpecified)]
		NotSpecified,
		[LocDescription(DirectoryStrings.IDs.MailboxServer)]
		MailboxServer,
		[LocDescription(DirectoryStrings.IDs.PublicFolderServer)]
		PublicFolderServer
	}
}
